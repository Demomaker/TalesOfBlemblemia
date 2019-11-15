using UnityEngine;

namespace Game
{
    /// <summary>
    /// A weapon to be held by a unit. A weapon has a bonus to its attack, a hit rate, a critical hit rate, a weapon type and an advantage
    /// Author: Jérémie Bertrand, Zacharie Lavigne
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {

        [SerializeField] UnitStats weaponStats;
        
        public UnitStats WeaponStats => weaponStats;

        /// <summary>
        /// Le type de l'arme
        /// </summary>
        protected WeaponType weaponType;
        public WeaponType WeaponType => weaponType;
        
        /// <summary>
        /// Le type d'arme contre lequelle l'arme a un avantage
        /// </summary>
        protected WeaponType advantage;
        public WeaponType Advantage => Harmony.Finder.LevelController.RevertWeaponTriangle ? advantage.GetAdvantageByWeaponType() : advantage;
    }
}