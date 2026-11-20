using System;
using Microsoft.Xrm.Sdk;

namespace Reforged.Dynamics.Crm.Sdk.Plugins
{
    /// <summary>
    /// Base class for plug-ins whose steps are registered externally, for example with spkl's
    /// <c>CrmPluginRegistration</c> attributes. <see cref="Execute"/> builds the local context, traces entry and exit,
    /// and wraps unexpected exceptions in <see cref="InvalidPluginExecutionException"/>.
    /// The platform caches plug-in instances and may run them on several threads at once,
    /// so derived classes must not keep per-execution state in fields.
    /// </summary>
    public abstract class PluginBase : IPlugin
    {
        protected PluginBase()
            : this(null, null)
        {
        }

        protected PluginBase(string unsecureConfiguration, string secureConfiguration)
        {
            UnsecureConfiguration = unsecureConfiguration;
            SecureConfiguration = secureConfiguration;
        }

        protected string UnsecureConfiguration { get; }

        protected string SecureConfiguration { get; }

        /// <summary>
        /// Entry point called by the platform.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var localContext = CreateLocalPluginContext(serviceProvider);
            var executionContext = localContext.PluginExecutionContext;
            var pluginName = GetType().FullName;

            localContext.Trace(
                "Entered {0}. Message: {1}, Entity: {2}, Stage: {3}, Depth: {4}, CorrelationId: {5}",
                pluginName,
                executionContext.MessageName,
                executionContext.PrimaryEntityName,
                executionContext.Stage,
                executionContext.Depth,
                executionContext.CorrelationId);

            try
            {
                ExecutePlugin(localContext);
            }
            catch (InvalidPluginExecutionException exception)
            {
                localContext.Trace("{0} failed: {1}", pluginName, exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                localContext.Trace("{0} failed: {1}", pluginName, exception.ToString());
                throw new InvalidPluginExecutionException(
                    string.Format("An error occurred in {0}: {1}", pluginName, exception.Message),
                    exception);
            }
            finally
            {
                localContext.Trace("Exiting {0}.", pluginName);
            }
        }

        /// <summary>
        /// Runs the plug-in logic for every step registered on this class.
        /// </summary>
        protected abstract void ExecutePlugin(ILocalPluginContext context);

        /// <summary>
        /// Creates the context passed to <see cref="ExecutePlugin"/>. Override to supply a custom context.
        /// </summary>
        protected virtual ILocalPluginContext CreateLocalPluginContext(IServiceProvider serviceProvider)
        {
            return new LocalPluginContext(serviceProvider);
        }
    }
}
