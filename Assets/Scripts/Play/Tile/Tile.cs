using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    //Author: Jérémie Bertrand
    public abstract class Tile : MonoBehaviour
    {
        private Button tileButton;
        private readonly TileType tileType;
        private Image tileImage;
        private Character linkedCharacter;
        private GridController gridController;
        
        private bool IsPossibleAction => gridController.ACharacterIsCurrentlySelected && gridController.SelectedCharacter.CanPlay && tileImage.sprite != gridController.NormalSprite;
        private bool LinkedCharacterCanBeAttacked => IsOccupiedByACharacter && linkedCharacter is Enemy && IsPossibleAction;
        private bool LinkedCharacterCanBeSelected => IsOccupiedByACharacter && linkedCharacter is Ally && linkedCharacter.CanPlay;
        private bool IsWalkable => tileType != TileType.OBSTACLE;
        public bool IsAvailable => IsWalkable && !IsOccupiedByACharacter;
        private bool IsOccupiedByACharacter => linkedCharacter != null;
        private Vector2Int positionInGrid;
        public Vector3 WorldPosition => transform.position;
        public Vector2Int LogicalPosition => positionInGrid;
        public Character LinkedCharacter => linkedCharacter;

        protected Tile(TileType tileType)
        {
            this.tileType = tileType;
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
            
            if (LinkedCharacterCanBeSelected)
            {
                gridController.SelectCharacter(linkedCharacter); 
                gridController.DisplayPossibleActionsFrom(this);
                return;
            }
            
            if (LinkedCharacterCanBeAttacked)
            {
                gridController.SelectedCharacter.Attack(linkedCharacter);
                if (linkedCharacter.IsDead)
                {
                    linkedCharacter.Die();
                    gridController.SelectedCharacter.MoveTo(this);
                }
            }
            else if (IsPossibleAction)
            {
                gridController.SelectedCharacter.MoveTo(this);
            }
            gridController.DeselectCharacter();
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

        public bool LinkCharacter(Character character)
        {
            if (!IsWalkable) return false;
            this.linkedCharacter = character;
            return IsOccupiedByACharacter;
        }

        public bool UnlinkCharacter()
        {
            if (!IsOccupiedByACharacter) return false;
            linkedCharacter = null;
            return true;
        }
    }

    public enum TileType 
    {
        EMPTY = 0,
        OBSTACLE = 1,
        FOREST = 2,
        FORTRESS = 3,
        DOOR = 4
    }
}

