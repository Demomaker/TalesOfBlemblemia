using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class EnemyRangeController
    {
        private static List<Tile> InRangeTiles = new List<Tile>();
        private static bool isEnabled;
        private static int tileUpdateKeeper = -1;

        public static void EnableEnemyRange(List<Unit> enemyUnits)
        {
            if (!isEnabled)
            {
                isEnabled = true;
                SetEnemyRange(enemyUnits);
            }
        }

        private static void SetEnemyRange(List<Unit> enemyUnits)
        {
            if (tileUpdateKeeper != Harmony.Finder.LevelController.LevelTileUpdateKeeper)
            {
                FindInRangeTiles(enemyUnits);
            }
            DisplayEnemyRange();
        }

        public static void DisplayEnemyRange()
        {
            if (isEnabled)
            {
                foreach (var tile in InRangeTiles)
                {
                    tile.DisplayEnemyRange();
                }
            }
        }

        private static void FindInRangeTiles(List<Unit> enemyUnits)
        {
            tileUpdateKeeper = Harmony.Finder.LevelController.LevelTileUpdateKeeper;
            var grid = Finder.GridController;
            for (int i = 0; i < grid.NbColumns ; i++)
            {
                for (int j = 0; j < grid.NbLines; j++)
                {
                    var tile = grid.GetTile(i, j);
                    if (TileIsReachableByEnemy(tile.LogicalPosition, enemyUnits))
                    {
                        InRangeTiles.Add(tile);
                    }
                }
            }
        }

        private static bool TileIsReachableByEnemy(Vector2Int tilePos, List<Unit> enemyUnits)
        {
            foreach (var enemy in enemyUnits)
            {
                if (enemy.MovementCosts[tilePos.x, tilePos.y] <= enemy.Stats.MoveSpeed)
                {
                    return true;
                }
            }
            return false;
        }

        public static void DisableEnemyRange()
        {
            isEnabled = false;
            HideEnemyRange();
        }

        private static void HideEnemyRange()
        {
            tileUpdateKeeper = -1;
            while (InRangeTiles.Count > 0)
            {
                var tile = InRangeTiles[0];
                tile.HideEnemyRange();
                InRangeTiles.Remove(tile);
            }
        }


        public static void OnComputerTurn()
        {
            HideEnemyRange();
        }

        public static void OnPlayerTurn(List<Unit> ownedUnits)
        {
            if (isEnabled)
            {
                SetEnemyRange(ownedUnits);
            }
        }
    }
}