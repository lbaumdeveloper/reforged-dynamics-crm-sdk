# Reforged.Dynamics.Crm.Sdk

Base classes and helpers for Dynamics 365 v9 **on-premises** plug-ins.

## How it is shipped

The NuGet package is a **source package**. Its `.cs` files are compiled into your plug-in assembly, so there is no extra DLL to deploy to the CRM server. Plug-in packages (dependent assemblies) are not available on-premises, and Microsoft doesn't support ILMerge, so a source package avoids both.

Requirements for the consuming project:

- `PackageReference` (SDK-style projects, or classic projects switched to `PackageReference`). `packages.config` does not support content files.
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

The SDK files then appear under the package in Solution Explorer and are compiled into your plug-in assembly. Register only your plug-in assembly with the Plug-in Registration Tool; `Microsoft.Xrm.Sdk.dll` and the other dependency DLLs in `bin` are already on the CRM server.

## Writing a plug-in

```csharp
public class AccountPlugin : PluginBase
{
    public AccountPlugin(string unsecureConfiguration, string secureConfiguration)
        : base(unsecureConfiguration, secureConfiguration)
    {
        RegisterEvent(PluginStage.PreOperation, PluginMessage.Create, "account", OnPreCreate);
    }

    private void OnPreCreate(ILocalPluginContext context)
    {
        var target = (Entity)context.PluginExecutionContext.InputParameters["Target"];
        context.Trace("Creating account {0}", target.GetAttributeValue<string>("name"));
    }
}
```

`PluginBase` traces entry and exit, runs every handler matching the current stage, message and entity, and wraps unexpected exceptions in `InvalidPluginExecutionException`. Plug-in instances are cached and shared between threads, so register handlers only in the constructor and keep no per-execution state in fields.

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
