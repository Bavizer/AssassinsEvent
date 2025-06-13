using AssassinsEvent.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AssassinsEvent;

internal sealed class Config
{
    internal IReadOnlyCollection<ItemType>? AssassinWeapons
    {
        get;
        private set => 
            field = value ?? throw new ArgumentNullException(nameof(AssassinWeapons));
    }

    [Description("Damage immunity duration applied to assassin on spawn.")]
    public float SpawnProtectionDuration
    {
        get;
        set => field = value > 0 ? value : 0;
    } = 3f;

    [Description("Duration after which assassin will get his weapons back.")]
    public float PunishmentDuration
    {
        get;
        set => field = value > 0 ? value : 0;
    } = 30f;

    [Description("Disable round lock after event ending? (true/false)")]
    public bool DisableRoundLockOnEnd { get; set; } = false;

    [Description("These items will be granted to assassins on spawn.")]
    public List<ItemType> AssassinItems
    {
        get;
        set
        {
            field = value?.ToList() ?? throw new ArgumentNullException(nameof(AssassinItems));
            AssassinWeapons = AssassinItems.Where(i => i.IsWeapon()).ToList().AsReadOnly();
        }
    } = [ 
            ItemType.KeycardO5, 
            ItemType.GunCOM18, 
            ItemType.Medkit, 
            ItemType.Painkillers 
        ];

    public EventStrings EventStrings { get; set; } = new();
}

internal struct EventStrings
{
    public string Target { get; set; } =
        "<b>Target: <color=red>{target}</color>\n<color=orange>Distance: </color>{distance} m.</b>";

    public string AwaitingForTarget { get; set; } = 
        "<color=orange><b>Wait for a new target...</b></color>";

    public string Punishment { get; set; } =
        "<color=red><b>ATTACK ONLY YOUR TARGET AND ASSASSIN</b></color>\nYou will get your weapons back in <color=orange>{duration} s.</color>";

    public string EventEnd { get; set; } =
        "<b>Event <color=orange>\"Assassins\"</color> has been ended\nWinner is <color=red>{winner}</color></b>";

    public EventStrings() { }
}
