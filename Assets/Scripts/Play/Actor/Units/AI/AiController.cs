using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Game
{
    /// <summary>
    /// Logique d'intelligence artificielle pour les unités ennemies
    /// Auteur: Zacharie Lavigne
    /// </summary>
    public class AiController : MonoBehaviour
    {
        /// <summary>
        /// Le nombre d'actions parmis lesquelles l'IA va choisir dépendamment du niveau de difficulté
        /// </summary>
        private int nbOfChoice = 5;

        private static Random random = new Random();

        public AiController(){}

        /// <summary>
        /// Détermine l'action qu'une unité contrôllée par intelligence artificielle devra exécuter pendant son tour
        /// </summary>
        /// <param name="aiUnit">L'unité dont l'action est déterminé</param>
        /// <returns>L'action que l'unité devra exécuter</returns>
        public Action DetermineAction(Unit aiUnit, Grid grid)
        {
            //Les actions potentielles que l'unité pourra effectuer pendant son tour
            List<Action> actionsToDo = ScanForEnnemies(grid, aiUnit);

            //Initie des chemins et ennemis pour tester
            //InitTestValues(actionsToDo, grid);
            
            //Attribution d'un score à chaque action
            ComputeChoiceScores(actionsToDo, aiUnit);
            
            //Les meilleures actions parmis lesquelles l'unité va choisir aléatoirement
            Action[] bestActions = GetBestActions(actionsToDo);

            //Vérification de s'il serait raisonnable de se reposer à se tour-ci
            AddRestActionIfNeeded(grid, bestActions, aiUnit, actionsToDo);

            return SelectRandomBestAction(bestActions);
        }

        /// <summary>
        /// Sélectionne au hasard une action parmis les meilleures actions possibles
        /// </summary>
        /// <param name="bestActions">Les meilleures actions possibles</param>
        /// <returns>Une action aléatoirement choisie parmis les meilleures actions que l'unité peut faire</returns>
        private Action SelectRandomBestAction(Action[] bestActions)
        {
            Action bestAction = null;
            bool allActionsAreNull = true;
            for (int i = 0; i < bestActions.Length; i++)
            {
                if (bestActions[i] != null)
                {
                    allActionsAreNull = false;
                    i = bestActions.Length;
                }
            }
            if (!allActionsAreNull)
            {
                while (bestAction == null)
                {
                    bestAction = bestActions[random.Next(0, nbOfChoice)];
                }
            }
            return bestAction;
        }

        /// <summary>
        /// Rajoute à la liste d'actions potentielles 
        /// </summary>
        /// <param name="bestActions">Tableau des meilleures actions que l'unité pourrait faire</param>
        /// <param name="aiUnit">L'unité controllée</param>
        private void AddRestActionIfNeeded(Grid grid, Action[] bestActions, Unit aiUnit, List<Action> potentialAction)
        {
            if(aiUnit.UnitStats.MaxHealthPoints * 1.33f < aiUnit.UnitStats.MaxHealthPoints + aiUnit.HpGainedByResting)
            {
                bestActions[nbOfChoice - 1] = new Action(FindFleePath(grid, potentialAction, aiUnit), ActionType.Rest, 12, null);
            }
        }

        /// <summary>
        /// Attribue un score à chaque action potentielle que l'unité pourrait effectuer
        /// </summary>
        /// <param name="actionsToDo">Les actions potentielles que l'unité pourrait faire</param>
        /// <param name="aiUnit">L'unité controllée par IA</param>
        private void ComputeChoiceScores(List<Action> actionsToDo, Unit aiUnit)
        {
            for(int i = 0; i < actionsToDo.Count; i++)
            {
                actionsToDo[i].Score += HpChoiceMod(aiUnit, actionsToDo[i].Target.CurrentHealthPoints) +
                                        DistanceChoiceMod(aiUnit, actionsToDo[i].Path) +
                                        ClassTypeChoiceMod(aiUnit, actionsToDo[i].Target.WeaponType) +
                                        EnvironmentChoiceMod(aiUnit/*, potentialTargets[i].CurrentTile*/) +
                                        HarmChoiceMod(aiUnit, actionsToDo[i].Target);
            }
        }
        
        /// <summary>
        /// Retourne un tableau des actions aux meilleurs scores en ordre décroissant
        /// Le nombre d'actions dépend du niveau de difficulté
        /// </summary>
        /// <param name="actionsToDo">Les actions que l'unité peut effectuer</param>
        /// <returns>Un tableau d'actions aux meilleurs scores, en ordre décroissant'</returns>
        private Action[] GetBestActions(List<Action> actionsToDo)
        {
            //Copie de la liste pour ne pas la modifier en dehors de la méthode
            actionsToDo = new List<Action>(actionsToDo);
            
            //Tableau regroupant les actions aux meilleurs scores
            Action[] bestAttacks = new Action[nbOfChoice];
            
            //L'index de la meilleure action dans la liste d'actions potentielles 
            int actionIndex = -1;
            
            for (int i = 0; i < nbOfChoice; i++)
            {
                actionIndex = GetBestAttack(actionsToDo);
                if (actionIndex > -1)
                {
                    bestAttacks[i] = actionsToDo[actionIndex];
                    //On enlève la meilleure action de la liste pour pouvoir trouver la meilleure ensuite
                    actionsToDo.RemoveAt(actionIndex);
                }
            }
            
            return bestAttacks;
        }
        
        /// <summary>
        /// Trouve l'index de l'action au meilleur score dans une liste
        /// </summary>
        /// <param name="actionsToDo">Liste d'actions potentielles</param>
        /// <returns>L'index de la meilleure action; -1 si la liste d'actions est vide</returns>
        private int GetBestAttack(List<Action> actionsToDo)
        {
            int bestActionIndex = -1;
            
            float highestActionScore = -1;
            
            if (actionsToDo.Count == 0)
            {
                return -1;
            }
            
            for(int i = 0; i < actionsToDo.Count; i++)
            {
                if(actionsToDo[i].Score > highestActionScore)
                {
                    bestActionIndex = i;
                    highestActionScore = actionsToDo[i].Score;
                }
            }
            return bestActionIndex;
        }

        /// <summary>
        /// Trouve le chemin vers où l'unité devrait s'enfuir pour se reposer
        /// </summary>
        /// <param name="aiUnit">L'unité contrôllée par IA</param>
        /// <returns>Le chemin à prendre pour s'enfuir</returns>
        private List<Tile> FindFleePath(Grid grid, List<Action> potentialActions, Unit aiUnit)
        {
            int[,] optionMap = null;//new int[grid.Width, grid.Height]; TODO remettre quand on aura Grid
            for (int i = 0; i < optionMap.GetLength(0); i++)
            {
                for (int j = 0; j < optionMap.GetLength(1); j++)
                {
                    optionMap[i, j] = 0;
                }
            }

            Unit ennemy = null;
            for (int i = 0; i < optionMap.GetLength(0); i++)
            {
                for (int j = 0; j < optionMap.GetLength(1); j++)
                {
                    for (int k = 0; k < potentialActions.Count; k++)
                    {
                        ennemy = potentialActions[k].Target;
                        optionMap[i, j] += (int)Math.Ceiling(((double)(ennemy.MovementCosts[i, j] - 1) / (double)ennemy.UnitStats.MoveSpeed));
                    }
                }
            }

            int highestScore = 0;
            int posX = -1;
            int posY = -1;
            for (int i = 0; i < optionMap.GetLength(0); i++)
            {
                for (int j = 0; j < optionMap.GetLength(1); j++)
                {
                    if (optionMap[i, j] >= highestScore && aiUnit.MovementCosts[i,j] <= aiUnit.UnitStats.MoveSpeed)
                    {
                        highestScore = optionMap[i, j];
                        posX = i;
                        posY = j;
                    }
                }
            }

            return FindPathTo(grid, aiUnit, posX, posY);
        }    

        /// <summary>
        /// Trouve un chemin vers une unité ciblée
        /// </summary>
        /// <param name="grid">La grille de jeu</param>
        /// <param name="aiUnit">L'unité contrôllée par l'IA</param>
        /// <param name="potentialTarget">L'unité ciblée</param>
        /// <returns>Le chemin le plus court vers la cible</returns>
        private List<Tile> FindPathTo(Grid grid, Unit aiUnit, Unit potentialTarget)
        {
            //return PathFinder.GetPath(grid, aiUnit.MovementCosts, new List<Tile>(), aiUnit.UnitTile.X, aiUnit.UnitTile.Y,
            //   potentialTarget.UnitTile.X, potentialTarget.UnitTile.Y);
            //TODO remettre quand on aura le PathFinder
            return null;
        }
        
        /// <summary>
        /// Trouve un chemin vers une position ciblée
        /// </summary>
        /// <param name="grid">La grille de jeu</param>
        /// <param name="aiUnit">L'unité contrôllée par l'IA</param>
        /// <param name="targetX">La position en X ciblée</param>
        /// <param name="targetY">La position en Y ciblée</param>
        /// <returns>Le chemin le plus court vers la cible</returns>
        private List<Tile> FindPathTo(Grid grid, Unit aiUnit, int targetX, int targetY)
        {
            //return PathFinder.GetPath(grid, aiUnit.MovementCosts, new List<Tile>(), aiUnit.UnitTile.X, aiUnit.UnitTile.Y,
            //    targetX, targetY);
            //TODO remettre quand on aura le PathFinder
            return null;
        }

        /// <summary>
        /// Trouve les ennemis de l'unité et initialise une liste d'une action potentielle par ennemis
        /// </summary>
        /// <param name="grid">La grille de jeu</param>
        /// <param name="aiUnit">L'unité contrôllée par l'IA</param>
        /// <returns>La liste des ennemis de l'unité</returns>
        private List<Action> ScanForEnnemies(Grid grid, Unit aiUnit)
        {
            List<Action> actions = new List<Action>();
            List<Unit> units = null; //new List<Unit>(Units); TODO créer un get qui obtient les unité dans le niveau
            for (int i = 0; i < units.Count; i++)
            {
                if(!units[i].IsEnemy)
                    actions.Add(new Action(FindPathTo(grid, aiUnit, units[i]), ActionType.Attack, 20f, units[i]));
            }
            return actions;
        }
        
        /// <summary>
        /// Détermine comment la vie de l'unité à attaquer et la force d'attaque de l'IA
        /// influencent le choix de l'action à faire de l'IA
        /// </summary>
        /// <param name="aiUnit">L'unité contrôllée par IA</param>
        /// <param name="targetHp">La vie de l'unité à attaquer</param>
        /// <returns>Le modificateur de score causé par les points de vie de l'unité à attaquer et la force d'attaque de l'IA</returns>
        public float HpChoiceMod(Unit aiUnit, int targetHp)
        {
            if (targetHp - aiUnit.UnitStats.AttackStrength <= 0)
                return 1;
            return -(targetHp - aiUnit.UnitStats.AttackStrength);
        }
        
        /// <summary>
        /// Détermine comment la distance entre l'unité à attaquer et l'IA
        /// influence le choix de l'action à faire de l'IA
        /// </summary>
        /// <param name="aiUnit">L'unité contrôllée par IA</param>
        /// <param name="targetPath">Le chemin à parcourir pour atteindre l'unité à attaquer</param>
        /// <returns>Le modificateur de score causé par la distance à parcourir pour atteindre l'unité à attaquer</returns>
        public float DistanceChoiceMod(Unit aiUnit, List<Tile> targetPath)
        {
            double nbToursDouble = CalculatePathCost(targetPath) / aiUnit.UnitStats.MoveSpeed;
            int nbTours = (int)Math.Ceiling(nbToursDouble);
            return -(2 * nbTours) + 3;
        }

        /// <summary>
        /// Calcule le nombre de mouvements nécessaires exécuter les mouvements d'un chemin
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int CalculatePathCost(List<Tile> path)
        {
            int cost = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                //TODO remettre quand il y aura tile
                //cost += path[i].costToMove;
            }
            return cost;
        }
        
        /// <summary>
        /// Détermine comment les types d'armes de l'unité à attaquer et celle de l'IA
        /// influencent le choix de l'action à faire de l'IA
        /// </summary>
        /// <param name="aiUnit">L'unité contrôllée par IA</param>
        /// <param name="targetWeaponType">Le type d'arme de l'unité à attaquer</param>
        /// <returns>Le modificateur de score causé par les types d'armes de l'unité à attaquer et celle de l'IA</returns>
        public float ClassTypeChoiceMod(Unit aiUnit, WeaponType targetWeaponType)
        {
            int choiceMod = 0;
            if (aiUnit.WeaponAdvantage == targetWeaponType)
            {
                switch (targetWeaponType)
                {
                    case WeaponType.Axe:
                        choiceMod = 3;
                        break;
                    case WeaponType.Spear:
                        choiceMod = 1;
                        break;
                    case WeaponType.Sword:
                        choiceMod = 2;
                        break;
                }
            }
            else if (aiUnit.WeaponType != targetWeaponType)
            {
                switch (targetWeaponType)
                {
                    case WeaponType.Axe:
                        choiceMod = -1;
                        break;
                    case WeaponType.Spear:
                        choiceMod = -2;
                        break;
                    case WeaponType.Sword:
                        choiceMod = -2;
                        break;
                }
            }

            return choiceMod;
        }
        /// <summary>
        /// Détermine comment l'environnement où se trouve l'unité ennemie
        /// influence le choix de l'action à faire de l'IA
        /// </summary>
        /// <param name="aiUnit">L'unité contrôllée par IA</param>
        /// <param name="enemyTargetTile">La case où se trouve l'unité à attaquer</param>
        /// <returns>Le modificateur de score causé par la tuile où se trouve l'unité à attaquer</returns>
        public float EnvironmentChoiceMod( Unit aiUnit/*, Tile enemyTargetTile*/)
        {
            float environmentChoiceMod = 0.0f;
            switch (aiUnit.WeaponType)
            {
                case WeaponType.Spear:
                    /*
                    if(tile.tileType == TileType.Fortress)
                        environmentChoiceMod = -2f;
                    else if(tile.tileType == TileType.Forest)
                        environmentChoiceMod = -1f;
                    */
                    break;
                case WeaponType.Axe:
                    /*
                    if(tile.tileType == TileType.Fortress)
                        environmentChoiceMod = -4f;
                    else if(tile.tileType == TileType.Forest)
                        environmentChoiceMod = -3f;
                    */
                    break;
                case WeaponType.Sword:
                    /*
                    if(tile.tileType == TileType.Fortress)
                        environmentChoiceMod = -3f;
                    else if(tile.tileType == TileType.Forest)
                        environmentChoiceMod = -2f;
                    */
                    break;
            }
            return environmentChoiceMod;
        }
        /// <summary>
        /// Détermine comment la vie que va perdre l'IA en attaquant une unité
        /// influence le choix de l'action à faire de l'IA
        /// </summary>
        /// <param name="aiUnit">L'unité contrôllée par IA</param>
        /// <param name="targetUnit">L'unité à attaquer</param>
        /// <returns>Le modificateur de score causé par les dommages qu'infligeront l'unité à attaquer</returns>
        public float HarmChoiceMod(Unit aiUnit, Unit targetUnit)
        {
            if (aiUnit.CurrentHealthPoints - targetUnit.UnitStats.AttackStrength <= 0)
                return -4f;
            return -(0.8f * targetUnit.UnitStats.AttackStrength);
        }
    }
}