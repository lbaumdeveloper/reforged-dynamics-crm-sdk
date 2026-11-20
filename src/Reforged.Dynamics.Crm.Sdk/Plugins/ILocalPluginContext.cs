using System;
using Microsoft.Xrm.Sdk;

namespace Reforged.Dynamics.Crm.Sdk.Plugins
{
    /// <summary>
    /// Services and context available to a plug-in during a single execution.
    /// </summary>
    public interface ILocalPluginContext
    {
        IServiceProvider ServiceProvider { get; }

        IPluginExecutionContext PluginExecutionContext { get; }

        ITracingService TracingService { get; }

        IOrganizationServiceFactory OrganizationServiceFactory { get; }

        /// <summary>
        /// Organization service running as the user the step is executed for.
        /// </summary>
        IOrganizationService OrganizationService { get; }

        /// <summary>
        /// Organization service running as the SYSTEM user.
        /// </summary>
        IOrganizationService SystemOrganizationService { get; }
    }
}
