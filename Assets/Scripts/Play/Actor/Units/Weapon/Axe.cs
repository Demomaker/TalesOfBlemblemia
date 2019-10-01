namespace Game
{
    /// Auteur: Zacharie Lavigne
    public class Axe : Weapon
    {
        private static Axe basicWeapon = new Axe();
        public static Axe BasicWeapon
        {
            get
            {
                if (basicWeapon == null)
                {
                    return new Axe();
                }
                return basicWeapon;
            }
        }

        public Axe(UnitStats weaponStats = null) : base(weaponStats, WeaponType.Axe, WeaponType.Spear)
        {
            if (weaponStats == null)
            {
                this.weaponStats = new UnitStats(0, 0, 3, 0.67f, 0.15f);
            }
        }
    }
}