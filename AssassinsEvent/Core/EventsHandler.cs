using AssassinsEvent.Extensions;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MEC;
using System.Linq;
using AssassinsEvent.Tools;

namespace AssassinsEvent.Core;

internal class EventsHandler : CustomEventsHandler
{
    private static AssassinsEvent Event => AssassinsEvent.Instance;

    public override void OnServerRoundRestarted() => Event.EndEvent();

    public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
    {
        if (ev.Player.Role != AssassinsEvent.PlayersRoleType)
            UnregisterIfPresent(ev.Player);
    }

    public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
    {
        if (ev.Attacker is null || ev.Player == ev.Attacker)
            return;

        if (Assassin.TryGet(ev.Player, out _) && Assassin.TryGet(ev.Attacker, out _))
            HandleAssassinsAttack(ev);
        else
            ev.IsAllowed = false;
    }

    public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
    {
        if (ev.Pickup is FirearmPickup)
            ev.IsAllowed = false;
    }

    public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev)
    {
        if (ev.Item is FirearmItem)
            ev.IsAllowed = false;
    }

    public override void OnServerWaveRespawning(WaveRespawningEventArgs ev) => ev.IsAllowed = false;

    public override void OnServerWaveTeamSelecting(WaveTeamSelectingEventArgs ev) => ev.IsAllowed = false;

    public override void OnPlayerDroppingAmmo(PlayerDroppingAmmoEventArgs ev) => ev.IsAllowed = false;

    public override void OnPlayerLeft(PlayerLeftEventArgs ev) => UnregisterIfPresent(ev.Player);

    private void UnregisterIfPresent(Player player)
    {
        if (Assassin.TryGet(player, out var assassin))
            Event.UnregisterAssassin(assassin!);
    }

    private void HandleAssassinsAttack(PlayerHurtingEventArgs ev)
    {
        if (Assassin.IsTargetFor(ev.Player, ev.Attacker!) || Assassin.IsTargetFor(ev.Attacker!, ev.Player))
            return;

        ev.IsAllowed = false;

        foreach (var item in ev.Attacker!.Items.ToArray())
        {
            if (item is FirearmItem firearm)
                firearm.Base.ServerDropItem(false);
        }

        float duration = Event.Config.PunishmentDuration;
        string hint = EventStringBuilder.GetPunishmentString(duration);
        ev.Attacker.SendHint(hint, 5);

        Timing.CallDelayed(duration, () => OnPunishmentEnd(ev.Attacker));
    }

    private void OnPunishmentEnd(Player player)
    {
        if (player.IsOffline || !player.IsAlive)
            return;

        if (!Assassin.TryGet(player, out _))
            return;

        player.AddItems(Event.Config.AssassinWeapons);
        player.SendHint(string.Empty, 0);
    }
}
