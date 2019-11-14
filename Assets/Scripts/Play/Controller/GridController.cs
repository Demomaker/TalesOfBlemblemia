using UnityEngine;
using UnityEngine.UI;

 namespace Game
 {
    //Authors: Jérémie Bertrand & Mike Bédard
    public class GridController : MonoBehaviour
    {
        //BC : Propreté. Attributs et "SerializedField" sont mélangés.
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

        //BR : Débutez les noms de propriétés "booleans" par "Is" ou "Has.
        //     Ici, ça donne "IsUnitSelected". Simple non ?
        public bool AUnitIsCurrentlySelected => SelectedUnit != null;

        public int NbColumns { get; private set; } = 0;
        public int NbLines { get; private set; } = 0;
        
        private void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            //BC : Appels répétés à "GetComponent<RectTransform>()".
            NbColumns = (int)(GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
            NbLines = (int)(GetComponent<RectTransform>().rect.height / gridLayoutGroup.cellSize.y);
        }

        public void SelectUnit(Unit unit)
        {
            if(SelectedUnit != null) DeselectUnit();
            //BC : Ici, vous auriez du appeller "DisplayPossibleActionsFrom", car chaque appel à "SelectUnit"
            //     est suivi d'un appel à "DisplayPossibleActionsFrom".
            //     C'est aussi plus cohérent avec le "DeselectUnit".
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
            var linkedUnit = fromTile.LinkedUnit;
            var movementCosts = linkedUnit.MovementCosts;
            for (int i = 0; i < movementCosts.GetLength(0); i++)
            {
                for (int j = 0; j < movementCosts.GetLength(1); j++)
                {
                    //BR : Accès répété à la même cellule de tableau. Super lent.
                    //     Voir trois lignes plus bas. 
                    if (movementCosts[i, j] > 0)
                    {
                        var tile = GetTile(i, j);
                        if (tile.IsAvailable && movementCosts[i, j] <= linkedUnit.MovesLeft)
                        {
                            tile.DisplayMoveActionPossibility();
                        }
                        else if (tile.LinkedUnit != null && linkedUnit.TargetIsInMovementRange(tile.LinkedUnit))
                        {
                            if (linkedUnit.WeaponType != WeaponType.HealingStaff && (tile.LinkedUnit.IsEnemy || tile.IsOccupiedByADoor))
                            {
                                tile.DisplayAttackActionPossibility();
                            }
                            else if (tile.LinkedUnit.IsRecruitable)
                            {
                                tile.DisplayRecruitActionPossibility();
                            }
                            else if (linkedUnit.WeaponType == WeaponType.HealingStaff && !tile.LinkedUnit.IsEnemy)
                            {
                                tile.DisplayHealActionPossibility();
                            }
                        }
                        else if (tile.LinkedDoor != null)
                        {
                            if (!tile.LinkedDoor.IsEnemyTarget)
                                tile.DisplayAttackActionPossibility();
                            else
                                tile.DisplayProtectable();
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
