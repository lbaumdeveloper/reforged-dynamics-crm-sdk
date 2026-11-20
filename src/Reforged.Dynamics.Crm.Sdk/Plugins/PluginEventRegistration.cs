using System;
using Microsoft.Xrm.Sdk;

namespace Reforged.Dynamics.Crm.Sdk.Plugins
{
    /// <summary>
    /// Maps a stage, message and optional entity to the handler that runs for it.
    /// </summary>
    public sealed class PluginEventRegistration
    {
        public PluginEventRegistration(PluginStage stage, string messageName, string entityLogicalName, Action<ILocalPluginContext> handler)
        {
            if (string.IsNullOrWhiteSpace(messageName))
            {
                throw new ArgumentException("A message name is required.", nameof(messageName));
            }

            Stage = stage;
            MessageName = messageName;
            EntityLogicalName = entityLogicalName;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public PluginStage Stage { get; }

        public string MessageName { get; }

        /// <summary>
        /// Logical name of the entity, or null or empty to match any entity.
        /// </summary>
        public string EntityLogicalName { get; }

        public Action<ILocalPluginContext> Handler { get; }

        /// <summary>
        /// Returns true when the registration applies to the given execution context.
        /// </summary>
        public bool Matches(IPluginExecutionContext context)
        {
            if (context == null)
            {
                return false;
            }

            return context.Stage == (int)Stage
                && string.Equals(context.MessageName, MessageName, StringComparison.OrdinalIgnoreCase)
                && (string.IsNullOrEmpty(EntityLogicalName)
                    || string.Equals(context.PrimaryEntityName, EntityLogicalName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
