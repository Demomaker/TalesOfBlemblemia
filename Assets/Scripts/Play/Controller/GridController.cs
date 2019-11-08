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
        public Unit SelectedUnit { get; private set; } = null;
        public Sprite AvailabilitySprite => movementTileSprite;
        public Sprite NormalSprite => normalTileSprite;
        public Sprite SelectedSprite => selectedTileSprite;
        public Sprite AttackableTileSprite => attackableTileSprite;
        public Sprite HealableTileSprite => healableTileSprite;
        public Sprite RecruitableTileSprite => recruitableTileSprite;
        public Sprite ProtectableTileSprite => protectableTileSprite;

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
    }
 }
