using Reforged.Dynamics.Crm.Sdk.Plugins;

[CrmPluginRegistration(
    MessageNameEnum.Create, "account", StageEnum.PreOperation, ExecutionModeEnum.Synchronous,
    "", "AccountPlugin: Create of account", 1, IsolationModeEnum.Sandbox)]
[CrmPluginRegistration(
    MessageNameEnum.Update, "account", StageEnum.PreOperation, ExecutionModeEnum.Synchronous,
    filteringAttributes: "name",
    stepName: "AccountPlugin: Update of account",
    executionOrder: 1,
    isolationModel: IsolationModeEnum.Sandbox,
    Image1Type = ImageTypeEnum.PreImage,
    Image1Name = PluginImage.PreImage,
    Image1Attributes = "name")]
public class AccountPlugin : PluginBase
{
    public AccountPlugin(string unsecureConfiguration, string secureConfiguration)
        : base(unsecureConfiguration, secureConfiguration)
    {
    }

    protected override void ExecutePlugin(ILocalPluginContext context)
    {
        var target = context.GetRequiredTarget();

        if (context.Is(PluginStage.PreOperation, PluginMessage.Create))
        {
            context.Trace("Creating account {0}", target.GetAttributeValue<string>("name"));
        }
        else if (context.Is(PluginStage.PreOperation, PluginMessage.Update))
        {
            context.Trace(
                "Renaming account {0} to {1}",
                context.GetRequiredPreImage().GetAttributeValue<string>("name"),
                target.GetAttributeValue<string>("name"));
        }
    }
}
