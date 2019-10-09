namespace Game
{
    /// Auteur: Zacharie Lavigne
    public class Axe : Weapon
    {
        private static Axe basicWeapon = new Axe();
        
        public const WeaponType ADVANTAGE = WeaponType.Spear;
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

        public Axe(UnitStats weaponStats = null) : base(weaponStats, WeaponType.Axe, ADVANTAGE)
        {
            if (weaponStats == null)
            {
                this.weaponStats = new UnitStats(AXE_MAX_HEALTH_POINTS, AXE_MOVE_SPEED, AXE_ATTACK_STRENGTH, AXE_HIT_RATE, AXE_CRIT_RATE);
            }
        }
    }
}