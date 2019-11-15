namespace Game
{
    /// Author: Jérémie Bertrand, Zacharie Lavigne
    public enum WeaponType
    {
        None,
        Sword,
        Spear,
        Axe,
        HealingStaff
    }

    public static class WeaponTypeExt
    {
        public const WeaponType SWORD_ADVANTAGE = WeaponType.Axe;
        public const WeaponType AXE_ADVANTAGE = WeaponType.Spear;
        public const WeaponType SPEAR_ADVANTAGE = WeaponType.Sword;
        
        public static WeaponType GetAdvantageByWeaponType(this WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Axe:
                    return AXE_ADVANTAGE;
                case WeaponType.Spear:
                    return SPEAR_ADVANTAGE;
                case WeaponType.Sword:
                    return SWORD_ADVANTAGE;
                default:
                    return WeaponType.None;
            }
        }
    }
}