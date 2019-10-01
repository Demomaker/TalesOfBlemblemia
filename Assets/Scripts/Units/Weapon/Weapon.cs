namespace Game
{
    /// <summary>
    /// Concept d'arme, qui a un bonus à l'attaque, une chance de toucher et une chance de coup critique, un type et un type contre qui elle a un avantage
    /// Chaque unité à une arme
    /// Auteur: Zacharie Lavigne
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