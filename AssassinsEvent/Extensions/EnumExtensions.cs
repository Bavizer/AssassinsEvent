namespace AssassinsEvent.Extensions;

public static class EnumExtensions
{
    public static bool IsWeapon(this ItemType itemType)
    {
        switch (itemType) 
        {
            case ItemType.GunA7:
            case ItemType.GunAK:
            case ItemType.GunCOM15:
            case ItemType.GunCOM18:
            case ItemType.GunCom45:
            case ItemType.GunCrossvec:
            case ItemType.GunE11SR:
            case ItemType.GunFRMG0:
            case ItemType.GunFSP9:
            case ItemType.GunLogicer:
            case ItemType.GunRevolver:
            case ItemType.GunSCP127:
            case ItemType.GunShotgun:
            case ItemType.GrenadeHE:
                return true;

            default:
                return false;
        }
    }

    public static ItemType GetAmmoType(this ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.GunA7:
            case ItemType.GunAK:
            case ItemType.GunLogicer:
                return ItemType.Ammo762x39;

            case ItemType.GunCOM15:
            case ItemType.GunCOM18:
            case ItemType.GunCom45:
            case ItemType.GunCrossvec:
            case ItemType.GunFSP9:
                return ItemType.Ammo9x19;

            case ItemType.GunE11SR:
            case ItemType.GunFRMG0:
                return ItemType.Ammo556x45;

            case ItemType.GunRevolver:
                return ItemType.Ammo44cal;

            case ItemType.GunShotgun:
                return ItemType.Ammo12gauge;

            default:
                return ItemType.None;
        }
    }
}
