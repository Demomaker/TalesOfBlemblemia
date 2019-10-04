using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The artificial intelligence controller for the enemy units
    /// Author: Zacharie Lavigne
    /// </summary>
    public static class AiController
    {
        /// <summary>
        /// The number of actions from which the enemy may choose randomly from based on the difficulty level 
        /// </summary>
        private static int nbOfChoice = 5;
        /// <summary>
        /// Chooses an action to do for a unit and executes it
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="enemyUnits">The player's units</param>
        public static void PlayTurn(Unit playableUnit, List<Unit> enemyUnits)
        {
            Action actionToDo = DetermineAction(playableUnit, enemyUnits);
            ExecuteAction(playableUnit, actionToDo);
        }
        /// <summary>
        /// Executes an enemy unit's turn action
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
            //Every potential actions the unit could do
            List<Action> actionsToDo = ScanForEnemies(playableUnit, enemyUnits);
            
            //Setting every action's turn
            ComputeChoiceScores(actionsToDo, playableUnit);
            
            //The best potential actions
            Action[] bestActions = GetBestActions(actionsToDo);

            //Verification of if resting and fleeing is needed
            AddRestActionIfNeeded(bestActions, playableUnit, actionsToDo);

            //The action is randomly selected from the best possible ones
            return SelectRandomBestAction(bestActions);
        }
        /// <summary>
        /// Randomly chooses an action from the best possible actions to do
        /// </summary>
        /// <param name="bestActions">The best possible actions to do</param>
        /// <returns>A randomly chosen an action</returns>
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
                    bestAction = bestActions[Finder.Random.Next(0, nbOfChoice)];
                }
            }
            return bestAction;
        }
        /// <summary>
        /// Constante permettant de déterminer si se soigner permettrait de gagner assez de vie pour le considérer comme option
        /// </summary>
        private const float HEALTH_MOD_FOR_RESTING = 1.33f; 
        /// <summary>
        /// Adds resting as a potential action if needed
        /// </summary>
        /// <param name="bestActions">The best possible actions to do</param>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="actionsToDo">The potential actions the unit could do</param>
        private static void AddRestActionIfNeeded(Action[] bestActions, Unit playableUnit, List<Action> actionsToDo)
        {
            //The unit should flee and rest if 4/3 of its health is smaller than its maximum health plus the health it would gain by resting
            if(playableUnit.Stats.MaxHealthPoints * HEALTH_MOD_FOR_RESTING < playableUnit.Stats.MaxHealthPoints + playableUnit.HpGainedByResting)
            {
                bestActions[nbOfChoice - 1] = new Action(FindFleePath(actionsToDo, playableUnit), ActionType.Rest, 12, null);
            }
        }
        /// <summary>
        /// Computes the score of every action the unit could do
        /// </summary>
        /// <param name="actionsToDo">The potential actions the unit could do</param>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        private static void ComputeChoiceScores(List<Action> actionsToDo, Unit playableUnit)
        {
            for(int i = 0; i < actionsToDo.Count; i++)
            {
                actionsToDo[i].Score += HpChoiceMod(playableUnit, actionsToDo[i].Target.CurrentHealthPoints) +
                                        DistanceChoiceMod(playableUnit, actionsToDo[i].Path) +
                                        WeaponTypeChoiceMod(playableUnit, actionsToDo[i].Target.WeaponType) +
                                        EnvironmentChoiceMod(playableUnit, actionsToDo[i].Target.CurrentTile) +
                                        HarmChoiceMod(playableUnit, actionsToDo[i].Target);
            }
        }
        /// <summary>
        /// Gets the best possible actions (with the highest scores) in descending order
        /// The number of best actions depends on the difficulty level
        /// </summary>
        /// <param name="actionsToDo">The action to execute on this turn</param>
        /// <returns>An array of all the best possible actions</returns>
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
                actionIndex = FindBestAttack(actionsToDo);
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
        /// Finds the index of the highest scored action in a list
        /// </summary>
        /// <param name="actionsToDo">The potential actions the unit could do</param>
        /// <returns>The index of the highest scored action</returns>
        private static int FindBestAttack(List<Action> actionsToDo)
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
        /// Finds the safest place a unit could go to rest
        /// </summary>
        /// <param name="potentialActions">The potential actions of this unit</param>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <returns>The path to the safest tile to go</returns>
        private static List<Tile> FindFleePath(List<Action> potentialActions, Unit playableUnit)
        {
            int[,] optionMap = new int[Finder.GridController.NbColumns, Finder.GridController.NbLines]; 
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
            Vector2Int position = new Vector2Int(-1, -1);
            for (int i = 0; i < optionMap.GetLength(0); i++)
            {
                for (int j = 0; j < optionMap.GetLength(1); j++)
                {
                    if (optionMap[i, j] >= highestScore && playableUnit.MovementCosts[i,j] <= playableUnit.Stats.MoveSpeed)
                    {
                        highestScore = optionMap[i, j];
                        position.x = i;
                        position.y = j;
                    }
                }
            }

            return FindPathTo(playableUnit, position);
        }
        /// <summary>
        /// Finds the shortest path to a target unit
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="potentialTarget">The target unit</param>
        /// <returns>The path to a target unit</returns>
        private static List<Tile> FindPathTo(Unit playableUnit, Unit potentialTarget)
        {
            return PathFinder.GetPath(Finder.GridController, playableUnit.MovementCosts, new List<Tile>(), playableUnit.CurrentTile.LogicalPosition.x, playableUnit.CurrentTile.LogicalPosition.y,
               potentialTarget.CurrentTile.LogicalPosition.x, potentialTarget.CurrentTile.LogicalPosition.y);
        }

        /// <summary>
        /// Finds the shortest path to a target position
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetPosition">The target position</param>
        /// <returns>The shortest path to a target position</returns>
        private static List<Tile> FindPathTo(Unit playableUnit, Vector2Int targetPosition)
        {
            return PathFinder.GetPath(Finder.GridController, playableUnit.MovementCosts, new List<Tile>(), playableUnit.CurrentTile.LogicalPosition.x, playableUnit.CurrentTile.LogicalPosition.y,
                targetPosition.x, targetPosition.y);
        }
        /// <summary>
        /// Initializes action based on the unit's enemies
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="enemyUnits">The player's units</param>
        /// <returns>A list of potential actions, on per enemy</returns>
        private static List<Action> ScanForEnemies(Unit playableUnit, List<Unit> enemyUnits)
        {
            List<Action> actions = new List<Action>();
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                if(!enemyUnits[i].IsEnemy)
                    actions.Add(new Action(FindPathTo(playableUnit, enemyUnits[i]), ActionType.Attack, 20f, enemyUnits[i]));
            }
            return actions;
        }
        /// <summary>
        /// Calculates how the health of a target unit influences the score of an action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetHp">The target unit's health</param>
        /// <returns>The score modifier caused by the target's health</returns>
        public static float HpChoiceMod(Unit playableUnit, int targetHp)
        {
            if (targetHp - playableUnit.Stats.AttackStrength <= 0)
                return 2;
            return -(targetHp - playableUnit.Stats.AttackStrength);
        }
        /// <summary>
        /// Calculates how the distance to a target unit influences the score of an action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetPath">The path to a target unit</param>
        /// <returns>The score modifier caused by the target's distance</returns>
        public static float DistanceChoiceMod(Unit playableUnit, List<Tile> targetPath)
        {
            double nbToursDouble = PathFinder.CalculatePathCost(targetPath, playableUnit.MovementCosts) / playableUnit.Stats.MoveSpeed;
            int nbTours = (int)Math.Ceiling(nbToursDouble);
            return -(2 * nbTours) + 3;
        }
        /// <summary>
        /// Calculates how the weapon types between the unit and a target influences the score of an action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetWeaponType">The target of an attack's weapon type</param>
        /// <returns>The score modifier caused by the weapon types involved in an attack</returns>
        public static float WeaponTypeChoiceMod(Unit playableUnit, WeaponType targetWeaponType)
        {
            int choiceMod = 0;
            if (playableUnit.WeaponAdvantage == targetWeaponType)
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
            else if (playableUnit.WeaponType != targetWeaponType)
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
        /// Calculates how the tile a target unit is on influences the score of an action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="enemyTargetTile">The target enemy's tile location</param>
        /// <returns>The score modifier caused by the target unit's tile</returns>
        public static float EnvironmentChoiceMod( Unit playableUnit, Tile enemyTargetTile)
        {
            float environmentChoiceMod = 0.0f;
            switch (playableUnit.WeaponType)
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
        /// Calculates how the the potential damage a unit would receive by attacking a target unit influences the score of an action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetUnit">The target unit</param>
        /// <returns>The score modifier caused by the potential damage a unit would receive by attacking a target unit</returns>
        public static float HarmChoiceMod(Unit playableUnit, Unit targetUnit)
        {
            if (playableUnit.CurrentHealthPoints - targetUnit.Stats.AttackStrength <= 0)
                return -4f;
            return -(0.8f * targetUnit.Stats.AttackStrength);
        }

    }
}