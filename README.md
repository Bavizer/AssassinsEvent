# ℹ Assassins Event
**A plugin for SCP: Secret Laboratory, that adds a new event (game mode):**
- 🎲 Players randomly spawn across Heavy Containment and Entrance Zones
- 🤵 Each player has a unique target and sees a distance to it.
- 🔪 Player can damage only his target and assassin. If player tries to attack other players, his weapons will be removed for some time.
- ⏱ The event ends when one or no assassins remain.

# ⭐ Features
- ⚙ **Configurable**: change some event settings in the config file.
- 🤖 **Automatic**: you just need to start the event. That's all.
- ⌨ **Console commands**: you can start and end the event using Remote Admin Console commands.
- 🌐 **API**: access public types and their members in your code.

> [!NOTE]
> **You need `RoundEvents` permission to execute event commnads.**

# ✨ Starting and Ending event
To start or end the event you can use one of the methods below:
- Using commands in **Remote Admin Console**:
   - `start_asns` command to start the event.
   - `end_asns` command to end the event.

> [!NOTE]
> **If you start the event using command, only players whose role is <ins>Class-D</ins> will be selected to be assassins.**

- Using **API**:
  - Call `AssassinsEvent.Instance.StartEvent(IEnumerable<Player> players)` method to start the event.
  - Call `AssassinsEvent.Instance.EndEvent()` method to end the event.

# 📁Installation and Configuration
- [Installation Guide](https://github.com/northwood-studios/LabAPI/wiki/Installing-Plugins)  
- [Configuration Guide](https://github.com/northwood-studios/LabAPI/wiki/Configuring-Plugins)

# 🖼 Gallery
https://github.com/user-attachments/assets/02ee0c1f-1568-40c2-85cf-dcfc42965b6c
