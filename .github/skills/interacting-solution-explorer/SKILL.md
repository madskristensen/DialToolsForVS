---
name: interacting-solution-explorer
description: Programmatically interact with Solution Explorer in Visual Studio extensions. Use when the user asks about selecting items in Solution Explorer, expanding/collapsing nodes, filtering, getting the active selection, editing labels, navigating solution items, or querying projects/files in a Visual Studio IDE extension. Covers VisualStudio.Extensibility (out-of-process), VSIX Community Toolkit (in-process), and legacy VSSDK (in-process) approaches.
---

# Solution Explorer Integration in Visual Studio Extensions

Solution Explorer is the primary navigation tool window in Visual Studio. Extensions can programmatically select items, expand/collapse nodes, apply filters, and start label editing.

Programmatic access to Solution Explorer is essential for extensions that need to navigate users to specific files, synchronize selection with external state, or build custom workflows around the project tree. The VisualStudio.Extensibility approach uses a LINQ-like Project Query API for structured data access, while the in-process approaches provide direct window manipulation.

**When to use this vs. alternatives:**
- Query projects/files, select items, expand/collapse nodes → **Solution Explorer integration** (this skill)
- Add custom virtual nodes under existing hierarchy items → [vs-solution-explorer-nodes](../adding-solution-explorer-nodes/SKILL.md)
- React to solution/project open/close events → [vs-solution-events](../handling-solution-events/SKILL.md)
- Add context menu items to Solution Explorer nodes → [vs-context-menu](../adding-context-menus/SKILL.md)
- Open/read/write files discovered via Solution Explorer → [vs-file-document-ops](../managing-files-documents/SKILL.md)

---

## 1. VSIX Community Toolkit (in-process)

The toolkit provides a typed `SolutionExplorerWindow` wrapper for common interactions.

**NuGet package:** `Community.VisualStudio.Toolkit`
**Key namespace:** `Community.VisualStudio.Toolkit`

### Get the Solution Explorer window

```csharp
SolutionExplorerWindow solExp = await VS.Windows.GetSolutionExplorerWindowAsync();
```

### Get selected items

```csharp
SolutionExplorerWindow solExp = await VS.Windows.GetSolutionExplorerWindowAsync();
IEnumerable<SolutionItem> selected = await solExp.GetSelectionAsync();

foreach (SolutionItem item in selected)
{
    await VS.MessageBox.ShowAsync($"Selected: {item.Name} ({item.Type})");
}
```

Each `SolutionItem` provides `Name`, `Type` (project, folder, file, etc.), full `Path`, and access to its hierarchy.

### Set the selection

```csharp
SolutionExplorerWindow solExp = await VS.Windows.GetSolutionExplorerWindowAsync();

// Select a single item
SolutionItem project = await VS.Solutions.GetActiveProjectAsync();
solExp.SetSelection(project);

// Select multiple items
IEnumerable<SolutionItem> items = await GetMyItemsAsync();
solExp.SetSelection(items);
```

### Expand and collapse nodes

```csharp
SolutionExplorerWindow solExp = await VS.Windows.GetSolutionExplorerWindowAsync();
SolutionItem project = await VS.Solutions.GetActiveProjectAsync();

// Expand just this node
solExp.Expand(project, SolutionItemExpansionMode.Single);

// Expand this node and all descendants
solExp.Expand(project, SolutionItemExpansionMode.Recursive);

// Expand ancestors to reveal the item (without expanding the item itself)
solExp.Expand(project, SolutionItemExpansionMode.Ancestors);

// Collapse a node
solExp.Collapse(project);
```

**SolutionItemExpansionMode flags:**

| Flag | Behavior |
|---|---|
| `Single` | Expand only the specified item |
| `Recursive` | Expand the item and all its descendants |
| `Ancestors` | Expand parent nodes to make the item visible |

### Edit an item label (inline rename)

```csharp
SolutionExplorerWindow solExp = await VS.Windows.GetSolutionExplorerWindowAsync();
IEnumerable<SolutionItem> selected = await solExp.GetSelectionAsync();
SolutionItem item = selected.FirstOrDefault();

if (item != null)
{
    solExp.EditLabel(item);
}
```

### Solution Explorer filters

