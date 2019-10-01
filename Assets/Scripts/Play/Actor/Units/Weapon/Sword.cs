namespace Game
{
    /// Auteur: Zacharie Lavigne
    public class Sword : Weapon
    {
        private static Sword basicWeapon = new Sword();
        public static Sword BasicWeapon
        {
            get
            {
                if (basicWeapon == null)
                {
                    return new Sword();
                }
                return basicWeapon;
            }
        }

        public Sword(UnitStats weaponStats = null) : base(weaponStats, WeaponType.Sword, WeaponType.Axe)
        {
            if (weaponStats == null)
            {
                this.weaponStats = new UnitStats(0, 0, 2, 0.80f, 0.35f);
            }
        }
    }
}