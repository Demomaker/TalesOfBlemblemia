using UnityEngine;

namespace Game
{
    /// <summary>
    /// A unit or weapon's stats
    /// Author: Zacharie Lavigne
    /// </summary>
    [CreateAssetMenu]
    public class UnitStats : ScriptableObject
    {
        /// <summary>
        /// The max health points
        /// </summary>
        [SerializeField] private int maxHealthPoints;
        public int MaxHealthPoints => maxHealthPoints;
        /// <summary>
        /// The unit's speed in tile per turn
        /// </summary>
        [SerializeField] private int moveSpeed;
        public int MoveSpeed => moveSpeed;
        /// <summary>
        /// The base damage a unit's attack would do
        /// </summary>
        [SerializeField] private int attackStrength;
        public int AttackStrength => attackStrength;
        /// <summary>
        /// The chance that a unit lands his attack
        /// </summary>
        [SerializeField] private float hitRate;
        public float HitRate => hitRate;
        /// <summary>
        /// The chance that a hit is critical
        /// </summary>
        [SerializeField] private float critRate;
        public float CritRate => critRate;

        public static UnitStats operator+ (UnitStats addends1, UnitStats addends2)
        {
            UnitStats sum = ScriptableObject.CreateInstance<UnitStats>();
            
            sum.maxHealthPoints = addends1.maxHealthPoints + addends2.maxHealthPoints;
            sum.moveSpeed = addends1.moveSpeed + addends2.moveSpeed;
            sum.attackStrength = addends1.attackStrength + addends2.attackStrength;
            sum.hitRate = addends1.hitRate + addends2.hitRate;
            sum.critRate = addends1.critRate + addends2.critRate;

            return sum;
        }
    }
}