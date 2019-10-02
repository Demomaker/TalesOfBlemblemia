namespace Game
{
    /// <summary>
    /// A weapon to be held by a unit. A weapon has a bonus to its attack, a hit rate, a critical hit rate, a weapon type and an advantage
    /// Author: Zacharie Lavigne
    /// </summary>
    public abstract class Weapon
    {
        protected UnitStats weaponStats;
        /// <summary>
        /// Les bonus que l'arme va offrir à une unité
        /// </summary>
        public UnitStats WeaponStats => weaponStats;

        private WeaponType weaponType;
        /// <summary>
        /// Le type de l'arme
        /// </summary>
        public WeaponType WeaponType => weaponType;
        
        private WeaponType advantage;
        /// <summary>
        /// Le type d'arme contre lequelle l'arme a un avantage
        /// </summary>
        public WeaponType Advantage => advantage;

        public Weapon(UnitStats weaponStats, WeaponType weaponType, WeaponType advantage)
        {
            this.weaponStats = weaponStats;
            this.weaponType = weaponType;
            this.advantage = advantage;
        }
    }
}