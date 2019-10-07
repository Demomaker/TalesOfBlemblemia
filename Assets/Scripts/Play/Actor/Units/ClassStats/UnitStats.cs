using UnityEngine;

namespace Game
{
    /// <summary>
    /// A unit or weapon's stats
    /// Author: Zacharie Lavigne
    /// </summary>
    public class UnitStats
    {
        private static UnitStats plebUnitStats = null;
        /// <summary>
        /// Pre-made set of stats for weak units
        /// </summary>
        public static UnitStats PlebUnitStats
        {
            get
            {
                if (plebUnitStats == null)
                {
                    return new UnitStats(UnitStatsValues.PLEB_MAX_HEALTH, UnitStatsValues.PLEB_MOVE_SPEED, UnitStatsValues.PLEB_ATTACK_STRENGTH, UnitStatsValues.PLEB_HIT_RATE, UnitStatsValues.PLEB_CRIT_RATE);
                }
                return plebUnitStats;
            }
        }
        
        private static UnitStats soldierUnitStats = null;
        /// <summary>
        /// Pre-made set of stats for medium units
        /// </summary>
        public static UnitStats SoldierUnitStats
        {
            get
            {
                if (soldierUnitStats == null)
                {
                    return new UnitStats(UnitStatsValues.SOLDIER_MAX_HEALTH, UnitStatsValues.SOLDIER_MOVE_SPEED, UnitStatsValues.SOLDIER_ATTACK_STRENGTH, UnitStatsValues.SOLDIER_HIT_RATE, UnitStatsValues.SOLDIER_CRIT_RATE);
                }
                return soldierUnitStats;
            }
        }

        private static UnitStats nobleUnitStats = null;
        /// <summary>
        /// Pre-made set of stats for strong units
        /// </summary>
        public static UnitStats NobleUnitStats
        {
            get
            {
                if (nobleUnitStats == null)
                {
                    return new UnitStats(UnitStatsValues.NOBLE_MAX_HEALTH, UnitStatsValues.NOBLE_MOVE_SPEED, UnitStatsValues.NOBLE_ATTACK_STRENGTH, UnitStatsValues.NOBLE_HIT_RATE, UnitStatsValues.NOBLE_CRIT_RATE);
                }
                return nobleUnitStats;
            }
        }
        
        /// <summary>
        /// The max health points
        /// </summary>
        private int maxHealthPoints;
        public int MaxHealthPoints => maxHealthPoints;
        /// <summary>
        /// The unit's speed in tile per turn
        /// </summary>
        private int moveSpeed;
        public int MoveSpeed => moveSpeed;
        /// <summary>
        /// The base damage a unit's attack would do
        /// </summary>
        private int attackStrength;
        public int AttackStrength => attackStrength;
        /// <summary>
        /// The chance that a unit lands his attack
        /// </summary>
        private float hitRate;
        public float HitRate => hitRate;
        /// <summary>
        /// The chance that a hit is critical
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