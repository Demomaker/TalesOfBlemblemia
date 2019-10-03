using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Game
{
    /// <summary>
    /// The artificial player that controls the enemy units
    /// Author: Zacharie Lavigne
    /// </summary>
    public static class AiController
    {
        /// <summary>
        /// The number of actions from which the enemy may choose randomly from based on the difficulty level 
        /// </summary>
        private static int nbOfChoice = 5;

        //TODO avoir un Random commun a tout?
        private static Random random = new Random();
        
        public static void PlayTurn(Unit playableUnit, List<Unit> enemyUnits)
        {
            Action actionToDo = DetermineAction(playableUnit, enemyUnits);
            ExecuteAction(playableUnit, actionToDo);
        }

        /// <summary>
        /// Execute an enemy unit's turn action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="actionToDo">The action to execute on this turn</param>
        private static void ExecuteAction(Unit playableUnit, Action actionToDo)
        {
            playableUnit.MoveByPath(actionToDo.Path);
            if (actionToDo.ActionType == ActionType.Attack && actionToDo.Target != null)
            {
                if(!playableUnit.Attack(actionToDo.Target, true))
                    playableUnit.Rest();
            }
            else
            {
                playableUnit.Rest();
            }
        }

        /// <summary>
        /// Finds an action to do for an AI controlled unit on its turn
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="enemyUnits">The player's units</param>
        /// <returns>The action the unit should play on its turn</returns>
        public static Action DetermineAction(Unit playableUnit, List<Unit> enemyUnits)
        {
            //Les actions potentielles que l'unité pourra effectuer pendant son tour
            List<Action> actionsToDo = ScanForEnemies(playableUnit, enemyUnits);
            
            //Attribution d'un score à chaque action
            ComputeChoiceScores(actionsToDo, playableUnit);
            
            //Les meilleures actions parmis lesquelles l'unité va choisir aléatoirement
            Action[] bestActions = GetBestActions(actionsToDo);

            //Vérification de s'il serait raisonnable de se reposer à se tour-ci
            AddRestActionIfNeeded(Finder.GridController, bestActions, playableUnit, actionsToDo);

            return SelectRandomBestAction(bestActions);
        }
        /// <summary>
        /// Sélectionne au hasard une action parmis les meilleures actions possibles
        /// </summary>
        /// <param name="bestActions">Les meilleures actions possibles</param>
        /// <returns>Une action aléatoirement choisie parmis les meilleures actions que l'unité peut faire</returns>
        private static Action SelectRandomBestAction(Action[] bestActions)
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
        private static void AddRestActionIfNeeded(GridController grid, Action[] bestActions, Unit aiUnit, List<Action> potentialAction)
        {
            if(aiUnit.Stats.MaxHealthPoints * 1.33f < aiUnit.Stats.MaxHealthPoints + aiUnit.HpGainedByResting)
            {
                bestActions[nbOfChoice - 1] = new Action(FindFleePath(grid, potentialAction, aiUnit), ActionType.Rest, 12, null);
            }
        }
        /// <summary>
        /// Attribue un score à chaque action potentielle que l'unité pourrait effectuer
        /// </summary>
        /// <param name="actionsToDo">Les actions potentielles que l'unité pourrait faire</param>
        /// <param name="aiUnit">L'unité controllée par IA</param>
        private static void ComputeChoiceScores(List<Action> actionsToDo, Unit aiUnit)
        {
            for(int i = 0; i < actionsToDo.Count; i++)
            {
                actionsToDo[i].Score += HpChoiceMod(aiUnit, actionsToDo[i].Target.CurrentHealthPoints) +
                                        DistanceChoiceMod(aiUnit, actionsToDo[i].Path) +
                                        ClassTypeChoiceMod(aiUnit, actionsToDo[i].Target.WeaponType) +
                                        EnvironmentChoiceMod(aiUnit, actionsToDo[i].Target.CurrentTile) +
                                        HarmChoiceMod(aiUnit, actionsToDo[i].Target);
            }
        }
        /// <summary>
        /// Retourne un tableau des actions aux meilleurs scores en ordre décroissant
        /// Le nombre d'actions dépend du niveau de difficulté
        /// </summary>
        /// <param name="actionsToDo">Les actions que l'unité peut effectuer</param>
        /// <returns>Un tableau d'actions aux meilleurs scores, en ordre décroissant'</returns>
        private static Action[] GetBestActions(List<Action> actionsToDo)
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
        private static int GetBestAttack(List<Action> actionsToDo)
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
        /// <param name="potentialActions">The potential actions of this unit</param>
        /// <param name="aiUnit">L'unité contrôllée par IA</param>
        /// <param name="grid">The level map</param>
        /// <returns>Le chemin à prendre pour s'enfuir</returns>
        private static List<Tile> FindFleePath(GridController grid, List<Action> potentialActions, Unit aiUnit)
        {
            int[,] optionMap = new int[grid.NbColumns, grid.NbLines]; 
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
                        optionMap[i, j] += (int)Math.Ceiling(((double)(ennemy.MovementCosts[i, j] - 1) / (double)ennemy.Stats.MoveSpeed));
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
                    if (optionMap[i, j] >= highestScore && aiUnit.MovementCosts[i,j] <= aiUnit.Stats.MoveSpeed)
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
        private static List<Tile> FindPathTo(GridController grid, Unit aiUnit, Unit potentialTarget)
        {
            return PathFinder.GetPath(grid, aiUnit.MovementCosts, new List<Tile>(), aiUnit.CurrentTile.LogicalPosition.x, aiUnit.CurrentTile.LogicalPosition.y,
               potentialTarget.CurrentTile.LogicalPosition.x, potentialTarget.CurrentTile.LogicalPosition.y);
        }
        /// <summary>
        /// Trouve un chemin vers une position ciblée
        /// </summary>
        /// <param name="grid">La grille de jeu</param>
        /// <param name="aiUnit">L'unité contrôllée par l'IA</param>
        /// <param name="targetX">La position en X ciblée</param>
        /// <param name="targetY">La position en Y ciblée</param>
        /// <returns>Le chemin le plus court vers la cible</returns>
        private static List<Tile> FindPathTo(GridController grid, Unit aiUnit, int targetX, int targetY)
        {
            return PathFinder.GetPath(grid, aiUnit.MovementCosts, new List<Tile>(), aiUnit.CurrentTile.LogicalPosition.x, aiUnit.CurrentTile.LogicalPosition.y,
                targetX, targetY);
        }
        /// <summary>
        /// Trouve les ennemis de l'unité et initialise une liste d'une action potentielle par ennemis
        /// </summary>
        /// <param name="grid">La grille de jeu</param>
        /// <param name="aiUnit">L'unité contrôllée par l'IA</param>
        /// <param name="enemyUnits"></param>
        /// <returns>La liste des ennemis de l'unité</returns>
        private static List<Action> ScanForEnemies(Unit aiUnit, List<Unit> enemyUnits)
        {
            List<Action> actions = new List<Action>();
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                if(!enemyUnits[i].IsEnemy)
                    actions.Add(new Action(FindPathTo(Finder.GridController, aiUnit, enemyUnits[i]), ActionType.Attack, 20f, enemyUnits[i]));
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
        public static float HpChoiceMod(Unit aiUnit, int targetHp)
        {
            if (targetHp - aiUnit.Stats.AttackStrength <= 0)
                return 1;
            return -(targetHp - aiUnit.Stats.AttackStrength);
        }
        /// <summary>
        /// Détermine comment la distance entre l'unité à attaquer et l'IA
        /// influence le choix de l'action à faire de l'IA
        /// </summary>
        /// <param name="aiUnit">L'unité contrôllée par IA</param>
        /// <param name="targetPath">Le chemin à parcourir pour atteindre l'unité à attaquer</param>
        /// <returns>Le modificateur de score causé par la distance à parcourir pour atteindre l'unité à attaquer</returns>
        public static float DistanceChoiceMod(Unit aiUnit, List<Tile> targetPath)
        {
            double nbToursDouble = CalculatePathCost(targetPath) / aiUnit.Stats.MoveSpeed;
            int nbTours = (int)Math.Ceiling(nbToursDouble);
            return -(2 * nbTours) + 3;
        }
        /// <summary>
        /// Calcule le nombre de mouvements nécessaires exécuter les mouvements d'un chemin
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static int CalculatePathCost(List<Tile> path)
        {
            int cost = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                cost += path[i].CostToMove;
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
        public static float ClassTypeChoiceMod(Unit aiUnit, WeaponType targetWeaponType)
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
        public static float EnvironmentChoiceMod( Unit aiUnit, Tile enemyTargetTile)
        {
            float environmentChoiceMod = 0.0f;
            switch (aiUnit.WeaponType)
            {
                case WeaponType.Spear:
                    if(enemyTargetTile.TileType == TileType.Fortress)
                        environmentChoiceMod = -2f;
                    else if(enemyTargetTile.TileType == TileType.Forest)
                        environmentChoiceMod = -1f;
                    break;
                case WeaponType.Axe:
                    if(enemyTargetTile.TileType == TileType.Fortress)
                        environmentChoiceMod = -4f;
                    else if(enemyTargetTile.TileType == TileType.Forest)
                        environmentChoiceMod = -3f;
                    break;
                case WeaponType.Sword:
                    if(enemyTargetTile.TileType == TileType.Fortress)
                        environmentChoiceMod = -3f;
                    else if(enemyTargetTile.TileType == TileType.Forest)
                        environmentChoiceMod = -2f;
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
        public static float HarmChoiceMod(Unit aiUnit, Unit targetUnit)
        {
            if (aiUnit.CurrentHealthPoints - targetUnit.Stats.AttackStrength <= 0)
                return -4f;
            return -(0.8f * targetUnit.Stats.AttackStrength);
        }

    }
}