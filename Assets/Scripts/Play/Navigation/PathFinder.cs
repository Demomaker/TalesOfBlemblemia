using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public static class PathFinder
    {
        public static int[,] PrepareComputeCost(Vector2Int from)
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
            
            return ComputeCosts(grid, movementCosts, from.x, from.y);
        }
        
        private static int[,] ComputeCosts(GridController grid, int[,] movementCosts, int fromX, int fromY)
        {
            int costToMove = grid.GetTile(fromX, fromY).CostToMove;
            
            IsRightMouvementPossible(grid, movementCosts, fromX, fromY, costToMove);

            IsDownMouvementPossible(grid, movementCosts, fromX, fromY, costToMove);

            IsLeftMouvementPossible(grid, movementCosts, fromX, fromY, costToMove);

            IsUpMouvementPossible(grid, movementCosts, fromX, fromY, costToMove);

            //Once all movements are calculated, the array with the cost to move to every cell is returned
            return movementCosts;
        }

        private static void IsUpMouvementPossible(GridController grid, int[,] movementCosts, int fromX, int fromY, int costToMove)
        {
            if (fromY > 0 && movementCosts[fromX, fromY] + costToMove < movementCosts[fromX, fromY - 1]
                          && grid.GetTile(fromX, fromY - 1).CostToMove != int.MaxValue
                          && !grid.GetTile(fromX,fromY - 1).LinkedUnit.IsEnemy)
            {
                movementCosts[fromX, fromY - 1] = movementCosts[fromX, fromY] + costToMove;
                ComputeCosts(grid, movementCosts, fromX, fromY - 1);
            }
        }

        private static void IsLeftMouvementPossible(GridController grid, int[,] movementCosts, int fromX, int fromY, int costToMove)
        {
            if (fromX > 0 && movementCosts[fromX, fromY] + costToMove < movementCosts[fromX - 1, fromY]
                          && grid.GetTile(fromX - 1, fromY).CostToMove != int.MaxValue
                          && !grid.GetTile(fromX -1, fromY).LinkedUnit.IsEnemy)
            {
                movementCosts[fromX - 1, fromY] = movementCosts[fromX, fromY] + costToMove;
                ComputeCosts(grid, movementCosts, fromX - 1, fromY);
            }
        }

        private static void IsDownMouvementPossible(GridController grid, int[,] movementCosts, int fromX, int fromY, int costToMove)
        {
            if (fromY < grid.NbLines - 1 && movementCosts[fromX, fromY] + costToMove < movementCosts[fromX, fromY + 1]
                                        && grid.GetTile(fromX, fromY + 1).CostToMove != int.MaxValue
                                        && !grid.GetTile(fromX, fromY +1).LinkedUnit.IsEnemy)
            {
                movementCosts[fromX, fromY + 1] = movementCosts[fromX, fromY] + costToMove;
                ComputeCosts(grid, movementCosts, fromX, fromY + 1);
            }
        }

        private static void IsRightMouvementPossible(GridController grid, int[,] movementCosts, int fromX, int fromY, int costToMove)
        {
            if (fromX < grid.NbColumns - 1 && movementCosts[fromX, fromY] + costToMove < movementCosts[fromX + 1, fromY]
                                       && grid.GetTile(fromX + 1, fromY).CostToMove != int.MaxValue
                                       && !grid.GetTile(fromX +1, fromY).LinkedUnit.IsEnemy)
            {
                //Changing the value to move to the cell 
                movementCosts[fromX + 1, fromY] = movementCosts[fromX, fromY] + costToMove;
                //Recursive call with the next cell as the starting point
                ComputeCosts(grid, movementCosts, fromX + 1, fromY);
            }
        }

        public static List<Tile> PrepareFindPath(GridController grid, int[,] movementCosts,
            int fromX, int fromY, int toX, int toY)
        {
            //If there is a unit on the target the unit must find the closest available tile.
            if (grid.GetTile(toX, toY).LinkedUnit!=null)
            {
                List<Tile> currentPath = null;
                //check for the closest available tile to target
                
                //Check left
                currentPath = CheckLeftOfTarget(grid, movementCosts, fromX, fromY, toX, toY, currentPath);
                //Check right
                currentPath = CheckRightOfTarget(grid, movementCosts, fromX, fromY, toX, toY, currentPath);
                //Check up
                currentPath = CheckUpOfTarget(grid, movementCosts, fromX, fromY, toX, toY, currentPath);
                //Check down
                currentPath = CheckDownOfTarget(grid, movementCosts, fromX, fromY, toX, toY, currentPath);
                return currentPath;
            }
            else
            {
                return FindPath(grid, movementCosts, new List<Tile>(), fromX, fromY, toX, toY);
            }
            
        }

        private static List<Tile> CheckLeftOfTarget(GridController grid, int[,] movementCosts, int fromX, int fromY, int toX, int toY,
            List<Tile> currentPath)
        {
            if (grid.GetTile(toX - 1, toY).LinkedUnit == null)
            {
                currentPath = FindPath(grid, movementCosts, new List<Tile>(), fromX, fromY, toX - 1, toY);
            }

            return currentPath;
        }

        private static List<Tile> CheckDownOfTarget(GridController grid, int[,] movementCosts, int fromX, int fromY, int toX, int toY,
            List<Tile> currentPath)
        {
            if (grid.GetTile(toX, toY - 1).LinkedUnit == null)
            {
                List<Tile> pathDown = FindPath(grid, movementCosts, new List<Tile>(), fromX, fromY, toX, toY - 1);
                if (currentPath == null ||
                    movementCosts[currentPath.Last().LogicalPosition.x, currentPath.Last().LogicalPosition.y]
                    > movementCosts[pathDown.Last().LogicalPosition.x, pathDown.Last().LogicalPosition.y])
                {
                    currentPath = pathDown;
                }
            }

            return currentPath;
        }

        private static List<Tile> CheckUpOfTarget(GridController grid, int[,] movementCosts, int fromX, int fromY, int toX, int toY,
            List<Tile> currentPath)
        {
            if (grid.GetTile(toX, toY + 1).LinkedUnit == null)
            {
                List<Tile> pathUp = FindPath(grid, movementCosts, new List<Tile>(), fromX, fromY, toX, toY + 1);
                if (currentPath == null ||
                    movementCosts[currentPath.Last().LogicalPosition.x, currentPath.Last().LogicalPosition.y]
                    > movementCosts[pathUp.Last().LogicalPosition.x, pathUp.Last().LogicalPosition.y])
                {
                    currentPath = pathUp;
                }
            }

            return currentPath;
        }

        private static List<Tile> CheckRightOfTarget(GridController grid, int[,] movementCosts, int fromX, int fromY, int toX,
            int toY, List<Tile> currentPath)
        {
            if (grid.GetTile(toX + 1, toY).LinkedUnit == null)
            {
                List<Tile> pathRight = FindPath(grid, movementCosts, new List<Tile>(), fromX, fromY, toX + 1, toY);
                if (currentPath == null ||
                    movementCosts[currentPath.Last().LogicalPosition.x, currentPath.Last().LogicalPosition.y]
                    > movementCosts[pathRight.Last().LogicalPosition.x, pathRight.Last().LogicalPosition.y])
                {
                    currentPath = pathRight;
                }
            }
            return currentPath;
        }

        private static List<Tile> FindPath(GridController grid, int[,] movementCosts, List<Tile> path, int fromX, int fromY, int toX, int toY)
        {
            //Exit statement
            if (grid.GetTile(fromX, fromY).Equals(grid.GetTile(toX, toY)))
            {
                path.Remove(path.Last());
                
                return path;
            }

            path.Add(null);

            int x = 0;
            int y = 0;
            
            (x,y) = CheckLeftMouvement(grid, movementCosts, path, toX, toY, x, y);
            
            //Check Right
            (x,y) = CheckRightMouvement(grid, movementCosts, path, toX, toY, x, y);
            
            //Check Up
            (x,y) = CheckUpMouvement(grid, movementCosts, path, toX, toY, x, y);
            
            //Check Down
            (x,y) = CheckDownMouvement(grid, movementCosts, path, toX, toY, x, y);

            return FindPath(grid, movementCosts, path, fromX, fromY, x, y);
        }

        private static (int,int) CheckDownMouvement(GridController grid, int[,] movementCosts, List<Tile> path, int toX, int toY, int x, int y)
        {
            if (toY + 1 < grid.NbLines && movementCosts[toX, toY + 1] < movementCosts[toX, toY])
            {
                if (path.Last() == null || path.Last().CostToMove > grid.GetTile(toX, toY + 1).CostToMove)
                {
                    path[path.Count - 1] = grid.GetTile(toX, toY + 1);

                    x = toX;
                    y = toY + 1;
                }
            }
            return (x,y);
        }

        private static (int,int) CheckUpMouvement(GridController grid, int[,] movementCosts, List<Tile> path, int toX, int toY, int x, int y)
        {
            if (toY - 1 >= 0 && movementCosts[toX, toY - 1] < movementCosts[toX, toY])
            {
                if (path.Last() == null || path.Last().CostToMove > grid.GetTile(toX, toY - 1).CostToMove)
                {
                    path[path.Count - 1] = grid.GetTile(toX, toY - 1);

                    x = toX;
                    y = toY - 1;
                }
            }
            return (x,y);
        }

        private static (int,int) CheckRightMouvement(GridController grid, int[,] movementCosts, List<Tile> path, int toX, int toY, int x, int y)
        {
            if (toX + 1 < grid.NbColumns && movementCosts[toX + 1, toY] < movementCosts[toX, toY])
            {
                if (path.Last() == null || path.Last().CostToMove > grid.GetTile(toX + 1, toY).CostToMove)
                {
                    path[path.Count - 1] = grid.GetTile(toX + 1, toY);

                    x = toX + 1;
                    y = toY;
                }
            }
            return (x,y);
        }

        private static (int,int) CheckLeftMouvement(GridController grid, int[,] movementCosts, List<Tile> path, int toX, int toY, int x, int y)
        {
            if (toX - 1 >= 0 && movementCosts[toX - 1, toY] < movementCosts[toX, toY])
            {
                if (path.Last() == null || path.Last().CostToMove > grid.GetTile(toX - 1, toY).CostToMove)
                {
                    path[path.Count - 1] = grid.GetTile(toX - 1, toY);

                    x = toX - 1;
                    y = toY;
                }
            }
            return (x,y);
        }


        public static List<Tile> GetPath(GridController grid, int[,] movementCosts, List<Tile> path, int fromX, int fromY,
            int toX, int toY)
        {
            List <Tile> pathInOrder = FindPath(grid, movementCosts, path, fromX, fromY, toX, toY);
            pathInOrder.Reverse();
            return pathInOrder;
        }
    }
}