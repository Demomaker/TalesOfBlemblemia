using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    //Author: Jérémie Bertrand
    public abstract class Tile : MonoBehaviour
    {
        private Button tileButton;
        [SerializeField] private TileType tileType;
        public TileType TileType => tileType;

        private Image tileImage;
        private Unit linkedUnit;
        private GridController gridController;
        
        private bool IsPossibleAction => gridController.AUnitIsCurrentlySelected && gridController.SelectedUnit.CanPlay && tileImage.sprite != gridController.NormalSprite;
        private bool LinkedUnitCanBeAttacked => IsOccupiedByAUnit && linkedUnit.IsEnemy && IsPossibleAction;
        private bool LinkedUnitCanBeSelected => IsOccupiedByAUnit && !linkedUnit.IsEnemy && linkedUnit.CanPlay;
        private bool IsWalkable => tileType != TileType.Obstacle;
        public bool IsAvailable => IsWalkable && !IsOccupiedByAUnit;
        private bool IsOccupiedByAUnit => linkedUnit != null;
        private Vector2Int positionInGrid;
        public Vector3 WorldPosition => transform.position;
        public Vector2Int LogicalPosition => positionInGrid;
        public Unit LinkedUnit => linkedUnit;
        private int costToMove;

        public int CostToMove => costToMove;
        

            protected Tile(TileType tileType, int costToMove = 1)
        {
            this.tileType = tileType;
            this.costToMove = costToMove;
        }
        
        protected virtual void Awake()
        {
            tileButton = GetComponent<Button>();
            tileImage = GetComponent<Image>();
            tileButton.onClick.AddListener(OnCellClick); 
            gridController = transform.parent.GetComponent<GridController>();
        }

        protected void Start()
        {
            int index = transform.GetSiblingIndex();
            positionInGrid.x = index % Finder.GridController.NbColumns;
            positionInGrid.y = index / Finder.GridController.NbLines;
        }

        private void OnCellClick()
        {
            EventSystem.current.SetSelectedGameObject(null);
            
            if (LinkedUnitCanBeSelected)
            {
                gridController.SelectUnit(linkedUnit); 
                gridController.DisplayPossibleActionsFrom(this);
                return;
            }
            
            if (LinkedUnitCanBeAttacked)
            {
                gridController.SelectedUnit.Attack(linkedUnit);
                if (linkedUnit.IsDead)
                {
                    linkedUnit.Die();
                    gridController.SelectedUnit.MoveTo(this);
                }
            }
            else if (IsPossibleAction)
            {
                gridController.SelectedUnit.MoveTo(this);
            }
            gridController.DeselectUnit();
        }

        public void DisplayMoveActionPossibility()
        {
            tileImage.sprite = gridController.AvailabilitySprite;
        }

        public void DisplaySelectedTile()
        {
            tileImage.sprite = gridController.SelectedSprite;
        }

        public void DisplayAttackActionPossibility()
        {
            tileImage.sprite = gridController.AttackableTileSprite;
        }
        
        public void HideActionPossibility()
        {
            tileImage.sprite = gridController.NormalSprite;
        }

        public bool LinkUnit(Unit unit)
        {
            if (!IsWalkable) return false;
            this.linkedUnit = unit;
            return IsOccupiedByAUnit;
        }

        public bool UnlinkUnit()
        {
            if (!IsOccupiedByAUnit) return false;
            linkedUnit = null;
            return true;
        }
    }

    public enum TileType 
    {
        Empty = 0,
        Obstacle = 1,
        Forest = 2,
        Fortress = 3,
        Door = 4
    }
}

