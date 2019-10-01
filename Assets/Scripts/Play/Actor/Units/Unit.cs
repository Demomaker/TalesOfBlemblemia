using System;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace GameOld
{
    /// <summary>
    /// Les unités sur le tableau qui seront soit contrôllées par le joueur ou l'intelligence artificielle
    /// Auteur: Zacharie Lavigne
    /// </summary>
    public class Unit : MonoBehaviour
    {
        /// <summary>
        /// The tile the unit is on
        /// </summary>
        private Tile unitTile;
        public Tile UnitTile => unitTile;
        /// <summary>
        /// Value of if a unit is an enemy 
        /// </summary>
        private bool isEnemy;
        public bool IsEnemy => isEnemy;
        /// <summary>
        /// The unit's class stats
        /// </summary>
        [SerializeField]
        private UnitStats classStats;
        [SerializeField]
        /// <summary>
        /// The unit's weapon
        /// </summary>
        private Weapon weapon;
        /// <summary>
        /// Array representing the movement cost needed to move to every tile on the grid
        /// </summary>
        private int[,] movementCosts;
        public int[,] MovementCosts => movementCosts;
        /// <summary>
        /// The unit's current health
        /// </summary>
        private int currentHealthPoints;
        public int CurrentHealthPoints => currentHealthPoints;
        /// <summary>
        /// The unit's stats
        /// </summary>
        public UnitStats UnitStats
        {
            get { return classStats + weapon.WeaponStats; }
        }
        /// <summary>
        /// The unit's weapon type
        /// </summary>
        public WeaponType WeaponType
        {
            get { return weapon.WeaponType; }
        }
        /// <summary>
        /// The weapon type this unit has advantage on 
        /// </summary>
        public WeaponType WeaponAdvantage
        {
            get { return weapon.Advantage; }
        }
        /// <summary>
        /// The health points a unit would gain by resting
        /// Resting replenishes half your health points without exceeding the unit's max health
        /// </summary>
        public int HpGainedByResting
        {
            get
            {
                int maxGain = UnitStats.MaxHealthPoints / 2;
                if (currentHealthPoints + maxGain > UnitStats.MaxHealthPoints)
                    return UnitStats.MaxHealthPoints - currentHealthPoints;
                return maxGain;
            }
        }

        private void OnEnable()
        {
            this.currentHealthPoints = UnitStats.MaxHealthPoints;
        }

        public void InitPathCosts(Grid grid)
        {
            //movementCosts = PathFinder.PrepareComputeCost(grid, unitTile.X, unitTile.Y);
        }
        
        public virtual void SetTurnAction(Grid grid)
        {
            if (isEnemy)
            {
                
            }
            else
            {
                //Player input
            }
        }
    }
}