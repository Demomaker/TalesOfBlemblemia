using UnityEngine;

namespace Game
{
    /// <summary>
    /// A unit or weapon's stats
    /// Author: Zacharie Lavigne
    /// </summary>
    public class UnitStats
    {
        public const int PLEB_MAX_HEALTH = 7;
        public const int PLEB_MOVE_SPEED = 3;
        public const int PLEB_ATTACK_STRENGTH = 0;
        public const float PLEB_HIT_RATE = 0f;
        public const float PLEB_CRIT_RATE = 0f;
        
        public const int SOLDIER_MAX_HEALTH = 9;
        public const int SOLDIER_MOVE_SPEED = 3;
        public const int SOLDIER_ATTACK_STRENGTH = 1;
        public const float SOLDIER_HIT_RATE = 0f;
        public const float SOLDIER_CRIT_RATE = 0f;
        
        public const int NOBLE_MAX_HEALTH = 12;
        public const int NOBLE_MOVE_SPEED = 5;
        public const int NOBLE_ATTACK_STRENGTH = 1;
        public const float NOBLE_HIT_RATE = 0f;
        public const float NOBLE_CRIT_RATE = 0f;
        
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
                    return new UnitStats(PLEB_MAX_HEALTH, PLEB_MOVE_SPEED, PLEB_ATTACK_STRENGTH, PLEB_HIT_RATE, PLEB_CRIT_RATE);
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
                    return new UnitStats(SOLDIER_MAX_HEALTH, SOLDIER_MOVE_SPEED, SOLDIER_ATTACK_STRENGTH, SOLDIER_HIT_RATE, SOLDIER_CRIT_RATE);
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
                    return new UnitStats(NOBLE_MAX_HEALTH, NOBLE_MOVE_SPEED, NOBLE_ATTACK_STRENGTH, NOBLE_HIT_RATE, NOBLE_CRIT_RATE);
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