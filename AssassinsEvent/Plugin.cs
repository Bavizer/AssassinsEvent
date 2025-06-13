using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;

namespace AssassinsEvent;

internal sealed class Plugin : Plugin<Config>
{
#nullable disable
    public static Plugin Instance { get; private set; }
#nullable restore

    public override string Name => "Assassins Event";

    public override string Description => "A Game Mode in which each player has a unique target and sees the distance to it.";

    public override string Author => "Bavizer";

    public override Version Version => new(1, 0, 0, 0);

    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    public override void Enable() => Instance = this;

    public override void Disable()
    {
        var @event = Core.AssassinsEvent.Instance;

        if (@event.IsActive)
            @event.EndEvent();
    }

    public void LoadAndSaveConfig()
    {
        LoadConfigs();
        SaveConfig();
    }
}
