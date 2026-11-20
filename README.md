# Reforged.Dynamics.Crm.Sdk

Base classes and helpers for Dynamics 365 v9 **on-premises** plug-ins.

## How it is shipped

The NuGet package is a **source package**. Its `.cs` files are compiled into your plug-in assembly, so there is no extra DLL to deploy to the CRM server. Plug-in packages (dependent assemblies) are not available on-premises, and Microsoft doesn't support ILMerge, so a source package avoids both.

Requirements for the consuming project:

- `PackageReference` or `packages.config`. With `PackageReference` the files are linked from the package and update with it. With `packages.config` NuGet copies them into a `Reforged.Dynamics.Crm.Sdk` folder in your project.
- Target framework .NET Framework 4.6.2 or 4.8.
- C# 7.3 or later. The SDK sources are kept at C# 7.3, the default for .NET Framework projects.

## Installing

The package is published to GitHub Packages. GitHub requires a token to install from it, even for public packages.

1. Create a [personal access token (classic)](https://github.com/settings/tokens) with the `read:packages` scope.
2. Add the feed once per machine:

   ```bash
   dotnet nuget add source https://nuget.pkg.github.com/lbaumdeveloper/index.json --name reforged --username <github-user> --password <token> --store-password-in-clear-text
   ```

   Visual Studio picks the source up from the user-level `NuGet.Config`.
3. Install `Reforged.Dynamics.Crm.Sdk` through **Manage NuGet Packages** on your plug-in project, or run:

   ```bash
   dotnet add package Reforged.Dynamics.Crm.Sdk
   ```

To try a build before it is published, download the `nuget-package` artifact from a workflow run, put the `.nupkg` in a folder and add that folder as a package source:

```bash
dotnet nuget add source C:\LocalNuGet --name LocalNuGet
```

The SDK files are then compiled into your plug-in assembly. Register only your plug-in assembly; `Microsoft.Xrm.Sdk.dll` and the other dependency DLLs in `bin` are already on the CRM server.

## Writing a plug-in

Steps are registered outside the code, for example with [spkl](https://github.com/scottdurow/SparkleXrm/wiki/spkl) `CrmPluginRegistration` attributes. Derive from `PluginBase` and implement `ExecutePlugin`; it runs for every step registered on the class.

```csharp
[CrmPluginRegistration(
    MessageNameEnum.Create, "account", StageEnum.PreOperation, ExecutionModeEnum.Synchronous,
    "", "AccountPlugin: Create of account", 1, IsolationModeEnum.Sandbox)]
public class AccountPlugin : PluginBase
{
    public AccountPlugin(string unsecureConfiguration, string secureConfiguration)
        : base(unsecureConfiguration, secureConfiguration)
    {
    }

    protected override void ExecutePlugin(ILocalPluginContext context)
    {
        context.Trace("Creating account {0}", context.GetTarget().GetAttributeValue<string>("name"));
    }
}
```

When one class has several steps, branch with `context.Is`:

```csharp
protected override void ExecutePlugin(ILocalPluginContext context)
{
    if (context.Is(PluginStage.PreOperation, PluginMessage.Update))
    {
        // ...
    }
    else if (context.Is(PluginStage.PostOperation, PluginMessage.Create))
    {
        // ...
    }
}
```

Context helpers:

| Method | Returns |
| --- | --- |
| `context.Is(stage, message)` | true when the execution matches the stage and message |
| `context.GetTarget()` | the `Target` entity, or null |
| `context.GetPreImage(name)` / `context.GetPostImage(name)` | the image, or null when it isn't registered |
| `context.TryGetPreImage(out image, name)` / `context.TryGetPostImage(out image, name)` | false when the image isn't registered |

The image name defaults to `PluginImage.PreImage` / `PluginImage.PostImage`. Use the same constants in the spkl attribute (`Image1Name = PluginImage.PreImage`) so the names can't drift apart.

`PluginBase` traces entry and exit and wraps unexpected exceptions in `InvalidPluginExecutionException`. Plug-in instances are cached and shared between threads, so keep no per-execution state in fields.

## Releasing

Pushing a tag such as `v0.2.0` builds the package with version `0.2.0` and publishes it to GitHub Packages using the workflow's built-in token. No secret is needed.

```bash
git tag v0.2.0
git push origin v0.2.0
```

## Building

```bash
dotnet build -c Release
dotnet pack -c Release -o artifacts
```

The .NET SDK builds the .NET Framework targets on any OS through `Microsoft.NETFramework.ReferenceAssemblies`.
