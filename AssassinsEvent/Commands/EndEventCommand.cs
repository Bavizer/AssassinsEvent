using CommandSystem;
using LabApi.Features.Console;
using System;

namespace AssassinsEvent.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class EndEventCommand : ICommand
{
    public string Command => "end_asns";

    public string[] Aliases => [];

    public string Description => "End Assassins Event";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
            return false;

        try
        {
            Core.AssassinsEvent.Instance.EndEvent();
        }
        catch (InvalidOperationException ex)
        {
            response = ex.Message;
            return false;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            response = "An error occured when ending event.";
            return false;
        }

        Logger.Info($"{sender.LogName} ended the event.");
        response = "Event has been ended.";
        return true;
    }
}
