---
name: providing-consuming-services
description: Provide and consume services in Visual Studio extensions. Use when the user asks how to create a custom VS service, register a service with ProvideServiceAttribute, consume services via GetServiceAsync or IServiceProvider, share state between extension components, use dependency injection with InitializeServices, proffer global or local services, query built-in VS services like SVsShell or SVsSolution, or use MEF imports to consume services in a Visual Studio IDE extension. Covers VisualStudio.Extensibility (out-of-process), VSIX Community Toolkit (in-process), and legacy VSSDK (in-process) approaches.
---

# Providing and Consuming Services in Visual Studio Extensions

A **service** in Visual Studio is a contract between two components: one component *proffers* (provides) a set of interfaces, and any other component can *consume* them by querying the service provider chain. Services are the primary mechanism for sharing functionality and state across packages, tool windows, commands, and editor components.

Without a proper service architecture, extension authors either tightly couple components (passing concrete instances through constructors), duplicate logic across packages, or resort to static singletons that break testability and lifetime guarantees. VS services solve this by providing a discoverable, lazily-created, lifetime-managed way to share behavior. In VSSDK, services also control *visibility* — a service can be global (available IDE-wide) or local (available only within the owning package's provider chain).

**When to use this vs. alternatives:**
- Sharing state or behavior between extension components (commands, tool windows, listeners) → **this skill**
- Consuming built-in VS services (e.g., `SVsShell`, `SVsSolution`, `IVsActivityLog`) → **this skill**
- MEF `[Import]`/`[Export]` for editor extension points (classifiers, taggers, completion sources) → use MEF composition directly; see [adding-editor-classifiers](../adding-editor-classifiers/SKILL.md), [creating-editor-taggers](../creating-editor-taggers/SKILL.md)
- Logging and error handling with services like `SVsActivityLog` → [handling-extension-errors](../handling-extension-errors/SKILL.md)
- Settings/options storage → [adding-options-settings](../adding-options-settings/SKILL.md)
- Threading concerns when calling services → [handling-async-threading](../handling-async-threading/SKILL.md)

## Decision guide

| Approach | Service scope | Discovery | Lifetime |
|----------|--------------|-----------|----------|
| **VisualStudio.Extensibility** | Extension-local (DI container) | Constructor injection via `InitializeServices` | Singleton / Transient / Scoped |
| **VSIX Community Toolkit** | Global (VS-wide) or local (package) | `VS.GetRequiredServiceAsync<S, I>()` or `VS.GetMefServiceAsync<T>()` | Managed by VS; lazy creation via callback |
| **VSSDK** | Global (VS-wide) or local (package) | `GetServiceAsync(typeof(S))` / `Package.GetGlobalService(typeof(S))` | Managed by VS; lazy creation via callback |

---

## 1. VisualStudio.Extensibility (out-of-process, recommended)

The VisualStudio.Extensibility SDK uses **.NET dependency injection** (`Microsoft.Extensions.DependencyInjection`) to share services between extension parts (commands, tool windows, listeners). Each extension has its own isolated DI container.

**NuGet package:** `Microsoft.VisualStudio.Extensibility`
**Key namespace:** `Microsoft.VisualStudio.Extensibility`

> **Note:** Services registered via `InitializeServices` are local to the extension — they are **not** visible to other extensions or to Visual Studio itself. This is by design for process isolation.

### Step 1: Define your service

```csharp
public class ProjectAnalysisService
{
    private readonly VisualStudioExtensibility extensibility;

    public ProjectAnalysisService(VisualStudioExtensibility extensibility)
    {
        this.extensibility = extensibility;
    }

    public async Task<int> CountOpenDocumentsAsync(CancellationToken ct)
    {
        // Use the extensibility object to interact with VS
        var documents = this.extensibility.Documents();
        // ... implementation
        return 0;
    }
}
```

### Step 2: Register in InitializeServices

Override `InitializeServices` in your `Extension` class to add the service to the DI container:

```csharp
[VisualStudioContribution]
public class MyExtension : Extension
{
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
            id: "MyExtension.a1b2c3d4-e5f6-7890-abcd-ef1234567890",
            version: this.ExtensionAssemblyVersion,
            publisherName: "MyPublisher",
            displayName: "My Extension",
            description: "Extension with shared services"),
    };

    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection); // Always call base

        // Singleton: one instance shared across all components
        serviceCollection.AddSingleton<ProjectAnalysisService>();

        // Transient: new instance per injection point
        serviceCollection.AddTransient<ReportGenerator>();
    }
}
```

### Step 3: Inject into extension parts

Any class marked with `[VisualStudioContribution]` can request the service via constructor injection:

```csharp
[VisualStudioContribution]
public class AnalyzeCommand : Command
{
    private readonly ProjectAnalysisService analysisService;

    public AnalyzeCommand(ProjectAnalysisService analysisService)
    {
        this.analysisService = analysisService;
    }

    public override CommandConfiguration CommandConfiguration => new("%MyExtension.AnalyzeCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.StatusInformation, IconSettings.IconAndText),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken ct)
    {
        int count = await this.analysisService.CountOpenDocumentsAsync(ct);
        await this.Extensibility.Shell().ShowPromptAsync(
            $"Found {count} open documents.",
            PromptOptions.OK,
            ct);
    }
}
```

### Service lifetimes

| Lifetime | Behavior | Use when |
|----------|----------|----------|
| `AddSingleton<T>()` | One instance for the entire extension lifetime | Shared state, caches, configuration |
| `AddTransient<T>()` | New instance per injection | Stateless helpers, UI controls |
| `AddScoped<T>()` | One instance per contributed component scope | Rarely needed; scope usually equals extension lifetime |

### Built-in services available via DI

The SDK automatically registers these services — inject them via constructor parameters:

- `VisualStudioExtensibility` — entry point for all VS interaction (documents, shell, editor)
- `TraceSource` — structured logging
- `IServiceBroker` — access to brokered services
- `IServiceProvider` — query the extension's own DI container

For in-process extensions, these additional services are available:
- `JoinableTaskFactory` / `JoinableTaskContext`
- `AsyncServiceProviderInjection<S, I>` — query VSSDK services
- `MefInjection<T>` — query MEF-exported services

### Consuming VSSDK services from an in-process VisualStudio.Extensibility extension

```csharp
[VisualStudioContribution]
public class MyCommand : Command
{
    private readonly AsyncServiceProviderInjection<SVsSolution, IVsSolution> solutionService;

    public MyCommand(
        AsyncServiceProviderInjection<SVsSolution, IVsSolution> solutionService)
    {
        this.solutionService = solutionService;
    }

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken ct)
    {
        IVsSolution solution = await this.solutionService.GetServiceAsync();
        // Use the solution service...
    }
}
```

---

## 2. VSIX Community Toolkit (in-process)

The Community Toolkit provides async helper methods for consuming VS services. For *providing* custom services, it uses the same VSSDK pattern (see section 3) but with the Toolkit's `AsyncPackage`-based infrastructure.

**NuGet package:** `Community.VisualStudio.Toolkit`
**Key namespace:** `Community.VisualStudio.Toolkit`

### Consuming built-in VS services

The Toolkit provides strongly-typed async accessors:

```csharp
// Get a VS service (service type S, interface type I)
IVsSolution solution = await VS.GetRequiredServiceAsync<SVsSolution, IVsSolution>();

// Get a MEF service
IContentTypeRegistryService contentTypeRegistry =
    await VS.GetMefServiceAsync<IContentTypeRegistryService>();

// Common shorthand helpers
DTE2 dte = await VS.GetServiceAsync<DTE, DTE2>();
```

### Providing a custom service

Define the service interface and marker type, then register using the Toolkit's package base class:

```csharp
// 1. Define the service marker (empty interface) and the service contract
public interface SMyAnalysisService { }

public interface IMyAnalysisService
{
    Task<int> AnalyzeAsync(string filePath);
}

// 2. Implement the service
public class MyAnalysisService : SMyAnalysisService, IMyAnalysisService
{
    private readonly AsyncPackage package;

    public MyAnalysisService(AsyncPackage package)
    {
        this.package = package;
    }

    public async Task<int> AnalyzeAsync(string filePath)
    {
        // Implementation
        return 42;
    }
}
```

```csharp
// 3. Register in your package
[ProvideService(typeof(SMyAnalysisService), IsAsyncQueryable = true)]
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid("your-package-guid-here")]
public sealed class MyPackage : ToolkitPackage
{
    protected override async Task InitializeAsync(
        CancellationToken cancellationToken,
        IProgress<ServiceProgressData> progress)
    {
        // Register the service with a factory callback (lazy creation)
        this.AddService(
            typeof(SMyAnalysisService),
            CreateMyAnalysisServiceAsync,
            promote: true); // true = globally visible; false = local to this package

        await base.InitializeAsync(cancellationToken, progress);
    }

    private async Task<object> CreateMyAnalysisServiceAsync(
        IAsyncServiceContainer container,
        CancellationToken cancellationToken,
        Type serviceType)
    {
        var service = new MyAnalysisService(this);
        return service;
    }
}
```

```csharp
// 4. Consume from another component
IMyAnalysisService analysis =
    await VS.GetRequiredServiceAsync<SMyAnalysisService, IMyAnalysisService>();
int result = await analysis.AnalyzeAsync("MyFile.cs");
```

### The `promote` parameter

- `promote: true` — The service is added to the **global** VS service provider. Any package or component in the IDE can query it.
- `promote: false` — The service is only available through this package's `IServiceProvider`. Other packages cannot see it.

---

## 3. VSSDK (in-process, legacy)

The VSSDK service model is the foundation that the Community Toolkit builds on. A `Package` (or `AsyncPackage`) implements `IServiceProvider` and `IServiceContainer`, allowing it to both consume and proffer services.

**NuGet package:** `Microsoft.VisualStudio.SDK`
**Key namespaces:** `Microsoft.VisualStudio.Shell`, `Microsoft.VisualStudio.Shell.Interop`, `Microsoft.VisualStudio.OLE.Interop`

### Consuming a VS service

**From an AsyncPackage (recommended):**

```csharp
protected override async Task InitializeAsync(
    CancellationToken cancellationToken,
    IProgress<ServiceProgressData> progress)
{
    await base.InitializeAsync(cancellationToken, progress);

    // Async query — does NOT block the UI thread
    IVsSolution solution =
        await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
}
```

**From a tool window, control, or non-package context:**

```csharp
// Use the static global service provider (package must be sited first)
IVsActivityLog log =
    Package.GetGlobalService(typeof(SVsActivityLog)) as IVsActivityLog;
```

**From an async context outside a package:**

```csharp
IVsShell shell = await AsyncServiceProvider.GlobalProvider
    .GetServiceAsync(typeof(SVsShell)) as IVsShell;
```

### Providing a custom service (synchronous — legacy)

```csharp
// 1. Define types
public interface SMyService { }     // Service marker type (for registration)
public interface IMyService         // Service contract
{
    string GetData();
}

public class MyService : SMyService, IMyService
{
    private readonly IServiceProvider serviceProvider;

    public MyService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public string GetData() => "Hello from MyService";
}
```

```csharp
// 2. Register and proffer the service
[ProvideService(typeof(SMyService))]
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid("your-package-guid-here")]
public sealed class MyPackage : AsyncPackage
{
    protected override async Task InitializeAsync(
        CancellationToken cancellationToken,
        IProgress<ServiceProgressData> progress)
    {
        await base.InitializeAsync(cancellationToken, progress);

        // Add with lazy callback; promote: true makes it globally visible
        this.AddService(typeof(SMyService), CreateServiceAsync, promote: true);
    }

    private Task<object> CreateServiceAsync(
        IAsyncServiceContainer container,
        CancellationToken cancellationToken,
        Type serviceType)
    {
        if (typeof(SMyService) == serviceType)
        {
            return Task.FromResult<object>(new MyService(this));
        }

        return Task.FromResult<object>(null);
    }
}
```

```csharp
// 3. Consume from another package or component
IMyService myService =
    await GetServiceAsync(typeof(SMyService)) as IMyService;
string data = myService?.GetData();
```

### Providing an async-queryable service

For services that should load asynchronously without blocking the UI thread:

```csharp
[ProvideService(typeof(SMyService), IsAsyncQueryable = true)]
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
public sealed class MyPackage : AsyncPackage
{
    // ...same pattern as above...
}
```

The `IsAsyncQueryable = true` flag tells VS that this service supports `GetServiceAsync` and will not force a synchronous package load.

### Common built-in VS services

| Service type (S) | Interface (I) | Purpose |
|-----------------|---------------|---------|
| `SVsSolution` | `IVsSolution` | Enumerate projects, monitor solution events |
| `SVsShell` | `IVsShell` | IDE state, properties, zombie state check |
| `SVsUIShell` | `IVsUIShell` | Window management, message boxes, refresh UI |
| `SVsActivityLog` | `IVsActivityLog` | Write to the VS activity log |
| `SVsOutputWindow` | `IVsOutputWindow` | Access Output Window panes |
| `SVsRunningDocumentTable` | `IVsRunningDocumentTable` | Track open documents |
| `SVsStatusbar` | `IVsStatusbar` | Status bar text and progress |
| `DTE` | `DTE2` | Top-level automation object |

---

## Key guidance

- **Always use `GetServiceAsync` instead of `GetService`** in `AsyncPackage`-based extensions to avoid blocking the UI thread.
- **Use `promote: true`** when you want other packages to consume your service globally; use `promote: false` for package-internal services.
- **Prefer constructor injection** in VisualStudio.Extensibility — don't call `GetService` manually.
- **Always call `base.InitializeServices(serviceCollection)`** when overriding `InitializeServices` in the new extensibility model.
- **Use the service marker pattern** (`SMyService` + `IMyService`) in VSSDK — the marker type (S) is used for registration and lookup, the interface type (I) is the contract you code against.
- **Register services before consuming them** — in `InitializeAsync`, call `AddService` before `GetServiceAsync` on the same service.
- **Never query services in a package constructor** — the package is not yet sited; `GetService` will return null.

## Troubleshooting

- **`GetServiceAsync` returns `null`:** The service provider package hasn't loaded yet, or the service wasn't registered with `[ProvideService]`. Verify the `ProvideServiceAttribute` is on the package class and the service type matches exactly. Also check that the service-providing extension is installed and enabled.
- **Service works in the provider package but is `null` from another package:** You're using `promote: false` (the default for `AddService`). Change to `promote: true` to make the service globally visible, or add it to the global service provider with `AddService(typeof(S), callback, promote: true)`.
- **`GetService` returns `null` in package constructor:** The package isn't sited yet. Move service queries to `InitializeAsync` or later. For non-package contexts, use `Package.GetGlobalService()` or `AsyncServiceProvider.GlobalProvider.GetServiceAsync()`.
- **Circular dependency at startup:** Package A services depend on Package B services, and vice versa. Break the cycle by using lazy service queries — don't query other services in the `AddService` callback; instead, resolve them on first use.
- **VisualStudio.Extensibility DI throws "service not registered":** You forgot to call `base.InitializeServices(serviceCollection)` in your `Extension` class, or the service type wasn't added with `AddSingleton`/`AddTransient` before the first component tried to inject it.
- **Async service blocks the UI thread:** You're using `GetService` (synchronous) instead of `GetServiceAsync`. Switch to the async variant and ensure `IsAsyncQueryable = true` is set on `[ProvideService]`.

## What NOT to do

> **Do NOT** query services in a package constructor — the package isn't sited yet; `GetService` returns `null`. Defer to `InitializeAsync`.

> **Do NOT** use `Package.GetGlobalService()` as the primary pattern in async code — runs synchronously. Prefer `GetServiceAsync` or `VS.GetRequiredServiceAsync<S, I>()`.

> **Do NOT** forget `promote: true` when registering a service for other packages — without it, external consumers get `null` with no error.

> **Do NOT** create services eagerly in `InitializeAsync` — use the `AddService` callback (lazy factory) so VS only creates on first query.

> **Do NOT** use synchronous `Package` — use `AsyncPackage` (VSSDK/Toolkit) or `Extension` (Extensibility). Sync `Package.Initialize()` blocks the UI thread.

> **Do NOT** share mutable state through a global service without synchronization — protect with locks, `SemaphoreSlim`, or immutable data.

## See also

- [handling-extension-errors](../handling-extension-errors/SKILL.md)
- [handling-async-threading](../handling-async-threading/SKILL.md)
- [adding-options-settings](../adding-options-settings/SKILL.md)
- [handling-solution-events](../handling-solution-events/SKILL.md)
- [handling-build-events](../handling-build-events/SKILL.md)

## References

- [Using and providing services (VSSDK)](https://learn.microsoft.com/visualstudio/extensibility/using-and-providing-services)
- [How to: Provide a service (VSSDK)](https://learn.microsoft.com/visualstudio/extensibility/how-to-provide-a-service)
- [How to: Get a service (VSSDK)](https://learn.microsoft.com/visualstudio/extensibility/how-to-get-a-service)
- [How to: Provide an asynchronous Visual Studio service](https://learn.microsoft.com/visualstudio/extensibility/how-to-provide-an-asynchronous-visual-studio-service)
- [Service essentials (VSSDK)](https://learn.microsoft.com/visualstudio/extensibility/internals/service-essentials)
- [Dependency injection in VisualStudio.Extensibility](https://learn.microsoft.com/visualstudio/extensibility/visualstudio.extensibility/inside-the-sdk/dependency-injection)
- [Extension anatomy (VisualStudio.Extensibility)](https://learn.microsoft.com/visualstudio/extensibility/visualstudio.extensibility/inside-the-sdk/extension-anatomy)
