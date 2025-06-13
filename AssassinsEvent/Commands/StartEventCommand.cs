using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using System;
using System.Linq;

namespace AssassinsEvent.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class StartEventCommand : ICommand
{
    public string Command => "start_asns";

    public string[] Aliases => [];

    public string Description => "Start Assassins Event";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
            return false;

        try
        {
            var members = Player.ReadyList.Where(p => p.Role == Core.AssassinsEvent.PlayersRoleType);
            Core.AssassinsEvent.Instance.StartEvent(members);
        }
        catch (InvalidOperationException ex)
        {
            response = ex.Message;
            return false;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            response = "An error occured when starting event.";
            return false;
        }

        Logger.Info($"{sender.LogName} started the event.");
        response = "Event has been started.";
        return true;
    }
}
