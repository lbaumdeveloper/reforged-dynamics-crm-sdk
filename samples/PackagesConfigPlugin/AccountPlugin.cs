using Microsoft.Xrm.Sdk;
using Reforged.Dynamics.Crm.Sdk.Plugins;

[CrmPluginRegistration(
    MessageNameEnum.Create, "account", StageEnum.PreOperation, ExecutionModeEnum.Synchronous,
    "", "AccountPlugin: Create of account", 1, IsolationModeEnum.Sandbox)]
public class AccountPlugin : PluginBase
{
    public AccountPlugin(string unsecureConfiguration, string secureConfiguration)
        : base(unsecureConfiguration, secureConfiguration)
    {
    }

    protected override void ExecutePlugin(ILocalPluginContext context)
    {
        if (!context.Is(PluginStage.PreOperation, PluginMessage.Create))
        {
            return;
        }

        var target = (Entity)context.PluginExecutionContext.InputParameters["Target"];
        context.Trace("Creating account {0}", target.GetAttributeValue<string>("name"));
    }
}
