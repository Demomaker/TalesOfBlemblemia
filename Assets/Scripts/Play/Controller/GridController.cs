using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 namespace Game
 {
    //Authors: Jérémie Bertrand & Mike Bédard
    public class GridController : MonoBehaviour
    {
        private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private Sprite movementTileSprite = null;
        [SerializeField] private Sprite normalTileSprite = null;
        [SerializeField] private Sprite selectedTileSprite = null;
        [SerializeField] private Sprite attackableTileSprite = null;
        [SerializeField] private Sprite healableTileSprite = null;
        [SerializeField] private Sprite recruitableTileSprite = null;
        [SerializeField] private Sprite protectableTileSprite = null;
        [SerializeField] private Sprite verticalPathTileSprite = null;
        [SerializeField] private Sprite horizontalPathTileSprite = null;
        [SerializeField] private Sprite downToRightPathTileSprite = null;
        [SerializeField] private Sprite downToLeftPathTileSprite = null;
        [SerializeField] private Sprite upToRightPathTileSprite = null;
        [SerializeField] private Sprite upToLeftPathTileSprite = null;
        [SerializeField] private Sprite leftArrowPathTileSprite = null;
        [SerializeField] private Sprite rightArrowPathTileSprite = null;
        [SerializeField] private Sprite downArrowPathTileSprite = null;
        [SerializeField] private Sprite upArrowPathTileSprite = null;
        [SerializeField] private Sprite leftStartPathTileSprite = null;
        [SerializeField] private Sprite rightStartPathTileSprite = null;
        [SerializeField] private Sprite downStartPathTileSprite = null;
        [SerializeField] private Sprite upStartPathTileSprite = null;


        public Unit SelectedUnit
        {
            get; 
            private set;
        }
        public Sprite AvailabilitySprite => movementTileSprite;
        public Sprite NormalSprite => normalTileSprite;
        public Sprite SelectedSprite => selectedTileSprite;
        public Sprite AttackableTileSprite => attackableTileSprite;
        public Sprite HealableTileSprite => healableTileSprite;
        public Sprite RecruitableTileSprite => recruitableTileSprite;
        public Sprite ProtectableTileSprite => protectableTileSprite;
        public Sprite VerticalPathTileSprite => verticalPathTileSprite;
        public Sprite HorizontalPathTileSprite => horizontalPathTileSprite;
        public Sprite DownToRightPathTileSprite => downToRightPathTileSprite;
        public Sprite DownToLeftPathTileSprite => downToLeftPathTileSprite;
        public Sprite UpToRightPathTileSprite => upToRightPathTileSprite;
        public Sprite UpToLeftPathTileSprite => upToLeftPathTileSprite;
        public Sprite LeftArrowPathTileSprite => leftArrowPathTileSprite;
        public Sprite RightArrowPathTileSprite => rightArrowPathTileSprite;
        public Sprite DownArrowPathTileSprite => downArrowPathTileSprite;
        public Sprite UpArrowPathTileSprite => upArrowPathTileSprite;
        public Sprite LeftStartPathTileSprite => leftStartPathTileSprite;
        public Sprite RightStartPathTileSprite => rightStartPathTileSprite;
        public Sprite DownStartPathTileSprite => downStartPathTileSprite;
        public Sprite UpStartPathTileSprite => upStartPathTileSprite;

        public bool AUnitIsCurrentlySelected => SelectedUnit != null;

        public int NbColumns { get; private set; } = 0;
        public int NbLines { get; private set; } = 0;
        
        private void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            NbColumns = (int)(GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
            NbLines = (int)(GetComponent<RectTransform>().rect.height / gridLayoutGroup.cellSize.y);
        }

        public void SelectUnit(Unit unit)
        {
            if(SelectedUnit != null) DeselectUnit();
            SelectedUnit = unit;
        }

        public void DeselectUnit()
        {
            SelectedUnit = null;
            foreach (Transform child in transform)
            {
                child.GetComponent<Tile>().HideActionPossibility();
            }
            RemoveActionPath();
        }
        public void RemoveActionPath()
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Tile>().HideActionPath();
            }
        }
        
        public void DisplayPossibleActionsFrom(Tile fromTile)
        {
            fromTile.DisplaySelectedTile();
            var playerUnit = fromTile.LinkedUnit;
            var movementCosts = playerUnit.MovementCosts;
            for (int i = 0; i < movementCosts.GetLength(0); i++)
            {
                for (int j = 0; j < movementCosts.GetLength(1); j++)
                {
                    if (movementCosts[i, j] > 0)
                    {
                        var tile = GetTile(i, j);
                        if (tile.IsAvailable && movementCosts[i, j] <= playerUnit.MovesLeft)
                        {
                            tile.DisplayMoveActionPossibility();
                        }
                        else if (tile.LinkedUnit != null && playerUnit.TargetIsInMovementRange(tile.LinkedUnit))
                        {
                            if (playerUnit.WeaponType != WeaponType.HealingStaff && (tile.LinkedUnit.IsEnemy))
                            {
                                tile.DisplayAttackActionPossibility();
                            }
                            else if (tile.LinkedUnit.IsRecruitable)
                            {
                                tile.DisplayRecruitActionPossibility();
                            }
                            else if (playerUnit.WeaponType == WeaponType.HealingStaff && !tile.LinkedUnit.IsEnemy)
                            {
                                tile.DisplayHealActionPossibility();
                            }
                        }
                        else if (tile.LinkedDoor != null && tile.LinkedDoor.isActiveAndEnabled)
                        {
                            if (tile.LinkedDoor.IsEnemyTarget)
                                tile.DisplayProtectable();
                            else if (playerUnit.TargetIsInMovementRange(tile.LinkedDoor))
                                tile.DisplayAttackActionPossibility();
                        }
                    }
                }
            }
        }
        
        public Tile FindAvailableAdjacentTile(Tile tile, Unit unit)
        {
            int tilePosX = tile.LogicalPosition.x;
            int tilePosY = tile.LogicalPosition.y;
            
            var unitMovesLeft = unit.MovesLeft;
            var movementCosts = unit.MovementCosts;
            
            //Check left
            Tile tileToCheck = Finder.GridController.GetTile(tilePosX - 1, tilePosY);
            if (tileToCheck != null && tileToCheck.IsAvailable && movementCosts[tilePosX - 1, tilePosY] <= unitMovesLeft)
                return tileToCheck;
            //Check up
            tileToCheck = Finder.GridController.GetTile(tilePosX, tilePosY - 1);
            if (tileToCheck != null && tileToCheck.IsAvailable && movementCosts[tilePosX, tilePosY - 1] <= unitMovesLeft)
                return tileToCheck;
            //Check right
            tileToCheck = Finder.GridController.GetTile(tilePosX + 1, tilePosY);
            if (tileToCheck != null && tileToCheck.IsAvailable && movementCosts[tilePosX + 1, tilePosY] <= unitMovesLeft)
                return tileToCheck;
            //Check down
            tileToCheck = Finder.GridController.GetTile(tilePosX, tilePosY + 1);
            if (tileToCheck != null && tileToCheck.IsAvailable && movementCosts[tilePosX, tilePosY + 1] <= unitMovesLeft)
                return tileToCheck;
            return null;
        }

        public Tile GetTile(int x, int y)
        {
            if (IsValidGridPosition(x, y))
            {
                return transform.GetChild(x + y * NbColumns).GetComponent<Tile>();
            }

            return null;
        }
        
        public bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < NbColumns && y < NbLines;
        }

        public void DisplayAction(Action unitTurnAction, Unit playerUnit)
        {
            var completePath = new List<Tile>();
            completePath.Add(playerUnit.CurrentTile);
            completePath.AddRange(unitTurnAction.Path);
            int prevIndex = -1;
            int nextIndex = -1;
            var pathCount = completePath.Count;
            for (int i = 0; i < pathCount; i++)
            {
                prevIndex = i - 1;
                if (prevIndex < 0) prevIndex = 0;
                nextIndex = i + 1;
                if (nextIndex >= pathCount) nextIndex = pathCount - 1;
                completePath[i].DisplayPathPossibility(completePath[prevIndex], completePath[nextIndex]);
            }
                
            var target = unitTurnAction.Target;
            if (unitTurnAction.Target != null)
            {
                if (target is Unit && playerUnit.WeaponType == WeaponType.HealingStaff)
                    target.CurrentTile.DisplayHealActionPossibility();
                else
                    target.CurrentTile.DisplayAttackActionPossibility();
            }
        }
    }
 }
