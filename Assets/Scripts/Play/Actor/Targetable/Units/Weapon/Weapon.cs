using UnityEngine;

namespace Game
{
    /// <summary>
    /// A weapon to be held by a unit. A weapon has a bonus to its attack, a hit rate, a critical hit rate, a weapon type and an advantage
    /// Author: Zacharie Lavigne
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        protected const WeaponType SWORD_ADVANTAGE = WeaponType.Axe;
        protected const WeaponType AXE_ADVANTAGE = WeaponType.Spear;
        protected const WeaponType SPEAR_ADVANTAGE = WeaponType.Sword;
        
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
        public WeaponType Advantage => Harmony.Finder.LevelController.RevertWeaponTriangle ? GetAdvantageByWeaponType(advantage) : advantage;

        //BR : Remplaçable par une méthode d'extension. J'en avais pas parlé ?
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