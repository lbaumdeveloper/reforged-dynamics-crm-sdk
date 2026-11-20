using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Reforged.Dynamics.Crm.Sdk.Plugins
{
    /// <summary>
    /// Base class for plug-ins. Derived classes register handlers in their constructor with
    /// <see cref="RegisterEvent"/>; <see cref="Execute"/> runs every handler matching the current context.
    /// The platform caches plug-in instances and may run them on several threads at once,
    /// so derived classes must not keep per-execution state in fields.
    /// </summary>
    public abstract class PluginBase : IPlugin
    {
        private readonly List<PluginEventRegistration> registrations = new List<PluginEventRegistration>();

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

        protected IReadOnlyList<PluginEventRegistration> Registrations => registrations;

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
                var handlers = registrations.Where(r => r.Matches(executionContext)).ToList();

                if (handlers.Count == 0)
                {
                    localContext.Trace("No handler registered for this context.");
                    return;
                }

                foreach (var registration in handlers)
                {
                    registration.Handler(localContext);
                }
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
        /// Registers a handler for a stage and message, optionally limited to one entity.
        /// Call only from the constructor of the derived class.
        /// </summary>
        protected void RegisterEvent(PluginStage stage, string messageName, string entityLogicalName, Action<ILocalPluginContext> handler)
        {
            registrations.Add(new PluginEventRegistration(stage, messageName, entityLogicalName, handler));
        }

        /// <summary>
        /// Creates the context passed to handlers. Override to supply a custom context.
        /// </summary>
        protected virtual ILocalPluginContext CreateLocalPluginContext(IServiceProvider serviceProvider)
        {
            return new LocalPluginContext(serviceProvider);
        }
    }
}
