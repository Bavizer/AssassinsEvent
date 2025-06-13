using AssassinsEvent.Extensions;
using AssassinsEvent.Tools;
using GameCore;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace AssassinsEvent.Core;

public sealed class AssassinsEvent
{
    #region Fields

    private readonly HashSet<Assassin> _assassins = new(Server.MaxPlayers);
    private readonly HashSet<Assassin> _freeTargets = new(Server.MaxPlayers);

    private readonly List<Room> _spawnRooms = new(Room.List.Count);

    private readonly CustomEventsHandler _eventsHandler = new EventsHandler();

    private CoroutineHandle _logicUpdateCoroutine;
    private CoroutineHandle _broadcastCoroutine;

    private bool _disableFriendlyFireOnEnd;
    private float _friendlyFireMultiplierOnEnd;

    public const ushort MinimumPlayersRequired = 2;
    public const RoleTypeId PlayersRoleType = RoleTypeId.ClassD;

    #endregion

    #region Properties

    internal Config Config => Plugin.Instance.Config!;

    public static AssassinsEvent Instance => field ??= new AssassinsEvent();

    public bool IsActive => _logicUpdateCoroutine.IsRunning || _broadcastCoroutine.IsRunning;

    public bool AreEndConditionsCompleted => _assassins.Count < MinimumPlayersRequired;

    public IReadOnlyList<Assassin> Assassins => _assassins.ToArray();

    #endregion

    #region Methods

    private AssassinsEvent() { }

    public void StartEvent(IEnumerable<Player> players)
    {
        var players1 = players?.ToArray() ?? throw new ArgumentNullException(nameof(players));
        var eventName = nameof(AssassinsEvent);

        if (IsActive)
            throw new InvalidOperationException($"Cannot start {eventName}: it's already active.");

        if (players1.Contains(null))
            throw new ArgumentException("Players list contains null element(-s).", nameof(players));

        if (players1.Length < MinimumPlayersRequired)
            throw new InvalidOperationException($"Minimum {MinimumPlayersRequired} players required to start {eventName}.");

        if (!Round.IsRoundInProgress)
            throw new InvalidOperationException($"Cannot start {eventName}: round has to be started.");

        try
        {
            Initialize();
            InitiallySetAssassins(players1);

            ApplyEventSettingsForServerAndRound(true);

            _logicUpdateCoroutine = Timing.RunCoroutine(Update());
            _broadcastCoroutine = Timing.RunCoroutine(SendBroadcast());
        }
        catch
        {
            Clear();
            throw;
        }
    }

    public void EndEvent()
    {
        if (!IsActive)
            throw new InvalidOperationException($"Cannot end {nameof(AssassinsEvent)}: it's not active.");

        try
        {
            OnEnd();
        }
        finally
        {
            Clear();
            Logger.Info("Event has been ended.");
        }
    }

    internal void RegisterAssassin(Assassin assassin)
    {
        if (assassin is null)
            throw new ArgumentNullException(nameof(assassin));

        if (!_assassins.Add(assassin))
            return;

        assassin.Spawn(_spawnRooms.RandomValue()!.Position + Vector3.up);
    }

    internal void UnregisterAssassin(Assassin assassin)
    {
        if (assassin is null)
            throw new ArgumentNullException(nameof(assassin));

        _assassins.Remove(assassin);
        _freeTargets.Remove(assassin);
    }

    private void Initialize()
    {
        Plugin.Instance.LoadAndSaveConfig();

        var spawnRooms = Room.List.Where(r => r.Zone is FacilityZone.HeavyContainment or FacilityZone.Entrance
                                            && r.Name is not RoomName.HczTesla and not RoomName.HczTestroom
                                                and not RoomName.EzCollapsedTunnel and not RoomName.EzEvacShelter
                                            && r.GameObject.name != "HCZ_Crossroom_Water(Clone)");
        _spawnRooms.AddRange(spawnRooms);

        _disableFriendlyFireOnEnd = ConfigFile.ServerConfig.GetBool("friendly_fire");
        _friendlyFireMultiplierOnEnd = ConfigFile.ServerConfig.GetFloat("friendly_fire_multiplier");

        CustomHandlersManager.RegisterEventsHandler(_eventsHandler);
    }

    private void Clear()
    {
        CustomHandlersManager.UnregisterEventsHandler(_eventsHandler);

        Timing.KillCoroutines(_logicUpdateCoroutine, _broadcastCoroutine);

        _assassins.Clear();
        _freeTargets.Clear();

        _spawnRooms.Clear();

        ApplyEventSettingsForServerAndRound(false);
    }

    private void ApplyEventSettingsForServerAndRound(bool isEventActive)
    {
        Round.IsLocked = isEventActive || !Config.DisableRoundLockOnEnd;
        Server.FriendlyFire = isEventActive || !_disableFriendlyFireOnEnd;
        AttackerDamageHandler._ffMultiplier = isEventActive ? 1f : _friendlyFireMultiplierOnEnd;

        if (isEventActive)
            Elevator.LockAll();
        else
            Elevator.UnlockAll();

        Pickup.List.ForEach(p => p.Destroy());
    }

    private IEnumerator<float> Update()
    {
        yield return Timing.WaitForOneFrame;

        while (!AreEndConditionsCompleted)
        {
            UpdateFreeTargetsSet();
            UpdateAssassins();
            yield return Timing.WaitForSeconds(1f);
        }

        EndEvent();
    }

    private void UpdateFreeTargetsSet()
    {
        foreach (var asn in _assassins)
        {
            if (_assassins.All(a => a.Target != asn))
                _freeTargets.Add(asn);
        }
    }

    private void UpdateAssassins()
    {
        foreach (var asn in _assassins)
        {
            asn.Player.SetMaxAmmoForEachFirearm();

            if (asn.Target is not null && _assassins.Contains(asn.Target))
                continue;

            var target = _freeTargets.FirstOrDefault(CanBeTarget);
            asn.Target = target;

            if (target is not null)
                _freeTargets.Remove(target);

            continue;
            bool CanBeTarget(Assassin a) => a != asn && (_assassins.Count == 2 || a.Target != asn);
        }
    }

    private IEnumerator<float> SendBroadcast()
    {
        yield return Timing.WaitForOneFrame;

        while (!AreEndConditionsCompleted)
        {
            _assassins.ForEach(asn => asn.ShowAssassinBroadcast(1));
            yield return Timing.WaitForSeconds(0.3f);
        }
    }

    private void InitiallySetAssassins(IEnumerable<Player> players)
    {
        players.ForEach(p => RegisterAssassin(new Assassin(p)));

        var assassins = _assassins.ToArray();
        int lastIndex = assassins.Length - 1;

        for (int i = 0; i <= lastIndex; i++)
            assassins[i].Target = assassins[i == lastIndex ? 0 : i + 1];
    }

    private void OnEnd()
    {
       Player? winner = _assassins.Count > 1 ? null : _assassins.SingleOrDefault()?.Player;

        foreach (var player in Player.ReadyList)
        {
            if (player.Role != RoleTypeId.Overwatch)
                player.SetRole(RoleTypeId.Spectator);
        }

        Logger.Info($"Winner is {(winner is null ? "undefined" : winner.Nickname)}");
        Server.SendBroadcast(EventStringBuilder.GetEventEndString(winner), 10, shouldClearPrevious: true);
    }

    #endregion
}
