namespace Game
{
    /// Auteur: Zacharie Lavigne
    public class Spear : Weapon
    {
        private static Spear basicWeapon = new Spear();
        public static Spear BasicWeapon
        {
            get
            {
                if (basicWeapon == null)
                {
                    return new Spear();
                }
                return basicWeapon;
            }
        }

        public Spear(UnitStats weaponStats = null) : base(weaponStats, WeaponType.Spear, WeaponType.Sword)
        {
            if (weaponStats == null)
            {
                this.weaponStats = new UnitStats(0, 0, 2, 0.95f, 0.15f);
            }
        }
    }
}