using UnityEngine;

namespace Game
{
    /// <summary>
    /// A weapon to be held by a unit. A weapon has a bonus to its attack, a hit rate, a critical hit rate, a weapon type and an advantage
    /// Author: Zacharie Lavigne, Jérémie Bertrand
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] UnitStats weaponStats;

        protected WeaponType weaponType;
        protected WeaponType advantage;
        
        public UnitStats WeaponStats => weaponStats;
        public WeaponType WeaponType => weaponType;
        public WeaponType Advantage => Harmony.Finder.LevelController.RevertWeaponTriangle ? advantage.GetAdvantageByWeaponType() : advantage;

    }
}