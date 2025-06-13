using AssassinsEvent.Extensions;
using AssassinsEvent.Tools;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using System;
using System.Linq;
using PlayerRoles;
using UnityEngine;

namespace AssassinsEvent.Core;

public class Assassin
{
    public readonly Player Player;

    public Assassin? Target { get; internal set; }

    internal Assassin(Player player) =>
        Player = player ?? throw new ArgumentNullException(nameof(player));

    public static bool TryGet(Player player, out Assassin? assassin)
    {
        if (player is null)
            throw new ArgumentNullException(nameof(player));

        assassin = AssassinsEvent.Instance.Assassins.FirstOrDefault(a => a.Player == player);
        return assassin != null;
    }

    public static bool IsTargetFor(Player assassin, Player target)
    {
        if (assassin is null)
            throw new ArgumentNullException(nameof(assassin));

        if (target is null)
            throw new ArgumentNullException(nameof(target));

        return GetPlayerTarget(assassin)?.Player == target;
    }

    public static Assassin? GetPlayerTarget(Player player)
    {
        TryGet(player, out var asn);
        return asn?.Target;
    }

    public static Assassin? GetPlayerAssassin(Player player)
    {
        TryGet(player, out var asn);
        return asn is null ? null : AssassinsEvent.Instance.Assassins.FirstOrDefault(a => a.Target == asn);
    }

    internal void Spawn(Vector3 position)
    {
        var config = AssassinsEvent.Instance.Config;

        Player.SetRole(AssassinsEvent.PlayersRoleType, flags: RoleSpawnFlags.None);
        Player.AddItems(config.AssassinItems);
        Player.Position = position;

        if (config.SpawnProtectionDuration > 0f)
            Player.EnableEffect<SpawnProtected>(duration: config.SpawnProtectionDuration);
    }

    internal void ShowAssassinBroadcast(ushort duration) =>
        Player.SendBroadcast(EventStringBuilder.GetAssassinString(this), duration, shouldClearPrevious: true);

    public static implicit operator Player(Assassin assassin) => assassin.Player;

    public static bool operator ==(Assassin? left, Assassin? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Assassin? left, Assassin? right) => !(left == right);

    public sealed override bool Equals(object? obj) => obj is Assassin asn && asn.Player == this.Player;

    public sealed override int GetHashCode() => Player.GetHashCode();

    public override string ToString() => Player.Nickname;
}
