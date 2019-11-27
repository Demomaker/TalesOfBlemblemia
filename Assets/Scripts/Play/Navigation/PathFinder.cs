﻿using System;
using System.Collections.Generic;
using System.Linq;
 using System.Security.Cryptography;
 using UnityEngine;

namespace Game
{
    //Authors: Pierre-Luc Maltais, Antoine Lessard, Zacharie Lavigne
    public static class PathFinder
    {
        public static int[,] ComputeCost(Vector2Int from, bool unitIsEnemy)
        {
            GridController grid = Finder.GridController;
            //Initialize movementCosts array
            int[,] movementCosts = new int[grid.NbColumns,grid.NbLines];

            for (int i = 0; i < grid.NbColumns; i++)
            {
                for (int j = 0; j < grid.NbLines; j++)
                {
                    movementCosts[i, j] = int.MaxValue;
                }
            }
            movementCosts[from.x, from.y] = 0;
            return ComputeCosts(grid, movementCosts, from, unitIsEnemy);
        }
        private static int[,] ComputeCosts(GridController grid, int[,] movementCosts, Vector2Int from, bool unitIsEnemy)
        {
            int costToMove = grid.GetTile(from.x, from.y).CostToMove;

            //We check if the cost to move is smaller then the cost of the current tile. We do so for each direction
            //Checking right movement first.
            IsMovementPossible(grid, movementCosts, from, 1, 0, costToMove, unitIsEnemy);
            
            //Checking downward movement.
            IsMovementPossible(grid, movementCosts, from, 0, -1, costToMove, unitIsEnemy);

            //Checking left movement
            IsMovementPossible(grid, movementCosts, from, -1, 0, costToMove, unitIsEnemy);

            //Checking upward movement.
            IsMovementPossible(grid, movementCosts, from, 0, 1, costToMove, unitIsEnemy);

            //Once all movements are calculated, the array with the cost to move to every cell is returned
            return movementCosts;
        }

        private static bool IsTileAccessible(Tile tile)
        {
            return tile.CostToMove != int.MaxValue;
        }

        private static bool IsTileOccupied(Tile tile)
        {
            return tile.LinkedUnit != null;
        }

        private static bool CostToMoveIsSmallerThenCurrentTile(int currentTileCost, int targetedTileCost)
        {
            return currentTileCost < targetedTileCost;
        }

        private static bool UnitOnTileIsFriendly(Tile tile, bool unitIsEnemy)
        {
            return unitIsEnemy == tile.LinkedUnit.IsEnemy;
        }

        private static bool CheckIfTargetIsWithinBoardLimit(Vector2Int from, GridController grid, int xModifier, int yModifier)
        {
            return from.x + xModifier < grid.NbColumns - 1 
                    && from.x + xModifier >= 0 
                    && from.y + yModifier < grid.NbLines - 1 
                    && from.y + yModifier >= 0;
        }

        private static void IsMovementPossible(
            GridController grid,
            int[,] movementCosts,
            Vector2Int from,
            int xModifier,
            int yModifier,
            int costToMove,
            bool unitIsEnemy
        )
        {
            if (CheckIfTargetIsWithinBoardLimit(from, grid, 0, 0)
                && CheckIfTargetIsWithinBoardLimit(from, grid, xModifier, yModifier)
                && CostToMoveIsSmallerThenCurrentTile(movementCosts[from.x, from.y] + costToMove, movementCosts[from.x + xModifier, from.y + yModifier] )
                && IsTileAccessible(grid.GetTile(from.x + xModifier, from.y + yModifier))
                && (!IsTileOccupied(grid.GetTile(from.x + xModifier, from.y + yModifier)) 
                || UnitOnTileIsFriendly(grid.GetTile(from.x + xModifier, from.y + yModifier), unitIsEnemy))
            )
            {
                //Changing the value to move to the cell 
                movementCosts[from.x + xModifier, from.y + yModifier] = movementCosts[from.x, from.y] + costToMove;
                //Recursive call with the next cell as the starting point
                ComputeCosts(grid, movementCosts, new Vector2Int(from.x + xModifier, from.y + yModifier), unitIsEnemy);
            }
        }

        public static List<Tile> FindPath(
            int[,] movementCosts, 
            Vector2Int from, 
            Vector2Int to, 
            Unit unit
        )
        {
            GridController grid = Finder.GridController;
            //If there is a unit on the target the unit must find the closest available tile.
            if (grid.GetTile(to.x, to.y).LinkedUnit != null)
            {
                List<Tile> currentPath;
                //check for the closest available tile to target
                List<List<Tile>> possiblePaths = new List<List<Tile>>();
                //Check left
                possiblePaths.Add(CheckIfTargetIsAccessible(grid, movementCosts, from, to, -1, 0, unit));
                //Check right
                possiblePaths.Add(CheckIfTargetIsAccessible(grid, movementCosts, from, to, 1, 0, unit));  
                //Check up
                possiblePaths.Add(CheckIfTargetIsAccessible(grid, movementCosts, from, to, 0, -1, unit));
                //Check down
                possiblePaths.Add(CheckIfTargetIsAccessible(grid, movementCosts, from, to, 0, +1, unit));

                currentPath = ChooseTarget(movementCosts, possiblePaths, null);
                
                return currentPath;
            }

            List<Tile> path = new List<Tile>();
            return FindPath(grid, movementCosts, path , from, to, unit);
        }

