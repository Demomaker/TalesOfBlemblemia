using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The artificial intelligence controller for the enemy units
    /// Author: Zacharie Lavigne
    /// </summary>
    public class AiController
    {
        private readonly int nbOfChoice;
        
        private AiControllerValues aiControllerValues = new AiControllerValues();

        public AiController(int nbOfChoice)
        {
            this.nbOfChoice = nbOfChoice;
        }

        public Action DetermineAction(Unit playableUnit, List<Unit> enemyUnits, List<Targetable> targetsToDestroy, GridController grid, LevelController levelController)
        {
            if (levelController.RevertWeaponTriangle && !(aiControllerValues is AiControllerValuesRevert)) aiControllerValues = new AiControllerValuesRevert();
            
            if (!playableUnit.IsAwake)
            {
                if ((playableUnit.IsAwake = CheckRadius(playableUnit, enemyUnits)) == false)
                {
                    return null;
                }
            }
            //Every potential actions the unit could do
            List<Action> actionsToDo = ScanForEnemies(playableUnit, enemyUnits, grid);
            actionsToDo.AddRange(ScanForTargets(playableUnit, targetsToDestroy, grid));
            
            //Setting every action's turn
            ComputeChoiceScores(actionsToDo, playableUnit);
            
            //The best potential actions
            List<Action> bestActions = GetBestActions(actionsToDo);

            //Verification of if resting and fleeing is needed
            AddRestActionIfNeeded(bestActions, playableUnit, actionsToDo, grid);

            //The action is randomly selected from the best possible ones
            return SelectRandomBestAction(bestActions);
        }

        private bool CheckRadius(Unit playableUnit, List<Unit> enemyUnits)
        {
            var playableUnitPosition = playableUnit.CurrentTile.LogicalPosition;
            int radius = playableUnit.DetectionRadius;
            foreach (var unit in enemyUnits)
            {
                var diffX = playableUnitPosition.x - unit.CurrentTile.LogicalPosition.x;
                var diffY = playableUnitPosition.y - unit.CurrentTile.LogicalPosition.y;
                if ((diffX * diffX + diffY * diffY) <= radius * radius)
                    return true;
            }
            return false;
        }

        private Action SelectRandomBestAction(List<Action> bestActions)
        {

            float weakestScore = GetWeakestScore(bestActions);
            if (weakestScore < 0)
            {
                foreach (var action in bestActions)
                {
                    action.Score -= weakestScore;
                }
            }
            
            int totalScore = (int) GetTotalScore(bestActions);
            
            if (totalScore > 0)
            {
                float diceRoll = Finder.Random.Next(0, totalScore);

                for (int i = 0; i < bestActions.Count; i++)
                {
                    var action = bestActions[i];
                    if (i >= bestActions.Count - 1 || (diceRoll < action.Score && diceRoll >= bestActions[i+1].Score))
                    {
                        return action;
                    }

                    diceRoll -= action.Score;
                }
            }
            return null;
        }

        private float GetWeakestScore(List<Action> bestActions)
        {
            float weakestScore = int.MaxValue;
            foreach (var action in bestActions)
            {
                if (action.Score < weakestScore)
                {
                    weakestScore = action.Score;
                }
            }
            return weakestScore;
        }

        private float GetTotalScore(List<Action> bestActions)
        {
            float totalScore = 0;
            foreach (var action in bestActions)
            {
                totalScore += action.Score;
            }
            return totalScore;
        }

        private void AddRestActionIfNeeded(List<Action> bestActions, Unit playableUnit, List<Action> actionsToDo, GridController grid)
        {
            //The unit should flee and rest if 4/3 of its health is smaller than its maximum health plus the health it would gain by resting
            if(playableUnit.CurrentHealthPoints < playableUnit.Stats.MaxHealthPoints/3)
            {
                bestActions.Add(new Action(FindFleePath(actionsToDo, playableUnit, grid), ActionType.Rest, null, aiControllerValues.BaseChoiceActionScore));
            }
        }
        
        private void ComputeChoiceScores(List<Action> actionsToDo, Unit playableUnit)
        {
            foreach (var action in actionsToDo)
            {
                if (action.Target is Unit targetUnit)
                {
                    action.Score += HpChoiceMod(playableUnit, targetUnit.CurrentHealthPoints) +
                                    DistanceChoiceMod(playableUnit, action.Target, action.Path) +
                                    WeaponTypeChoiceMod(playableUnit, targetUnit.WeaponType) +
                                    EnvironmentChoiceMod(playableUnit, targetUnit.CurrentTile) +
                                    HarmChoiceMod(playableUnit, targetUnit);
                }
            }
        }
        
        private List<Action> GetBestActions(List<Action> actionsToDo)
        {
            //Copy to not change the original list
            actionsToDo = new List<Action>(actionsToDo);
            
            List<Action> bestAttacks = new List<Action>();
            
            //The index of the highest scored action in the action list
            int actionIndex = 0;
            
            for (int i = 0; i < nbOfChoice; i++)
            {
                actionIndex = FindBestAttack(actionsToDo);
                if (actionIndex > -1 && actionsToDo[actionIndex] != null)
                {
                    bestAttacks.Add(actionsToDo[actionIndex]);
                    //The current best action in the list is removed
                    actionsToDo.RemoveAt(actionIndex);
                }
                else if(bestAttacks.Count == 0)
                    i = nbOfChoice;
            }
            
            return bestAttacks;
        }
        
        /// <summary>
        /// Finds the index of the highest scored action in a list
        /// </summary>
        /// <param name="actionsToDo">The potential actions the unit could do</param>
        /// <returns>The index of the highest scored action</returns>
        private int FindBestAttack(List<Action> actionsToDo)
        {
            int bestActionIndex = -1;
            
            float highestActionScore = int.MinValue;
            
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
        private List<Tile> FindFleePath(List<Action> potentialActions, Unit playableUnit, GridController grid)
        {
            int[,] optionMap = new int[grid.NbColumns, grid.NbLines]; 
            for (int i = 0; i < optionMap.GetLength(0); i++)
            {
                for (int j = 0; j < optionMap.GetLength(1); j++)
                {
                    optionMap[i, j] = 0;
                }
            }

            Unit enemy = null;
            for (int i = 0; i < optionMap.GetLength(0); i++)
            {
                for (int j = 0; j < optionMap.GetLength(1); j++)
                {
                    for (int k = 0; k < potentialActions.Count; k++)
                    {
                        if (enemy.GetType() == typeof(Unit))
                        {
                            enemy = (Unit)potentialActions[k].Target;
                            //Each tile has a score based on the added number of turns it would take each player's unit to get there
                            optionMap[i, j] += (int)Math.Ceiling(((double)(enemy.MovementCosts[i, j] - 1) / (double)enemy.Stats.MoveSpeed));
                        }
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

            return FindPathTo(playableUnit, position, grid);
        }
        
        /// <summary>
        /// Finds the shortest path to a target unit
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="potentialTarget">The target unit</param>
        /// <returns>The path to a target unit</returns>
        private List<Tile> FindPathTo(Unit playableUnit, Targetable potentialTarget, GridController grid)
        {
            List<Tile> path;
            if (playableUnit.TargetIsInRange(potentialTarget))
            {
                path = new List<Tile>();
                path.Add(playableUnit.CurrentTile);
            }
            else
            {
                path = PathFinder.GetPath(
                    grid,
                    new List<Tile>(), 
                    new Vector2Int(playableUnit.CurrentTile.LogicalPosition.x, playableUnit.CurrentTile.LogicalPosition.y), 
                    new Vector2Int(potentialTarget.CurrentTile.LogicalPosition.x, potentialTarget.CurrentTile.LogicalPosition.y), 
                    playableUnit
                );
                /*
                Vector2Int currentPosition = new Vector2Int(playableUnit.CurrentTile.LogicalPosition.x, playableUnit.CurrentTile.LogicalPosition.y);
                Vector2Int targetPosition = new Vector2Int(potentialTarget.CurrentTile.LogicalPosition.x, potentialTarget.CurrentTile.LogicalPosition.y);
                int[,] computedCost = PathFinder.ComputeCost(currentPosition, true);
                path = PathFinder.FindPath(computedCost, currentPosition, targetPosition, playableUnit);*/
                path.Reverse();
            }

            return path;
        }

        /// <summary>
        /// Finds the shortest path to a target position
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetPosition">The target position</param>
        /// <returns>The shortest path to a target position</returns>
        private List<Tile> FindPathTo(Unit playableUnit, Vector2Int targetPosition, GridController grid)
        {
            return PathFinder.GetPath(
                grid,
                new List<Tile>(), 
                new Vector2Int(playableUnit.CurrentTile.LogicalPosition.x, playableUnit.CurrentTile.LogicalPosition.y), 
                new Vector2Int(targetPosition.x, targetPosition.y),
                playableUnit
            );
        }
        
        /// <summary>
        /// Initializes action based on the unit's enemies
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="enemyUnits">The player's units</param>
        /// <returns>A list of potential actions, on per enemy</returns>
        private List<Action> ScanForEnemies(Unit playableUnit, List<Unit> enemyUnits, GridController grid)
        {
            List<Action> actions = new List<Action>();
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                if(enemyUnits[i] != null && enemyUnits[i].IsPlayer)
                    actions.Add(new Action(FindPathTo(playableUnit, enemyUnits[i], grid), ActionType.Attack, enemyUnits[i], aiControllerValues.BaseChoiceActionScore));
            }
            return actions;
        }
        
        /// <summary>
        /// Initializes action based on the unit's targets
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="enemyTargets">The AI's targets</param>
        /// <returns>A list of potential actions, on per target</returns>
        private List<Action> ScanForTargets(Unit playableUnit, List<Targetable> enemyTargets, GridController grid)
        {
            List<Action> actions = new List<Action>();
            foreach (var target in enemyTargets)
            {
                if (target != null)
                {
                    actions.Add(new Action(FindPathTo(playableUnit, target, grid), ActionType.Attack, target, aiControllerValues.BaseTargetActionScore));
                }
            }
            return actions;
        }
        
        /// <summary>
        /// Calculates how the health of a target unit influences the score of an action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetHp">The target unit's health</param>
        /// <returns>The score modifier caused by the target's health</returns>
        public float HpChoiceMod(Unit playableUnit, int targetHp)
        {
            if (targetHp - playableUnit.Stats.AttackStrength <= 0)
                return aiControllerValues.KillingEnemyChoiceMod;
            return -(targetHp - playableUnit.Stats.AttackStrength);
        }
        
        /// <summary>
        /// Calculates how the distance to a target unit influences the score of an action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetPath">The path to a target unit</param>
        /// <returns>The score modifier caused by the target's distance</returns>
        public float DistanceChoiceMod(Unit playableUnit, Targetable target, List<Tile> targetPath)
        {
            float scoreMod = 0f;
            if (playableUnit.TargetIsInRange(target))
                scoreMod = aiControllerValues.AdjacentTargetChoiceMod;
            else if (targetPath.Count <= 1)
                scoreMod += aiControllerValues.InaccessibleTargetChoiceMod;
            else
            {
                double nbToursDouble = PathFinder.CalculatePathCost(targetPath, playableUnit.MovementCosts) / playableUnit.Stats.MoveSpeed;
                int nbTours = (int)Math.Ceiling(nbToursDouble);
                scoreMod = aiControllerValues.TurnMultiplierForDistanceChoiceMod * nbTours + aiControllerValues.TurnAdderForDistanceChoiceMod;
            }
            return scoreMod;
        }
        
        /// <summary>
        /// Calculates how the weapon types between the unit and a target influences the score of an action
        /// </summary>
        /// <param name="playableUnit">The unit currently controlled by the AI</param>
        /// <param name="targetWeaponType">The target of an attack's weapon type</param>
        /// <returns>The score modifier caused by the weapon types involved in an attack</returns>
        public float WeaponTypeChoiceMod(Unit playableUnit, WeaponType targetWeaponType)
        {
            float choiceMod = 0f;
            if (playableUnit.WeaponAdvantage == targetWeaponType)
            {
                switch (targetWeaponType)
                {
                    case WeaponType.Axe:
                        choiceMod = aiControllerValues.AttackingAxeWithAdvantageChoiceMod;
                        break;
                    case WeaponType.Spear:
                        choiceMod = aiControllerValues.AttackingSpearWithAdvantageChoiceMod;
                        break;
                    case WeaponType.Sword:
                        choiceMod = aiControllerValues.AttackingSwordWithAdvantageChoiceMod;
                        break;
                }
            }
            else if (playableUnit.WeaponType != targetWeaponType)
            {
                switch (targetWeaponType)
                {
                    case WeaponType.Axe:
                        choiceMod = aiControllerValues.AttackingAxeWithoutAdvantageChoiceMod;
                        break;
                    case WeaponType.Spear:
                        choiceMod = aiControllerValues.AttackingSpearWithoutAdvantageChoiceMod;
                        break;
                    case WeaponType.Sword:
                        choiceMod = aiControllerValues.AttackingSwordWithoutAdvantageChoiceMod;
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
        public float EnvironmentChoiceMod( Unit playableUnit, Tile enemyTargetTile)
        {
            float environmentChoiceMod = 0.0f;
            switch (playableUnit.WeaponType)
            {
                case WeaponType.Spear:
                    if(enemyTargetTile.TileType == TileType.Fortress)
                        environmentChoiceMod = aiControllerValues.SpearAttackingFortressChoiceMod;
                    else if(enemyTargetTile.TileType == TileType.Forest)
                        environmentChoiceMod = aiControllerValues.SpearAttackingForestChoiceMod;
                    break;
                case WeaponType.Axe:
                    if(enemyTargetTile.TileType == TileType.Fortress)
                        environmentChoiceMod = aiControllerValues.AxeAttackingFortressChoiceMod;
                    else if(enemyTargetTile.TileType == TileType.Forest)
                        environmentChoiceMod = aiControllerValues.AxeAttackingForestChoiceMod;
                    break;
                case WeaponType.Sword:
                    if(enemyTargetTile.TileType == TileType.Fortress)
                        environmentChoiceMod = aiControllerValues.SwordAttackingFortressChoiceMod;
                    else if(enemyTargetTile.TileType == TileType.Forest)
                        environmentChoiceMod = aiControllerValues.SwordAttackingForestChoiceMod;
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
        public float HarmChoiceMod(Unit playableUnit, Unit targetUnit)
        {
            if (playableUnit.CurrentHealthPoints - targetUnit.Stats.AttackStrength <= 0)
                return aiControllerValues.PotentialDeathChoiceMod;
            return aiControllerValues.DamageReceiveChoiceMod * targetUnit.Stats.AttackStrength;
        }
    }
}