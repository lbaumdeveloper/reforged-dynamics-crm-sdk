using System;
using Microsoft.Xrm.Sdk;

namespace Reforged.Dynamics.Crm.Sdk.Plugins
{
    /// <summary>
    /// Extension methods for <see cref="ILocalPluginContext"/>.
    /// </summary>
    public static class LocalPluginContextExtensions
    {
        private const string TargetParameter = "Target";

        /// <summary>
        /// Returns true when the current execution runs in the given stage for the given message.
        /// </summary>
        public static bool Is(this ILocalPluginContext context, PluginStage stage, string messageName)
        {
            var executionContext = GetExecutionContext(context);
            return executionContext.Stage == (int)stage
                && string.Equals(executionContext.MessageName, messageName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the Target input parameter as an entity, or null when it is missing or not an entity.
        /// </summary>
        public static Entity GetTarget(this ILocalPluginContext context)
        {
            object target;
            return GetExecutionContext(context).InputParameters.TryGetValue(TargetParameter, out target)
                ? target as Entity
                : null;
        }

        /// <summary>
        /// Returns the pre-entity image with the given name, or null when it is not registered.
        /// </summary>
        public static Entity GetPreImage(this ILocalPluginContext context, string name = PluginImage.PreImage)
        {
            Entity image;
            return context.TryGetPreImage(out image, name) ? image : null;
        }

        /// <summary>
        /// Gets the pre-entity image with the given name. Returns false when it is not registered.
        /// </summary>
        public static bool TryGetPreImage(this ILocalPluginContext context, out Entity image, string name = PluginImage.PreImage)
        {
            return TryGetImage(GetExecutionContext(context).PreEntityImages, name, out image);
        }

        /// <summary>
        /// Returns the post-entity image with the given name, or null when it is not registered.
        /// </summary>
        public static Entity GetPostImage(this ILocalPluginContext context, string name = PluginImage.PostImage)
        {
            Entity image;
            return context.TryGetPostImage(out image, name) ? image : null;
        }

        /// <summary>
        /// Gets the post-entity image with the given name. Returns false when it is not registered.
        /// </summary>
        public static bool TryGetPostImage(this ILocalPluginContext context, out Entity image, string name = PluginImage.PostImage)
        {
            return TryGetImage(GetExecutionContext(context).PostEntityImages, name, out image);
        }

        private static bool TryGetImage(EntityImageCollection images, string name, out Entity image)
        {
            image = null;
            return images != null && images.TryGetValue(name, out image) && image != null;
        }

        private static IPluginExecutionContext GetExecutionContext(ILocalPluginContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.PluginExecutionContext;
        }
    }
}
