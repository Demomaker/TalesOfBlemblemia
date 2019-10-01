using System;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Les unités sur le tableau qui seront soit contrôllées par le joueur ou l'intelligence artificielle
    /// Auteur: Zacharie Lavigne
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// La tuile où se trouve l'unité
        /// </summary>
        private Tile unitTile;
        public Tile UnitTile => unitTile;
        /// <summary>
        /// L'intelligence artificielle de l'unité, seulement utilisée pour les unités ennemies
        /// </summary>
        private AiController aiController;
        /// <summary>
        /// Représente si une unité est ennemie
        /// </summary>
        private bool isEnnemy;
        public bool IsEnnemy => isEnnemy;
        /// <summary>
        /// Les statistiques de la classe, ou niveau, de l'unité
        /// </summary>
        private UnitStats classStats;
        /// <summary>
        /// L'arme de l'unité
        /// </summary>
        private Weapon weapon;
        /// <summary>
        /// Le coût en mouvements pour accéder à chaque cases de la grille de jeu
        /// </summary>
        private int[,] movementCosts;
        public int[,] MovementCosts => movementCosts;
        /// <summary>
        /// Les points de vie courrants de l'unité
        /// </summary>
        private int currentHealthPoints;
        public int CurrentHealthPoints => currentHealthPoints;
        /// <summary>
        /// L'action à effectuer par une unité pendant son tour
        /// </summary>
        private Action turnAction;
        public Action TurnAction => turnAction;

        /// <summary>
        /// Les statistiques de l'unité, c'est-à-dire les statistiques de la classe additionnées à celles de son arme
        /// </summary>
        public UnitStats UnitStats
        {
            get { return classStats + weapon.WeaponStats; }
        }
        /// <summary>
        /// Le type de l'arme de l'arme de l'unité
        /// </summary>
        public WeaponType WeaponType
        {
            get { return weapon.WeaponType; }
        }
        /// <summary>
        /// Le type de l'arme contre laquelle l'arme de l'unité a un avantage
        /// </summary>
        public WeaponType WeaponAdvantage
        {
            get { return weapon.Advantage; }
        }
        /// <summary>
        /// Les points de vie qu'une unité gagne en se reposant
        /// Se reposer redonne la moitié de sa vie à une unité, sans pouvoir dépasser ses points de vie maximum
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

        public Unit(Tile tile, AiController aiController, bool isEnnemy, UnitStats classStats, Weapon weapon)
        {
            this.unitTile = tile;
            this.aiController = aiController;
            this.isEnnemy = isEnnemy;
            this.classStats = classStats;
            this.weapon = weapon;
            this.currentHealthPoints = UnitStats.MaxHealthPoints;
        }

        public void InitPathCosts(Grid grid)
        {
            //movementCosts = PathFinder.PrepareComputeCost(grid, unitTile.X, unitTile.Y);
        }
        
        public virtual void SetTurnAction(Grid grid)
        {
            if (isEnnemy)
            {
                turnAction = aiController.DetermineAction(this, grid);
            }
            else
            {
                //Player input
            }
        }
    }
}