# Copilot Instructions

## Project Details

* Surface Dial Tools for Visual Studio – a VSIX extension that adds Surface Dial support to Visual Studio through the Windows `RadialController` API; the features are described in #file:'README.md', the history in #file:'CHANGELOG.md'
* Installs into Visual Studio 17.0 and later, `amd64` only (see #file:'src/source.extension.vsixmanifest'); .NET Framework 4.8, `LangVersion` is `latest`
* The solution is #file:'DialControllerTools.slnx'. It has one old-style (non-SDK) project, #file:'src/DialControllerTools.csproj': every `.cs`, content and resource file is listed in it by hand, so a new file is neither compiled nor packaged until it is added there. Package versions are `PackageReference` items in the same file – there is no central package management
* Windows only: building needs Visual Studio with the "Visual Studio extension development" workload and a Windows SDK that matches the `Windows.WinMD` path in the project file. Use the MSBuild of Visual Studio; `dotnet build` fails on this project already at its `.resx` resources. The project also references private assemblies of the installed Visual Studio (see "Private Visual Studio Assemblies"), so the Visual Studio that you build with matters
* Build from the repository root in PowerShell. `msbuild` is usually not on `PATH`, so find it with `vswhere` (add `-version '[17.0,18.0)'` to build with Visual Studio 2022 instead of the latest one); `DeployExtension=false` keeps the build from installing the extension into the Experimental instance:

  ```powershell
  $msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
  & $msbuild DialControllerTools.slnx -restore -p:Configuration=Debug -p:DeployExtension=false -v:m
  ```

* Test: there is no test project. Run the extension in the Visual Studio Experimental instance (F5 starts `devenv.exe /rootsuffix Exp`, see the start options of the project) with a Surface Dial paired, and open a solution first: the host ignores the Dial while no solution is open
* CI: GitHub Actions (#file:'.github/workflows/build.yml') builds the solution in `Release` on `windows-2022` (Visual Studio 2022, the oldest Visual Studio that the extension supports; see "Private Visual Studio Assemblies") and uploads the VSIX as an artifact. A step stamps `Major.Minor.<run number>` into the manifest and into `Vsix.Version` of `source.extension.cs`, so change versions by hand only for a deliberate release bump. The hosted runner has its own Windows SDK versions, so a step passes the newest installed one as `TargetPlatformVersion`; that only helps while the `Windows.WinMD` reference in the project file is built from that property. Publishing to the Open VSIX Gallery is a manual run of the workflow with "publish" ticked

## Solution Structure

**IMPORTANT: Every new source file must be added to #file:'src/DialControllerTools.csproj'**: `<Compile Include>` for code and `<Content Include>` with `IncludeInVSIX` for icons. A file that is created outside Visual Studio is not added automatically.

```text
/
├── .claude/                                  – Claude Code settings (the vs-agent-plugins plugin) and helper scripts; `skills` is a local, git-ignored link to `.github/skills`
├── .github/                                  – these instructions, the `vs-extension-expert` agent, skills, prompts and workflows (the CI build among them)
├── art/                                      – images for the README
├── src/                                      – the extension, a single project
│   ├── DialPackage.cs                        – the `AsyncPackage`: registers the option pages and creates `DialControllerHost` once the shell is initialized
│   ├── Contracts/                            – the provider model
│   │   ├── IDialController.cs                – the controller contract and `GetControllersAsync`, which creates every controller through MEF
│   │   ├── IDialControllerProvider.cs        – the provider contract and `BaseDialControllerProvider`, which builds the Dial menu item from an icon
│   │   ├── ExportDialController.cs           – the `[DialControllerProvider(order)]` MEF export attribute and its metadata
│   │   ├── IDialControllerHost.cs            – what a controller may ask of the host (`RequestActivation`, `ReleaseActivation`)
│   │   ├── KnownProviders.cs                 – the monikers of the built-in controllers
│   │   └── RotationDirection.cs
│   ├── Controller/                           – the `RadialController` plumbing and the status bar indicator
│   │   ├── DialControllerHost.cs             – one `RadialController` per Visual Studio window (main and floating, tracked through private Visual Studio assemblies); routes clicks and rotations to the controller selected on the Dial
│   │   ├── RadialControllerInterop.cs        – COM interop that creates a `RadialController` for a window handle
│   │   ├── RadialControllerStatusBarHost.cs  – the WPF control of the status bar indicator (its template part is the private `TabItemTextControl`), styled in `Themes/Generic.xaml`
│   │   └── StatusbarInjector.cs              – inserts that control into the status bar of the main window
│   ├── Providers/                            – one folder per Dial menu item
│   │   ├── BaseController.cs                 – base class of the controllers
│   │   ├── BaseTextController.cs             – base class of the controllers that work on the active text editor
│   │   └── <Name>/                           – `<Name>ControllerProvider.cs`, `<Name>Controller.cs` and `icon.png` for Bookmarks, Customizable, Debug, Editor, Errors, Find, Navigate, Scroll and Zoom
│   ├── Helpers/                              – `VsHelpers` (DTE and VSIX file helpers), `VsCommands` (reads `Resources/commands.txt`), `PositiveIntegerConverter`
│   ├── Options.cs                            – the Tools > Options > Surface Dial > General page: menu visibility and scrolling (`ShowLogInOutput` is declared, but nothing reads it)
│   ├── CustomOptions.cs                      – the "Custom controls" page, with `CustomOptionsControl.*` as its UI
│   ├── Themes/Generic.xaml                   – the default style of the status bar control
│   ├── Resources/                            – the extension icon and `commands.txt`, the Visual Studio command names that the options page offers for the Customizable controller
│   ├── VSCommandTable.vsct / .cs             – the command table and its constants (no commands are defined yet)
│   ├── source.extension.vsixmanifest / .cs / .resx / .ico – the manifest and the files generated from it
│   ├── Properties/AssemblyInfo.cs            – assembly attributes, taken from the `Vsix` constants
│   ├── app.config
│   └── DialControllerTools.csproj
├── Directory.Build.targets                   – links `.claude/skills` to `.github/skills`
└── DialControllerTools.slnx
```

### How It Works

* `DialPackage` loads in the background once the shell is initialized, reads the options and, on idle, creates `DialControllerHost`, which creates a `RadialController` for the main window and for every floating window.
* Floating windows: `DialControllerHost` subscribes to `FloatingWindowManager.FloatingWindowCreated`, and it creates a `RadialController` for each `MainWindow` or `FloatingWindow` that exists at start and for each one that appears later; the controller of a window is dropped when the window unloads. These types come from private Visual Studio assemblies (see the next section).
* Every Dial menu item is a provider: a MEF export of `IDialControllerProvider` marked with `[DialControllerProvider(order)]`. `GetControllersAsync` creates them all and sorts them by `order`. A provider builds the menu item (the moniker is its text, the `icon.png` of its folder is its icon) and the controller that handles `OnClick` and `OnRotate`. The extension's own assembly is declared as a `MefComponent` asset in the manifest.
* A controller is created with `new` by its provider; `BaseDialControllerProvider` then satisfies its `[Import]` fields, so they are `null` in the constructor. A provider that throws or returns `null` is dropped: the error goes to the Dial output pane and the other providers still load.
* The host ignores the Dial while no solution is open. It routes a click or a rotation to the controller whose `Moniker` is the selected menu item, but only when that controller's `CanHandleClick` or `CanHandleRotate` is `true` (both are `false` in `BaseController`). It catches the exceptions of `OnClick` and `OnRotate` and writes them to the output pane; `CanHandleClick`, `CanHandleRotate` and `OnActivate` are not guarded.
* The status bar indicator is inserted into the `StatusBarPanel` element that is found in the visual tree of the main window – an implementation detail of Visual Studio that can change between releases.

### Private Visual Studio Assemblies

The extension has to know when Visual Studio creates a floating (undocked) window, and it does so with types of assemblies that are installed with Visual Studio but are not part of the public SDK (no NuGet package, no compatibility promise):

* `Microsoft.VisualStudio.Shell.ViewManager` – `FloatingWindowManager.FloatingWindowCreated`, `FloatingWindow` and `TabItemTextControl` (used by `Controller/DialControllerHost.cs`, `Controller/RadialControllerStatusBarHost.cs` and `Themes/Generic.xaml`)
* `Microsoft.VisualStudio.Shell.UI.Internal` – the `MainWindow` type (`Controller/DialControllerHost.cs`)
* `Microsoft.VisualStudio.Platform.WindowManagement` and `Microsoft.VisualStudio.Diagnostics.Assert` – referenced as well, although no source file uses their types

The project file references them with a `<HintPath>` into the Visual Studio that builds the extension (`$(MSBuildExtensionsPath)\..\Common7\IDE`, and `PrivateAssemblies` for the last one) and `<Private>False</Private>`. What follows from that:

* A package update does not touch them. They are resolved from the Visual Studio whose MSBuild runs the build.
* They are not a stable API: a Visual Studio update can rename or remove what the extension uses. When floating windows stop getting a Dial controller, or the extension fails with a `MissingMethodException` or a `TypeLoadException`, look here first.
* Their assembly version follows the Visual Studio that builds the extension: Visual Studio 2026 ships them as 18.0.0.0, while the public SDK assemblies stay at 17.0.0.0, so an extension built with Visual Studio 2026 references 18.0.0.0 of the first two. Compare that with the `[17.0,)` install target of the manifest before you claim support for an older Visual Studio, and build with the oldest Visual Studio that must be supported: a newer host usually redirects the older references to its own assemblies, an older host cannot satisfy a newer reference.
* Never set `Private` to `True`: the assemblies belong to Visual Studio and must not be shipped in the VSIX. Do not remove a reference, even one that no source file uses, without a build and a run with a floating window. Do not add other private APIs without a clear reason and a fallback.

### Adding a Dial Menu Item

Copy a small provider such as `Providers/Zoom/` and change all of these; a miss compiles and fails at run time:

1. Add the value to `KnownProviders`. The provider's `Moniker` is `nameof(KnownProviders.X)`; the folder name may differ from the value (`Navigate` uses `KnownProviders.Navigation`).
2. In `Options.cs` add a `ShowXMenu` property and its entry in `InitializeMenuVisibility`. The host reads `MenuVisibility[moniker]` for every controller, so a missing entry throws a `KeyNotFoundException` while the host is created: `DialPackage` only logs it, the main window gets no `RadialController`, and `DialPackage.DialControllerHost` stays `null`, which the Errors, Bookmarks and Debug controllers dereference.
3. Add `Providers/X/XControllerProvider.cs` (`[DialControllerProvider(order: n)]`, derived from `BaseDialControllerProvider`; pick an unused `order`, 1 to 9 are taken) and `XController.cs` (derived from `BaseController`, or from `BaseTextController` when it works on the text editor). Override `CanHandleClick` and `CanHandleRotate` to return `true` for what the controller handles – without it the host never calls it. If the controller touches COM objects in its constructor, switch to the UI thread in the provider before creating it, as `Providers/Errors/ErrorsControllerProvider.cs` does.
4. Add `Providers/X/icon.png`, registered in the project as `<Content>` with `IncludeInVSIX`; the provider loads it with `VsHelpers.GetFileInVsix(@"Providers\X\icon.png")`, and a missing item makes the provider fail and be dropped.
5. List the new `.cs` files in `<Compile Include>` of the project.
6. Describe the feature in `README.md` and `CHANGELOG.md`.

## Visual Studio Extension Rules

* Threading: use Visual Studio COM objects (`DTE`, `IVs*` services) and WPF elements on the UI thread only. Switch with `await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken)` before you create or call such an object. The Errors, Bookmarks and Debug providers get the `DTE`, switch, and only then create the controller; most other providers, such as Zoom, do not switch, so do not copy them for a controller that touches COM objects. Never block with `.Result`, `.Wait()` or `JoinableTaskFactory.Run`, and do not use `ConfigureAwait(false)` in code that returns to the UI thread. The VSTHRD analyzers come with the SDK packages: fix what they report, and where a suppression is unavoidable use a narrow `#pragma warning disable` / `restore` pair, as the existing code does.
* Errors: never swallow a caught exception – log it with `await ex.LogAsync("context")` (Community.VisualStudio.Toolkit) or write it to the Dial output pane (`DialPackage.GetOutputPaneAsync()`), and never let an exception escape from an event handler into Visual Studio. `async void` is for event handlers only. `VsHelpers.ExecuteCommand` swallows every exception and returns `false`, and the controllers ignore the result, so a mistyped command name silently does nothing: check each name against `Resources/commands.txt`.
* Theming: UI follows the Visual Studio theme – use `EnvironmentColors` and `VsFonts` resource keys with `DynamicResource` (see `Themes/Generic.xaml`), never hard-coded colors or fonts.
* Generated files: `source.extension.cs` and `VSCommandTable.cs` carry an `<auto-generated>` header, and `source.extension.resx` is generated from the manifest too, without a header. The `VsixManifestGenerator` and `VsctGenerator` custom tools set in the project file make them inside Visual Studio when a generator extension (VSIX Synchronizer, earlier Extensibility Tools) is installed; MSBuild and agents cannot run them, so edit the files by hand and keep them in sync with `source.extension.vsixmanifest` and `VSCommandTable.vsct`. They are out of sync now: `Vsix.Id`, `Vsix.Name` and `Vsix.Version` differ from the manifest, and the CI build stamps only `Version`, so do not rely on the other constants.
* Failures that appear only at run time: a missing manifest asset (such as the `MefComponent` one), a missing `<Content>` item for an icon, a mismatched id in the `.vsct`, or a Visual Studio update that changes a private assembly compiles fine and breaks the extension later – no provider is found, a provider is dropped, a command does nothing, floating windows get no controller. Check such changes in the Experimental instance.
* .NET Framework 4.8: language features that need no runtime support work (file-scoped namespaces, pattern matching, switch expressions, target-typed `new`, `is not null`); `record` and `init` need an `IsExternalInit` polyfill type, which the project does not have. The runtime has no `Span<T>`/`Memory<T>`, `Index`/`Range`, default interface members or `IAsyncEnumerable<T>` without a polyfill package. Check that an API exists in .NET Framework 4.8 before using it.
* Package versions: Visual Studio loads its own copy of some libraries, so a newer package in the project can break the extension at run time. The build warns about it (CVSTBLD002 for `Newtonsoft.Json` above 13.0.1): check such a warning before you raise a package version.

## Libraries in Use

* [`Community.VisualStudio.Toolkit.17`](https://github.com/VsixCommunity/Community.VisualStudio.Toolkit) – the `VS.*` helpers, `OutputWindowPane`, `LogAsync`
* [Visual Studio SDK](https://learn.microsoft.com/visualstudio/extensibility/) packages (`Shell`, `Editor`, `Language.Intellisense`, `ComponentModelHost`, interop assemblies, `EnvDTE*`) – the extensibility APIs
* `Microsoft.VSSDK.BuildTools` – builds the VSIX (`.vsct`, `.pkgdef`, packaging)
* `Windows.UI.Input.RadialController` – the Surface Dial API, referenced from the Windows SDK metadata (`Windows.WinMD`) and not shipped in the VSIX

Use GitHub MCP tools for code search in these repositories when needed.

## Agents and Skills

* Use the `vs-extension-expert` agent (#file:'.github/agents/vs-extension-expert.agent.md') for work on the extension code.
* Skills live in `.github/skills`; `Directory.Build.targets` links the folder to the git-ignored `.claude/skills` for Claude Code. They are copied from [`vs-agent-plugins`](https://github.com/madskristensen/vs-agent-plugins) by Mads Kristensen, see #file:'.github/skills/README.md': `handling-async-threading`, `handling-extension-errors`, `theming-extension-ui`, `adding-options-settings`, `providing-consuming-services` and `interacting-solution-explorer`. When a task matches the domain of a skill, load it and follow it before you write code.
* `.claude/settings.json` also enables the whole catalogue (40 skills) as the Claude Code plugin `vs-extensibility-skills`; if Claude Code has not offered it, install it with `claude plugin install vs-extensibility-skills@vs-agent-plugins --scope project`.
* The skills describe three approaches: VisualStudio.Extensibility (out of process), Community.VisualStudio.Toolkit and VSSDK. This extension is in-process VSSDK (`AsyncPackage`) with a few Toolkit helpers, so follow the VSSDK and Toolkit parts and ignore the VisualStudio.Extensibility ones.

## MCP Servers

Agents discover servers from #file:'.mcp.json'; these are only hints on when to prefer one.

* Use the `GitHub` server for code search in dependency repositories. In Claude Code it authenticates through `.claude/scripts/github-mcp-headers.ps1`, which reuses the GitHub token stored by Git Credential Manager, because the server does not support the OAuth dynamic client registration Claude Code needs.
* Use the `Microsoft Learn` server for official Microsoft documentation, the Visual Studio extensibility documentation included. Visual Studio bundles this server itself, so `.mcp.json` lists it for Claude Code only.
* When the solution is open in Visual Studio and an MCP server that exposes the IDE (`vs`) is connected, prefer its tools to the shell: build with it instead of resolving MSBuild, read the diagnostics from it instead of parsing the build output, and navigate with its symbol tools instead of text search.
* When adding or changing a server, keep `servers` (Visual Studio / VS Code / GitHub Copilot) and `mcpServers` (Claude Code) in sync and sorted alphabetically by key, except for the servers that Visual Studio already bundles (Microsoft Learn, NuGet): those go only in `mcpServers`.

## Definition of Done

Before reporting a task as finished:

1. Follow `.editorconfig` and the style of the surrounding code.
2. Build the solution and make sure that it succeeds with no errors; do not claim a successful build that you did not run.
3. Compare the warnings with the build before your change: the change must add none. The build already reports many (VSTHRD010 in most controllers, CS0649 for `[Import]` fields, MSB3277 version conflicts), so compare instead of expecting zero.
4. There is no automated test project, so say what you could not verify without a Surface Dial and the Experimental instance instead of claiming that it works. For window handling or the private assemblies that also means a floating (undocked) window.
5. Update `README.md` and `CHANGELOG.md` when a user-visible feature changes.

## C# Coding Guidelines

* Follow #file:'.editorconfig' (4 spaces, CRLF, braces on their own line, `System` usings first) and match the file that you edit. Do not reformat code that you do not change.
* Use `var` only where the type is apparent from the right-hand side; spell out built-in types and everything else. Use block bodies for methods and constructors; expression bodies are fine for properties and accessors.
* Use language keywords (`string`, `int`) and `nameof` instead of string literals for member names. Do not qualify members with `this.` unless it is needed.
* Use `is null` / `is not null` instead of `== null` / `!= null`, pattern matching instead of `as` with a null check or `is` with a cast, inline `out` declarations, and `?.Invoke` for events and delegates.
* Name async methods with the `Async` suffix, pass the `CancellationToken` through async chains, and give the token as the last parameter.
* Types live in the `DialControllerTools` namespace whatever the folder is; only `Helpers/VsCommands.cs` and `Helpers/PositiveIntegerConverter.cs` use `DialControllerTools.Helpers`. Most files use a block-scoped namespace; a new file may use a file-scoped one, as `Contracts/IDialController.cs` does. Do not convert existing files.
* Sort `using` directives with `System` first, separate the groups with a blank line and remove unused ones. Run Code Analysis in `Debug` and fix the new findings, as #file:'.github/CONTRIBUTING.md' asks.
* XML documentation: document new public and protected members. Start with `<summary>`, refer to types and members through `<see cref="..."/>` and to keywords through `<see langword="null"/>`, split paragraphs into `<para>` elements inside `<summary>`, write lists as `<list type="bullet">`, and put `/// <inheritdoc />` alone on an explicit interface implementation.

## Naming Conventions

* Use PascalCase for types, methods and public members, camelCase for locals and parameters, and camelCase for private fields (with the `_` prefix where the file already uses it).
* Prefix interface names with `I`.
* Name a provider `<Name>ControllerProvider` and its controller `<Name>Controller`; keep the folder name the same `<Name>`.

## General

* Make only high-confidence suggestions when reviewing code changes.
* Write code with good maintainability practices, including comments on why certain design decisions were made.
* Handle edge cases and write clear exception handling.
* Never duplicate code unless explicitly allowed.
* All comments, documentation, README files, and markdown files must be written in **English only**.
* Use an en dash (`–`) as a punctuation dash, never an em dash (`—`), and do not use emojis in text, comments or messages.
* In markdown, write each paragraph on one line instead of hard-wrapping it.
* Scripts and one-off automation: PowerShell 7 (`pwsh`, `.ps1`) or F# Interactive (`dotnet fsi`, `.fsx`); do not use Python.
* Commit messages follow the recent history of this repository: one short sentence in the past tense (`Simplified package initialization to fix loading issue`), no Conventional Commits prefix.
