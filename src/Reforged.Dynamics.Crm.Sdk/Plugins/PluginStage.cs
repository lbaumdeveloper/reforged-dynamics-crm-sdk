namespace Reforged.Dynamics.Crm.Sdk.Plugins
{
    /// <summary>
    /// Pipeline stages a plug-in step can be registered on.
    /// </summary>
    public enum PluginStage
    {
        PreValidation = 10,
        PreOperation = 20,
        MainOperation = 30,
        PostOperation = 40
    }
}
