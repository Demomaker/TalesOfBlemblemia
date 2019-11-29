using System;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// A weapon to be held by a unit. A weapon has a bonus to its attack, a hit rate, a critical hit rate, a weapon type and an advantage
    /// Author: Zacharie Lavigne, Jérémie Bertrand, Antoine Lessard
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] private UnitStats weaponStats;

        protected WeaponType weaponType;
        protected WeaponType advantage;
        private LevelController levelController;
        
        public UnitStats WeaponStats => weaponStats;
        public WeaponType WeaponType => weaponType;
        public WeaponType Advantage => levelController.RevertWeaponTriangle ? advantage.GetAdvantageByWeaponType() : advantage;
        
        private void Awake()
        {
            levelController = Harmony.Finder.LevelController;
        }
    }
}