```csharp
SolutionExplorerWindow solExp = await VS.Windows.GetSolutionExplorerWindowAsync();

// Is any filter active?
bool filtered = solExp.IsFilterEnabled();

// Is a specific filter active?
bool myFilter = solExp.IsFilterEnabled<MyCustomFilter>();

// Get the current filter
CommandID currentFilter = solExp.GetCurrentFilter();

// Enable a custom filter
solExp.EnableFilter<MyCustomFilter>();

// Enable a filter by GUID and ID
solExp.EnableFilter(filterGroupGuid, filterId);

// Disable all filtering
solExp.DisableFilter();
```

### Working with solution items via VS.Solutions

```csharp
// Get the current solution
Solution solution = await VS.Solutions.GetCurrentSolutionAsync();

// Get the active project
Project project = await VS.Solutions.GetActiveProjectAsync();

// Find a specific project by name
SolutionItem item = await VS.Solutions.FindSolutionItemAsync("MyProject");

// Get all projects in the solution
IEnumerable<SolutionItem> allProjects = await VS.Solutions.GetAllProjectsAsync();
```

---

## 2. VSSDK (in-process, legacy)

The VSSDK provides low-level access through `IVsUIHierarchyWindow`, `IVsSolutionBuildManager`, and `IVsHierarchy`.

**NuGet package:** `Microsoft.VisualStudio.SDK`
**Key namespaces:** `Microsoft.VisualStudio.Shell.Interop`, `Microsoft.VisualStudio.Shell`

### Get the Solution Explorer window

```csharp
IVsUIHierarchyWindow solExp = GetSolutionExplorerWindow();

private IVsUIHierarchyWindow GetSolutionExplorerWindow()
{
    ThreadHelper.ThrowIfNotOnUIThread();
    IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
    Guid slnExplorerGuid = new Guid(ToolWindowGuids80.SolutionExplorer);
    IVsWindowFrame frame;
    uiShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref slnExplorerGuid, out frame);
    frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object docView);
    return (IVsUIHierarchyWindow)docView;
}
```

### Get the current selection

```csharp
ThreadHelper.ThrowIfNotOnUIThread();
IVsMonitorSelection monitorSelection =
    (IVsMonitorSelection)GetService(typeof(SVsShellMonitorSelection));

monitorSelection.GetCurrentSelection(
    out IntPtr hierarchyPtr,
    out uint itemId,
    out IVsMultiItemSelect multiSelect,
    out IntPtr containerPtr);

if (hierarchyPtr != IntPtr.Zero)
{
    IVsHierarchy hierarchy = (IVsHierarchy)
        Marshal.GetObjectForIUnknown(hierarchyPtr);
    Marshal.Release(hierarchyPtr);

    hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_Name, out object name);
    // name is the display name of the selected item
}
```

### Expand a node

```csharp
ThreadHelper.ThrowIfNotOnUIThread();
IVsUIHierarchyWindow solExp = GetSolutionExplorerWindow();
solExp.ExpandItem(
    (IVsUIHierarchy)hierarchy,
    itemId,
    EXPANDFLAGS.EXPF_ExpandFolder);
```

**EXPANDFLAGS:**

| Flag | Behavior |
|---|---|
| `EXPF_ExpandFolder` | Expand the item |
| `EXPF_CollapseFolder` | Collapse the item |
| `EXPF_SelectItem` | Select the item |
| `EXPF_ExpandFolderRecursively` | Expand recursively |
| `EXPF_ExpandParentsToShowItem` | Reveal the item by expanding parents |

### Navigate to a file in Solution Explorer

```csharp
// Using DTE (EnvDTE)
DTE2 dte = (DTE2)GetService(typeof(DTE));
dte.ExecuteCommand("SolutionExplorer.SyncWithActiveDocument");
```

---

## 3. VisualStudio.Extensibility (out-of-process) — Project Query API

The new extensibility model uses the **Project Query API** to interact with projects, solution folders, and files. It does not have a direct Solution Explorer window wrapper, but it provides powerful query and mutation capabilities for the solution tree.

**NuGet package:** `Microsoft.VisualStudio.Extensibility`
**Key namespace:** `Microsoft.VisualStudio.Extensibility`

### Access the workspace

```csharp
WorkspacesExtensibility workspace = this.Extensibility.Workspaces();
```

### Query all projects

