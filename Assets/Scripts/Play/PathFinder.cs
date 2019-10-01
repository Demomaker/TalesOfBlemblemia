using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
       public static class PathFinder
    {
        public static int[,] PrepareComputeCost(Grid grid, Vector2Int from)
        {
            //Initialize movementCosts array
            int[,] movementCosts = new int[grid.Width,grid.Height];
            
            for (int i = 0; i < grid.Width; i++)
            {
                for (int j = 0; j < grid.Height; j++)
                {
                    movementCosts[i, j] = int.MaxValue;
                }
            }
            movementCosts[from.x, from.y] = 0;
            
            return ComputeCosts(grid, movementCosts, from.x, from.y);
        }
        
        public static int[,] ComputeCosts(Grid grid, int[,] movementCosts, int fromX, int fromY)
        {
            int costToMove = grid.GridElements[fromX, fromY].costToMove;
            
            IsRightMouvementPossible(grid, movementCosts, fromX, fromY, costToMove);

            IsDownMouvementPossible(grid, movementCosts, fromX, fromY, costToMove);

            IsLeftMouvementPossible(grid, movementCosts, fromX, fromY, costToMove);

            IsUpMouvementPossible(grid, movementCosts, fromX, fromY, costToMove);

            //Once all movements are calculated, the array with the cost to move to every cell is returned
            return movementCosts;
        }

        private static void IsUpMouvementPossible(Grid grid, int[,] movementCosts, int fromX, int fromY, int costToMove)
        {
            if (fromY > 0 && movementCosts[fromX, fromY] + costToMove < movementCosts[fromX, fromY - 1]
                          && grid.GridElements[fromX, fromY - 1].costToMove != int.MaxValue)
            {
                movementCosts[fromX, fromY - 1] = movementCosts[fromX, fromY] + costToMove;
                ComputeCosts(grid, movementCosts, fromX, fromY - 1);
            }
        }

        private static void IsLeftMouvementPossible(Grid grid, int[,] movementCosts, int fromX, int fromY, int costToMove)
        {
            if (fromX > 0 && movementCosts[fromX, fromY] + costToMove < movementCosts[fromX - 1, fromY]
                          && grid.GridElements[fromX - 1, fromY].costToMove != int.MaxValue)
            {
                movementCosts[fromX - 1, fromY] = movementCosts[fromX, fromY] + costToMove;
                ComputeCosts(grid, movementCosts, fromX - 1, fromY);
            }
        }

        private static void IsDownMouvementPossible(Grid grid, int[,] movementCosts, int fromX, int fromY, int costToMove)
        {
            if (fromY < grid.Height - 1 && movementCosts[fromX, fromY] + costToMove < movementCosts[fromX, fromY + 1]
                                        && grid.GridElements[fromX, fromY + 1].costToMove != int.MaxValue)
            {
                movementCosts[fromX, fromY + 1] = movementCosts[fromX, fromY] + costToMove;
                ComputeCosts(grid, movementCosts, fromX, fromY + 1);
            }
        }

        private static void IsRightMouvementPossible(Grid grid, int[,] movementCosts, int fromX, int fromY, int costToMove)
        {
            if (fromX < grid.Width - 1 && movementCosts[fromX, fromY] + costToMove < movementCosts[fromX + 1, fromY]
                                       && grid.GridElements[fromX + 1, fromY].costToMove != int.MaxValue)
            {
                //Changing the value to move to the cell 
                movementCosts[fromX + 1, fromY] = movementCosts[fromX, fromY] + costToMove;
                //Recursive call with the next cell as the starting point
                ComputeCosts(grid, movementCosts, fromX + 1, fromY);
            }
        }

        public static List<Tile> FindPath(Grid grid, int[,] movementCosts, List<Tile> path, int fromX, int fromY, int toX, int toY)
        {
            //Exit statement
            if (grid.GridElements[fromX, fromY].Equals(grid.GridElements[toX, toY]))
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

        private static (int,int) CheckDownMouvement(Grid grid, int[,] movementCosts, List<Tile> path, int toX, int toY, int x, int y)
        {
            if (toY + 1 < grid.Height && movementCosts[toX, toY + 1] < movementCosts[toX, toY])
            {
                if (path.Last() == null || path.Last().costToMove > grid.GridElements[toX, toY + 1].costToMove)
                {
                    path[path.Count - 1] = grid.GridElements[toX, toY + 1];

                    x = toX;
                    y = toY + 1;
                }
            }
            return (x,y);
        }

        private static (int,int) CheckUpMouvement(Grid grid, int[,] movementCosts, List<Tile> path, int toX, int toY, int x, int y)
        {
            if (toY - 1 >= 0 && movementCosts[toX, toY - 1] < movementCosts[toX, toY])
            {
                if (path.Last() == null || path.Last().costToMove > grid.GridElements[toX, toY - 1].costToMove)
                {
                    path[path.Count - 1] = grid.GridElements[toX, toY - 1];

                    x = toX;
                    y = toY - 1;
                }
            }
            return (x,y);
        }

        private static (int,int) CheckRightMouvement(Grid grid, int[,] movementCosts, List<Tile> path, int toX, int toY, int x, int y)
        {
            if (toX + 1 < grid.Width && movementCosts[toX + 1, toY] < movementCosts[toX, toY])
            {
                if (path.Last() == null || path.Last().costToMove > grid.GridElements[toX + 1, toY].costToMove)
                {
                    path[path.Count - 1] = grid.GridElements[toX + 1, toY];

                    x = toX + 1;
                    y = toY;
                }
            }
            return (x,y);
        }

        private static (int,int) CheckLeftMouvement(Grid grid, int[,] movementCosts, List<Tile> path, int toX, int toY, int x, int y)
        {
            if (toX - 1 >= 0 && movementCosts[toX - 1, toY] < movementCosts[toX, toY])
            {
                if (path.Last() == null || path.Last().costToMove > grid.GridElements[toX - 1, toY].costToMove)
                {
                    path[path.Count - 1] = grid.GridElements[toX - 1, toY];

                    x = toX - 1;
                    y = toY;
                }
            }

            return (x,y);
        }


        public static List<Tile> GetPath(Grid grid, int[,] movementCosts, List<Tile> path, int fromX, int fromY,
            int toX, int toY)
        {
            List <Tile> pathInOrder = FindPath(grid, movementCosts, path, fromX, fromY, toX, toY);
            pathInOrder.Reverse();
            return pathInOrder;
        }
    }
}