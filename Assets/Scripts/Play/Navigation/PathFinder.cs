﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    //Author Pierre-Luc Maltais, Antoine Lessard, Zacharie Lavigne.
    public static class PathFinder
    {
        public static int[,] PrepareComputeCost(Vector2Int from, bool unitIsEnemy)
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
            IsRightMovementPossible(grid, movementCosts, from, costToMove, unitIsEnemy);
            
            IsDownMovementPossible(grid, movementCosts, from, costToMove, unitIsEnemy);

            IsLeftMovementPossible(grid, movementCosts, from, costToMove, unitIsEnemy);

            IsUpMovementPossible(grid, movementCosts, from, costToMove, unitIsEnemy);

            //Once all movements are calculated, the array with the cost to move to every cell is returned
            return movementCosts;
        }

        #region IsMovementPossible
        private static void IsRightMovementPossible(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from, 
            int costToMove, 
            bool unitIsEnemy
        )
        {
            if (from.x < grid.NbColumns - 1 && movementCosts[from.x, from.y] + costToMove < movementCosts[from.x + 1, from.y]
                                            && grid.GetTile(from.x + 1, from.y).CostToMove != int.MaxValue
                                            && (grid.GetTile(from.x + 1, from.y).LinkedUnit == null 
                                            || unitIsEnemy == grid.GetTile(from.x +1, from.y).LinkedUnit.IsEnemy))
            {
                //Changing the value to move to the cell 
                movementCosts[from.x + 1, from.y] = movementCosts[from.x, from.y] + costToMove;
                //Recursive call with the next cell as the starting point
                ComputeCosts(grid, movementCosts, new Vector2Int(from.x + 1, from.y), unitIsEnemy);
            }
        }
        
        private static void IsDownMovementPossible(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from,
            int costToMove, 
            bool unitIsEnemy
        )
        {
            if (from.y < grid.NbLines - 1 && movementCosts[from.x, from.y] + costToMove < movementCosts[from.x, from.y + 1]
                                          && grid.GetTile(from.x, from.y + 1).CostToMove != int.MaxValue
                                          && (grid.GetTile(from.x, from.y + 1).LinkedUnit == null 
                                          || unitIsEnemy == grid.GetTile(from.x, from.y + 1).LinkedUnit.IsEnemy))
            {
                movementCosts[from.x, from.y + 1] = movementCosts[from.x, from.y] + costToMove;
                ComputeCosts(grid, movementCosts, new Vector2Int(from.x,from.y + 1), unitIsEnemy);
            }
        }
        
        private static void IsLeftMovementPossible(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from, 
            int costToMove, 
            bool unitIsEnemy
        )
        {
            if (from.x > 0 && movementCosts[from.x, from.y] + costToMove < movementCosts[from.x - 1, from.y]
                           && grid.GetTile(from.x - 1, from.y).CostToMove != int.MaxValue
                           && (grid.GetTile(from.x - 1, from.y).LinkedUnit == null 
                           || unitIsEnemy == grid.GetTile(from.x -1, from.y).LinkedUnit.IsEnemy))
            {
                movementCosts[from.x - 1, from.y] = movementCosts[from.x, from.y] + costToMove;
                ComputeCosts(grid, movementCosts, new Vector2Int(from.x-1,from.y), unitIsEnemy);
            }
        }

        private static void IsUpMovementPossible(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from, 
            int costToMove, 
            bool unitIsEnemy
        )
        {
            if (from.y > 0 && movementCosts[from.x, from.y] + costToMove < movementCosts[from.x, from.y - 1]
                          && grid.GetTile(from.x, from.y - 1).CostToMove != int.MaxValue
                          && (grid.GetTile(from.x, from.y - 1).LinkedUnit == null 
                          || unitIsEnemy == grid.GetTile(from.x, from.y - 1).LinkedUnit.IsEnemy))
            {
                movementCosts[from.x, from.y - 1] = movementCosts[from.x, from.y] + costToMove;
                ComputeCosts(grid, movementCosts, new Vector2Int(from.x,from.y-1), unitIsEnemy);
            }
        }
        #endregion
        
        public static List<Tile> PrepareFindPath(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from, 
            Vector2Int to, 
            Unit unit
        )
        {
            //If there is a unit on the target the unit must find the closest available tile.
            if (grid.GetTile(to.x, to.y).LinkedUnit != null)
            {
                List<Tile> currentPath = null;
                //check for the closest available tile to target
                
                //Check left
                currentPath = CheckLeftOfTarget(grid, movementCosts, from, to, currentPath, unit);
                //Check right
                currentPath = CheckRightOfTarget(grid, movementCosts, from, to, currentPath, unit);
                //Check up
                currentPath = CheckUpOfTarget(grid, movementCosts, from, to , currentPath, unit);
                //Check down
                currentPath = CheckDownOfTarget(grid, movementCosts, from, to, currentPath, unit);
                
                return currentPath;
            }
            else
            {
                return FindPath(grid, movementCosts, new List<Tile>(), from, to, unit);
            }
            
        }
        //TODO il trouve un path impossible et donne donc une list contenant 1 élément
        #region CheckTargets
        private static List<Tile> CheckLeftOfTarget(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from, 
            Vector2Int to,
            List<Tile> currentPath, 
            Unit unit
        )
        {
            if (grid.GetTile(to.x - 1, to.y).LinkedUnit == null)
            {
                currentPath = FindPath(grid, movementCosts, new List<Tile>(), from, to, unit);
            }

            return currentPath;
        }

        private static List<Tile> CheckDownOfTarget(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from, 
            Vector2Int to,
            List<Tile> currentPath, 
            Unit unit
        )
        {
            if (grid.GetTile(to.x, to.y - 1).LinkedUnit == null)
            {
                List<Tile> pathDown = FindPath(grid, movementCosts, new List<Tile>(), from, new Vector2Int(to.x,to.y-1), unit);
                if (currentPath == null ||
                    movementCosts[currentPath.Last().LogicalPosition.x, currentPath.Last().LogicalPosition.y]
                    > movementCosts[pathDown.Last().LogicalPosition.x, pathDown.Last().LogicalPosition.y])
                {
                    currentPath = pathDown;
                }
            }

            return currentPath;
        }

        private static List<Tile> CheckUpOfTarget(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from, 
            Vector2Int to,
            List<Tile> currentPath, 
            Unit unit
        )
        {
            if (grid.GetTile(to.x, to.y + 1).LinkedUnit == null)
            {
                List<Tile> pathUp = FindPath(grid, movementCosts, new List<Tile>(), from, new Vector2Int(to.x,to.y + 1), unit);
                if (currentPath == null ||
                    movementCosts[currentPath.Last().LogicalPosition.x, currentPath.Last().LogicalPosition.y]
                    > movementCosts[pathUp.Last().LogicalPosition.x, pathUp.Last().LogicalPosition.y])
                {
                    currentPath = pathUp;
                }
            }

            return currentPath;
        }

        private static List<Tile> CheckRightOfTarget(
            GridController grid, 
            int[,] movementCosts, 
            Vector2Int from, 
            Vector2Int to, 
            List<Tile> currentPath, 
            Unit unit
        )
        {
            if (grid.GetTile(to.x + 1, to.y).LinkedUnit == null)
            {
                List<Tile> pathRight = FindPath(grid, movementCosts, new List<Tile>(), from, new Vector2Int(to.x + 1,to.y), unit);
                if (currentPath == null ||
                    movementCosts[currentPath.Last().LogicalPosition.x, currentPath.Last().LogicalPosition.y]
                    > movementCosts[pathRight.Last().LogicalPosition.x, pathRight.Last().LogicalPosition.y])
                {
                    currentPath = pathRight;
                }
            }
            return currentPath;
        }
        #endregion

        public static List<Tile> FindPath(
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
                        movementCosts = PrepareComputeCost(from, unit.IsEnemy);
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
            int[,] movementCosts, 
            List<Tile> path, 
            Vector2Int from,
            Vector2Int to, 
            Unit unit
        )
        {
            List <Tile> pathInOrder = FindPath(grid, PrepareComputeCost(from, unit.IsEnemy), new List<Tile>(), from, to, unit);
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