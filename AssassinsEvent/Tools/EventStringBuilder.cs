using AssassinsEvent.Core;
using LabApi.Features.Wrappers;
using System;
using UnityEngine;

namespace AssassinsEvent.Tools;

internal static class EventStringBuilder
{
    private static EventStrings EventStrings => Core.AssassinsEvent.Instance.Config.EventStrings;

    internal static string GetAssassinString(Assassin assassin)
    {
        if (assassin is null)
            throw new ArgumentNullException(nameof(assassin));

        string message;

        if (assassin.Target is not null)
        {
            int distance = (int)Vector3.Distance(assassin.Player.Position, assassin.Target.Player.Position);
            var dist = distance.ToString();
            var target = assassin.Target.ToString();
            message = EventStrings.Target.Replace("{target}", target).Replace("{distance}", dist);
        }
        else
        {
            message = EventStrings.AwaitingForTarget;
        }

        return message;
    }

    internal static string GetPunishmentString(float duration) =>
        EventStrings.Punishment.Replace("{duration}", duration.ToString());

    internal static string GetEventEndString(Player? winner)
    {
        var winnerString = winner is null ? "undefined" : winner.Nickname;
        return EventStrings.EventEnd.Replace("{winner}", winnerString);
    }
}