        private static List<Tile> ChooseTarget(int[,] movementCosts, List<List<Tile>> possiblePaths, List<Tile> currentPath)
        {
            foreach (var possiblePath in possiblePaths)
            {
                if (currentPath == null ||
                    movementCosts[currentPath.Last().LogicalPosition.x, currentPath.Last().LogicalPosition.y]
                    > movementCosts[possiblePath.Last().LogicalPosition.x, possiblePath.Last().LogicalPosition.y])
                {
                    currentPath = possiblePath;
                }
            }
            return currentPath;
        }

        private static List<Tile> CheckIfTargetIsAccessible(
            GridController grid,
            int[,] movementCosts,
            Vector2Int from,
            Vector2Int to,
            int toXModifier,
            int toYModifier,
            Unit unit
            )
        {
            List<Tile> newPath = new List<Tile>();
            if (grid.GetTile(to.x + toXModifier, to.y + toYModifier).LinkedUnit == null)
            {
                    newPath = FindPath(grid, movementCosts, new List<Tile>(), from, new Vector2Int(to.x + toXModifier,to.y + toYModifier), unit);
            }
            return newPath;
        }

        private static List<Tile> FindPath(
            GridController grid, 
            int[,] movementCosts, 
            List<Tile> path, 
            Vector2Int from,
            Vector2Int to, 
            Unit unit
        )
        {
            //Exit statement if path impossible
            if (CheckIfTileInaccessible(to, movementCosts))
            {
                List<Tile> emptyPath = new List<Tile>();
                emptyPath.Add(grid.GetTile(from.x, from.y));
                return emptyPath;
            }
            //Exit statement when path is found
            if (grid.GetTile(from.x, from.y).Equals(grid.GetTile(to.x, to.y)))
            {
                path.Remove(null);
                path.Reverse();
                if (unit.IsEnemy && path.Count > 0)
                {
                    Tile lastTileInTurn = path.Last();
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        var nexTileLogicalPosition = path[i + 1].LogicalPosition;
                        if (movementCosts[nexTileLogicalPosition.x, nexTileLogicalPosition.y] > unit.MovesLeft)
                        {
                            lastTileInTurn = path[i];
                            i = path.Count;
                        }
                    }

                    if (lastTileInTurn != null && !lastTileInTurn.IsAvailable)
                    {
                        TileType lastTileType = lastTileInTurn.TileType;
                        grid.GetTile(lastTileInTurn.LogicalPosition.x, lastTileInTurn.LogicalPosition.y).MakeObstacle();
                        movementCosts = ComputeCost(from, unit.IsEnemy);
                        var t = grid.GetTile(lastTileInTurn.LogicalPosition.x, lastTileInTurn.LogicalPosition.y);
                        grid.GetTile(lastTileInTurn.LogicalPosition.x, lastTileInTurn.LogicalPosition.y).UnMakeObstacle(lastTileType);
                        int lastIndex = path.Count - 1;
                        
                        return FindPath(grid, movementCosts, new List<Tile>(), 
                            new Vector2Int(path[0].LogicalPosition.x,path[0].LogicalPosition.y), 
                            new Vector2Int(path[lastIndex].LogicalPosition.x,path[lastIndex].LogicalPosition.y), 
                            unit);
                    }
                }

                return path;
            }

            path.Add(null);
            
            Vector2Int target = new Vector2Int(-1, -1);
           
            //Check Down
            target = CheckDownMovement(grid, movementCosts, path, to, target, unit.IsEnemy);

            //Check Right
            target = CheckRightMovement(grid, movementCosts, path, to, target, unit.IsEnemy);
            
            //Check Up
            target = CheckUpMovement(grid, movementCosts, path, to, target, unit.IsEnemy);
            
            //Check Left
            target = CheckLeftMovement(grid, movementCosts, path, to, target, unit.IsEnemy);
            
            return FindPath(grid, movementCosts, path, from, target, unit);
        }


        private static bool CheckIfTileInaccessible(Vector2Int to, int[,] movementCosts)
        {
            bool tileIsInaccessible = true;
            if (to.x >= 0 && to.y >= 0)
            {
                if (tileIsInaccessible && to.x > 0)
                    tileIsInaccessible = movementCosts[to.x - 1, to.y] == Int32.MaxValue;
                if (tileIsInaccessible && to.y > 0)
                    tileIsInaccessible = movementCosts[to.x, to.y - 1] == Int32.MaxValue;
                if (tileIsInaccessible && to.x < movementCosts.GetLength(0) - 1)
                    tileIsInaccessible = movementCosts[to.x + 1, to.y] == Int32.MaxValue;
                if (tileIsInaccessible && to.y < movementCosts.GetLength(1) - 1)
                    tileIsInaccessible = movementCosts[to.x, to.y + 1] == Int32.MaxValue;
            }
            return tileIsInaccessible;
        }
        
