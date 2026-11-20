using System;

namespace Reforged.Dynamics.Crm.Sdk.Plugins
{
    /// <summary>
    /// Extension methods for <see cref="ILocalPluginContext"/>.
    /// </summary>
    public static class LocalPluginContextExtensions
    {
        /// <summary>
        /// Returns true when the current execution runs in the given stage for the given message.
        /// </summary>
        public static bool Is(this ILocalPluginContext context, PluginStage stage, string messageName)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executionContext = context.PluginExecutionContext;
            return executionContext.Stage == (int)stage
                && string.Equals(executionContext.MessageName, messageName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
