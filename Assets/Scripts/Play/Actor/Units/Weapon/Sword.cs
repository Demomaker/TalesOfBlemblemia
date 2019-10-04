namespace Game
{
    /// Auteur: Zacharie Lavigne
    public class Sword : Weapon
    {
        protected const float SWORD_HIT_RATE = 0.8f;
        protected const float SWORD_CRIT_RATE = 0.3f;
        protected const int SWORD_ATTACK_STRENGTH = 2;
        protected const int SWORD_MOVE_SPEED = 0;
        protected const int SWORD_MAX_HEALTH_POINTS = 0;
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
                this.weaponStats = new UnitStats(SWORD_MAX_HEALTH_POINTS, SWORD_MOVE_SPEED, SWORD_ATTACK_STRENGTH, SWORD_HIT_RATE, SWORD_CRIT_RATE);
            }
        }
    }
}