        #region CheckMovements
        private static Vector2Int CheckDownMovement(GridController grid, int[,] movementCosts, List<Tile> path, Vector2Int to, Vector2Int newPosition, bool unitIsEnemy)
        {
            if (to.y + 1 < grid.NbLines && to.y + 1 >= 0 && movementCosts[to.x, to.y + 1] < movementCosts[to.x, to.y])
            {
                var tile = grid.GetTile(to.x, to.y + 1);
                //The first tile in the path should be available
                if (unitIsEnemy && path.Count == 1 && !tile.IsAvailable)
                    return newPosition;
                if (path.Last() == null || (path.Last().CostToMove > tile.CostToMove && (tile.LinkedUnit == null || tile.LinkedUnit.IsEnemy != unitIsEnemy)))
                {
                    path[path.Count - 1] = tile;

                    newPosition.x = to.x;
                    newPosition.y = to.y + 1;
                }
            }
            return newPosition;
        }

        private static Vector2Int CheckUpMovement(GridController grid, int[,] movementCosts, List<Tile> path, Vector2Int to, Vector2Int newPosition, bool unitIsEnemy)
        {
            if (to.y - 1 < grid.NbLines && to.y - 1 >= 0 && movementCosts[to.x, to.y - 1] < movementCosts[to.x, to.y])
            {
                var tile = grid.GetTile(to.x, to.y - 1);
                //The first tile in the path should be available
                if (unitIsEnemy && path.Count == 1 && !tile.IsAvailable)
                    return newPosition;
                if (path.Last() == null || (path.Last().CostToMove > tile.CostToMove && (tile.LinkedUnit == null || tile.LinkedUnit.IsEnemy != unitIsEnemy)))
                {
                    path[path.Count - 1] = tile;

                    newPosition.x = to.x;
                    newPosition.y = to.y - 1;
                }
            }
            return newPosition;
        }

        private static Vector2Int CheckRightMovement(GridController grid, int[,] movementCosts, List<Tile> path, Vector2Int to, Vector2Int newPosition, bool unitIsEnemy)
        {
            if (to.x + 1 < grid.NbColumns && to.x + 1 >= 0 && movementCosts[to.x + 1, to.y] < movementCosts[to.x, to.y])
            {
                var tile = grid.GetTile(to.x + 1, to.y);
                //The first tile in the path should be available
                if (unitIsEnemy && path.Count == 1 && !tile.IsAvailable)
                    return newPosition;
                if (path.Last() == null || (path.Last().CostToMove > tile.CostToMove && (tile.LinkedUnit == null || tile.LinkedUnit.IsEnemy != unitIsEnemy)))
                {
                    path[path.Count - 1] = tile;

                    newPosition.x = to.x + 1;
                    newPosition.y = to.y;
                }
            }
            return newPosition;
        }

        private static Vector2Int CheckLeftMovement(GridController grid, int[,] movementCosts, List<Tile> path, Vector2Int to, Vector2Int newPosition, bool unitIsEnemy)
        {
            if (to.x - 1 < grid.NbColumns && to.x - 1 >= 0 && movementCosts[to.x - 1, to.y] < movementCosts[to.x, to.y])
            {
                var tile = grid.GetTile(to.x - 1, to.y);
                //The first tile in the path should be available
                if (unitIsEnemy && path.Count == 1 && !tile.IsAvailable)
                    return newPosition;
                if (path.Last() == null || (path.Last().CostToMove > tile.CostToMove && (tile.LinkedUnit == null || tile.LinkedUnit.IsEnemy != unitIsEnemy)))
                {
                    path[path.Count - 1] = tile;

                    newPosition.x = to.x - 1;
                    newPosition.y = to.y;
                }
            }
            return newPosition;
        }
        #endregion
        
        public static List<Tile> GetPath(
            GridController grid,
            List<Tile> path, 
            Vector2Int from,
            Vector2Int to, 
            Unit unit
        )
        {
            int[,] moveCosts = ComputeCost(from, unit.IsEnemy);
            List<Tile> currentPath = new List<Tile>();
            
            List <Tile> pathInOrder = FindPath(grid, moveCosts , currentPath, from, to, unit);
            if (path != null)
                pathInOrder.Reverse();
            return pathInOrder;
        }

        /// <summary>
        /// Calculates de cost of movements needed to travel a path
        /// Author: Zacharie Lavigne
        /// </summary>
        /// <param name="path">The path to calculate the cost</param>
        /// <param name="movementCosts">The cost in movements to get to every tile in the grid for a specific unit</param>
        /// <returns>The cost of movement of the path</returns>
        public static int CalculatePathCost(List<Tile> path, int[,] movementCosts)
        {
            if (path.Count > 0)
                return movementCosts[path.Last().LogicalPosition.x, path.Last().LogicalPosition.y];
            return 0;
        }
    }
}