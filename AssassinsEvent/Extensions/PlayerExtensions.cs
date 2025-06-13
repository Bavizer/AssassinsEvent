using InventorySystem.Items;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Features.Wrappers;
using System.Collections.Generic;

namespace AssassinsEvent.Extensions;

public static class PlayerExtensions
{
    public static void SetMaxAmmoForEachFirearm(this Player player)
    {
        foreach (var item in player.Items)
        {
            if (item is not FirearmItem firearm)
                continue;

            int maxAmmo = firearm.Base.GetTotalMaxAmmo();
            player.SetAmmo(firearm.Type.GetAmmoType(), (ushort)maxAmmo);
        }
    }

    public static void AddItems(this Player player, IEnumerable<ItemType>? items, ItemAddReason addReason = ItemAddReason.AdminCommand) =>
        items?.ForEach(i => player.AddItem(i, addReason));
}
