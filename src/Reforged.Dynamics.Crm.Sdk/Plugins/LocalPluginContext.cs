using System;
using Microsoft.Xrm.Sdk;

namespace Reforged.Dynamics.Crm.Sdk.Plugins
{
    /// <summary>
    /// Default <see cref="ILocalPluginContext"/> that resolves services from the plug-in service provider.
    /// Organization services are created on first access.
    /// </summary>
    public class LocalPluginContext : ILocalPluginContext
    {
        private IOrganizationService organizationService;
        private IOrganizationService systemOrganizationService;

        public LocalPluginContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            PluginExecutionContext = GetService<IPluginExecutionContext>(serviceProvider);
            TracingService = GetService<ITracingService>(serviceProvider);
            OrganizationServiceFactory = GetService<IOrganizationServiceFactory>(serviceProvider);
        }

        public IServiceProvider ServiceProvider { get; }

        public IPluginExecutionContext PluginExecutionContext { get; }

        public ITracingService TracingService { get; }

        public IOrganizationServiceFactory OrganizationServiceFactory { get; }

        public IOrganizationService OrganizationService =>
            organizationService ?? (organizationService = OrganizationServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId));

        public IOrganizationService SystemOrganizationService =>
            systemOrganizationService ?? (systemOrganizationService = OrganizationServiceFactory.CreateOrganizationService(null));

        public void Trace(string format, params object[] args)
        {
            if (TracingService == null || string.IsNullOrEmpty(format))
            {
                return;
            }

            if (args == null || args.Length == 0)
            {
                TracingService.Trace("{0}", format);
                return;
            }

            TracingService.Trace(format, args);
        }

        private static T GetService<T>(IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}