```csharp
IQueryResults<IProjectSnapshot> allProjects = await workspace.QueryProjectsAsync(
    project => project.With(p => new { p.Name, p.Path, p.Guid }),
    cancellationToken);

foreach (IProjectSnapshot project in allProjects)
{
    string name = project.Name;
    string path = project.Path;
}
```

### Query files in a project

```csharp
IQueryResults<IFileSnapshot> files = await workspace.QueryProjectsAsync(
    project => project.Where(p => p.Guid == knownGuid)
        .Get(p => p.Files
            .With(f => new { f.Path, f.IsHidden, f.IsSearchable })),
    cancellationToken);
```

### Filter projects by capability

```csharp
IQueryResults<IProjectSnapshot> webProjects = await workspace.QueryProjectsByCapabilitiesAsync(
    project => project.With(p => new { p.Path, p.Guid }),
    "DotNetCoreWeb",
    cancellationToken);
```

### Query solution folders

```csharp
IQueryResults<ISolutionFolderSnapshot> folders = await workspace.QuerySolutionAsync(
    solution => solution.Get(s => s.SolutionFolders
        .With(folder => folder.Name)
        .With(folder => folder.IsNested)
        .With(folder => folder.VisualPath)),
    cancellationToken);
```

### Add a file to a project

```csharp
IQueryResult<IProjectSnapshot> result = await workspace.UpdateProjectsAsync(
    project => project.Where(p => p.Guid == knownGuid),
    project => project.CreateFile("NewFile.cs"),
    cancellationToken);
```

### Rename a file

```csharp
IQueryResult<IProjectSnapshot> result = await workspace.UpdateProjectsAsync(
    project => project.Where(p => p.Guid == knownGuid),
    project => project.RenameFile(filePath, newFileName),
    cancellationToken);
```

### Subscribe to project changes

```csharp
var solutions = await workspace.QuerySolutionAsync(
    solution => solution.With(s => s.FileName),
    cancellationToken);

var singleSolution = solutions.FirstOrDefault();
var unsubscriber = await singleSolution
    .AsQueryable()
    .With(p => p.Projects)
    .SubscribeAsync(new MyObserver(), CancellationToken.None);
```

> **Note:** The Project Query API provides data access and mutation over the solution tree but does not directly control the Solution Explorer UI (selection, expansion, scrolling). For full UI control, an in-process component is still needed.

---

## Troubleshooting

- **`GetSolutionExplorerWindowAsync()` returns null:** Solution Explorer hasn't been opened yet. Call `VS.Windows.ShowToolWindowAsync<SolutionExplorerWindow>()` first, or access the window after the user has opened it.
- **Selection returns empty even though items are selected:** For VSSDK, you're using `GetSelection` before switching to the UI thread. Call `ThreadHelper.ThrowIfNotOnUIThread()` and ensure you're on the main thread.
- **Expand/collapse doesn't work:** The node GUID or item ID is wrong. Use `IVsUIHierarchyWindow.ExpandItem` with the correct `EXPANDFLAGS` value.
- **Project Query returns no results:** Ensure the solution is fully loaded. Use activation constraints or wait for `OnAfterBackgroundSolutionLoadComplete` before querying.

## What NOT to do

> **Do NOT** use `DTE.Solution` or `DTE.SelectedItems` in new extensions — deprecated, requires UI thread, limited vs. Project Query API or Toolkit wrappers.

> **Do NOT** manipulate Solution Explorer UI from a background thread — all `IVsUIHierarchyWindow` operations require the main thread.

> **Do NOT** cache `VSITEMID` values across sessions — they're not stable and may change on solution reload.

## See also

- [vs-solution-explorer-nodes](../adding-solution-explorer-nodes/SKILL.md)
- [vs-solution-events](../handling-solution-events/SKILL.md)
- [vs-context-menu](../adding-context-menus/SKILL.md)
- [vs-file-document-ops](../managing-files-documents/SKILL.md)

## Additional resources

- [VSIX Cookbook — Solution Explorer](https://www.vsixcookbook.com/recipes/solution-explorer.html)
- [VisualStudio.Extensibility — Project Query API](https://learn.microsoft.com/visualstudio/extensibility/visualstudio.extensibility/project/project)
- [VSProjectQueryAPISample](https://github.com/Microsoft/VSExtensibility/tree/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample)
