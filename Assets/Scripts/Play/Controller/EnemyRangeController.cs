using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    //Author : Zacharie Lavigne
    [Findable("EnemyRangeCont")]
    public class EnemyRangeController : MonoBehaviour
    {
        private List<Tile> InRangeTiles = new List<Tile>();
        private bool isActivated;
        private int tileUpdateKeeper = -1;
        private LevelController levelController;
        private GridController gridController;

        private void Awake()
        {
            levelController = Harmony.Finder.LevelController;
            gridController = Harmony.Finder.GridController;
        }

        private void EnableEnemyRange(List<Unit> enemyUnits)
        {
            if (!isActivated)
            {
                isActivated = true;
                SetEnemyRange(enemyUnits);
            }
            DisplayEnemyRange();
        }
        private void DisableEnemyRange()
        {
            isActivated = false;
            HideEnemyRange();
        }
        
        private void SetEnemyRange(List<Unit> enemyUnits)
        {
            if (tileUpdateKeeper != levelController.LevelTileUpdateKeeper)
                FindInRangeTiles(enemyUnits);
        }

        public void DisplayEnemyRange()
        {
            if (isActivated)
            {
                foreach (var tile in InRangeTiles)
                {
                    tile.DisplayEnemyRange();
                }
            }
        }

        private void FindInRangeTiles(List<Unit> enemyUnits)
        {
            tileUpdateKeeper = levelController.LevelTileUpdateKeeper;
            for (int i = 0; i < gridController.NbColumns ; i++)
            {
                for (int j = 0; j < gridController.NbLines; j++)
                {
                    var tile = gridController.TileArray[i, j];
                    if (TileIsReachableByEnemy(tile.LogicalPosition, enemyUnits))
                    {
                        InRangeTiles.Add(tile);
                    }
                }
            }
        }

        private static bool TileIsReachableByEnemy(Vector2Int tilePos, IEnumerable<Unit> enemyUnits)
        {
            return enemyUnits.Any(enemy => enemy.MovementCosts[tilePos.x, tilePos.y] <= enemy.Stats.MoveSpeed);
        }
        
        private void HideEnemyRange()
        {
            tileUpdateKeeper = -1;
            while (InRangeTiles.Count > 0)
            {
                var tile = InRangeTiles[0];
                tile.HideEnemyRange();
                InRangeTiles.Remove(tile);
            }
        }
        
        [UsedImplicitly]
        public void OnToggleChange(Toggle enemyRangeToggle)
        {
            if (enemyRangeToggle.isOn)
                EnableEnemyRange(levelController.ComputerPlayer.OwnedUnits);
            else
                DisableEnemyRange();
        }
        public void OnComputerTurn()
        {
            HideEnemyRange();
        }
        public void OnPlayerTurn(List<Unit> ownedUnits)
        {
            if (isActivated)
            {
                SetEnemyRange(ownedUnits);
                DisplayEnemyRange();
            }
        }

        private void Update()
        {
            if (levelController.LevelTileUpdateKeeper > tileUpdateKeeper && isActivated && levelController.CurrentPlayer is HumanPlayer)
            {
                HideEnemyRange();
                FindInRangeTiles(levelController.ComputerPlayer.OwnedUnits);
                DisplayEnemyRange();
            }
        }
        
    }
}