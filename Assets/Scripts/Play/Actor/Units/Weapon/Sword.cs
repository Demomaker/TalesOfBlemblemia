namespace Game
{
    /// Auteur: Zacharie Lavigne
    public class Sword : Weapon
    {
        private static Sword basicWeapon = new Sword();
        public const WeaponType ADVANTAGE = WeaponType.Axe;

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

        public Sword(UnitStats weaponStats = null) : base(weaponStats, WeaponType.Sword, ADVANTAGE)
        {
            if (weaponStats == null)
            {
                this.weaponStats = new UnitStats(SWORD_MAX_HEALTH_POINTS, SWORD_MOVE_SPEED, SWORD_ATTACK_STRENGTH, SWORD_HIT_RATE, SWORD_CRIT_RATE);
            }
        }
    }
}