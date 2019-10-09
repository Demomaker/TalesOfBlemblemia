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

        public Spear(UnitStats weaponStats = null) : base(weaponStats, WeaponType.Spear, SPEAR_ADVANTAGE)
        {
            if (weaponStats == null)
            {
                this.weaponStats = new UnitStats(SPEAR_MAX_HEALTH_POINTS, SPEAR_MOVE_SPEED, SPEAR_ATTACK_STRENGTH, SPEAR_HIT_RATE, SPEAR_CRIT_RATE);
            }
        }
    }
}