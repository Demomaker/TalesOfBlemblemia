namespace Game
{
    /// <summary>
    /// A weapon to be held by a unit. A weapon has a bonus to its attack, a hit rate, a critical hit rate, a weapon type and an advantage
    /// Author: Zacharie Lavigne
    /// </summary>
    public abstract class Weapon
    {
        protected const float AXE_HIT_RATE = 0.67f;
        protected const float AXE_CRIT_RATE = 0.15f;
        protected const int AXE_ATTACK_STRENGTH = 3;
        protected const int AXE_MOVE_SPEED = 0;
        protected const int AXE_MAX_HEALTH_POINTS = 0;
        protected const float SPEAR_HIT_RATE = 0.95f;
        protected const float SPEAR_CRIT_RATE = 0.15f;
        protected const int SPEAR_ATTACK_STRENGTH = 2;
        protected const int SPEAR_MOVE_SPEED = 0;
        protected const int SPEAR_MAX_HEALTH_POINTS = 0;
        protected const float SWORD_HIT_RATE = 0.8f;
        protected const float SWORD_CRIT_RATE = 0.3f;
        protected const int SWORD_ATTACK_STRENGTH = 2;
        protected const int SWORD_MOVE_SPEED = 0;
        protected const int SWORD_MAX_HEALTH_POINTS = 0;
        protected const WeaponType SWORD_ADVANTAGE = WeaponType.Axe;
        protected const WeaponType AXE_ADVANTAGE = WeaponType.Spear;
        protected const WeaponType SPEAR_ADVANTAGE = WeaponType.Sword;
        
        protected UnitStats weaponStats;
        /// <summary>
        /// Les bonus que l'arme va offrir à une unité
        /// </summary>
        public UnitStats WeaponStats => weaponStats;

        private readonly WeaponType weaponType;
        /// <summary>
        /// Le type de l'arme
        /// </summary>
        public WeaponType WeaponType => weaponType;
        
        private readonly WeaponType advantage;
        
        /// <summary>
        /// Le type d'arme contre lequelle l'arme a un avantage
        /// </summary>
        public WeaponType Advantage => Harmony.Finder.LevelController.RevertWeaponTriangle ? GetAdvantageByWeaponType(advantage) : advantage;

        public Weapon(UnitStats weaponStats, WeaponType weaponType, WeaponType advantage)
        {
            this.weaponStats = weaponStats;
            this.weaponType = weaponType;
            this.advantage = advantage;
        }

        private static WeaponType GetAdvantageByWeaponType(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Axe:
                    return Axe.AXE_ADVANTAGE;
                case WeaponType.Spear:
                    return Spear.SPEAR_ADVANTAGE;
                case WeaponType.Sword:
                    return Sword.SWORD_ADVANTAGE;
                default:
                    return WeaponType.None;
            }
        }
    }
}