namespace Game
{
    /// <summary>
    /// Propriétés statistiques qu'une unité peut avoir
    /// Auteur: Zacharie Lavigne
    /// </summary>
    public class UnitStats
    {
        private static UnitStats plebUnitStats = new UnitStats(7, 3, 0, 0f, 0f);
        /// <summary>
        /// Ensemble pré-fabriqué de statistiques pour une unité de niveau plèbe
        /// Implémenté en singleton car toutes les unités de ce niveau auront les mêmes statistiques
        /// </summary>
        public static UnitStats PlebUnitStats
        {
            get
            {
                if (plebUnitStats == null)
                {
                    return new UnitStats(7, 3, 0, 0f, 0f);
                }
                return plebUnitStats;
            }
        }
        
        private static UnitStats soldierUnitStats = new UnitStats(9, 3, 1, 0f, 0f);
        /// <summary>
        /// Ensemble pré-fabriqué de statistiques pour une unité de niveau soldat
        /// Implémenté en singleton car toutes les unités de ce niveau auront les mêmes statistiques
        /// </summary>
        public static UnitStats SoldierUnitStats
        {
            get
            {
                if (soldierUnitStats == null)
                {
                    return new UnitStats(9, 3, 1, 0f, 0f);
                }
                return soldierUnitStats;
            }
        }
        
        private static UnitStats nobleUnitStats = new UnitStats(12, 5, 1, 0f, 0f);
        /// <summary>
        /// Ensemble pré-fabriqué de statistiques pour une unité de niveau noble
        /// Implémenté en singleton car toutes les unités de ce niveau auront les mêmes statistiques
        /// </summary>
        public static UnitStats NobleUnitStats
        {
            get
            {
                if (nobleUnitStats == null)
                {
                    return new UnitStats(12, 5, 1, 0f, 0f);
                }
                return nobleUnitStats;
            }
        }
        
        /// <summary>
        /// Les points de vie maximum d'une unité 
        /// </summary>
        private int maxHealthPoints;
        public int MaxHealthPoints => maxHealthPoints;
        /// <summary>
        /// La vitesse, en case qu'une unité peut parcourir par tour d'une unité 
        /// </summary>
        private int moveSpeed;
        public int MoveSpeed => moveSpeed;
        /// <summary>
        /// Les points de dégâts qu'une unité peut affliger
        /// </summary>
        private int attackStrength;
        public int AttackStrength => attackStrength;
        /// <summary>
        /// La chance, en pourcentage (sur 1), qu'une unité a de toucher une unité adverse en attaquant
        /// </summary>
        private float hitRate;
        public float HitRate => hitRate;
        /// <summary>
        /// La chance, en pourcentage (sur 1), qu'une unité a d'effectuer une attaque critique sur une unité adverse en attaquant
        /// </summary>
        private float critRate;
        public float CritRate => critRate;
        
        public UnitStats(int maxHealthPoints, int moveSpeed, int attackStrength, float hitRate, float critRate)
        {
            this.maxHealthPoints = maxHealthPoints;
            this.moveSpeed = moveSpeed;
            this.attackStrength = attackStrength;
            this.hitRate = hitRate;
            this.critRate = critRate;
        }
        
        public static UnitStats operator+ (UnitStats addends1, UnitStats addends2) 
        {
            UnitStats sum = new UnitStats(0, 0, 0, 0f, 0f);

            sum.maxHealthPoints = addends1.maxHealthPoints + addends2.maxHealthPoints;
            sum.moveSpeed = addends1.moveSpeed + addends2.moveSpeed;
            sum.attackStrength = addends1.attackStrength + addends2.attackStrength;
            sum.hitRate = addends1.hitRate + addends2.hitRate;
            sum.critRate = addends1.critRate + addends2.critRate;

            return sum;
        }
        
        
    }
}