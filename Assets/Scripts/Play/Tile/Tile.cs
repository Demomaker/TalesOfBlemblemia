using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    // <summary>
    /// Behaviour of a tile
    /// Author: Jérémie Bertrand
    /// </summary>
    public abstract class Tile : MonoBehaviour
    {
        private Button tileButton;
        [SerializeField] private TileType tileType;
        public TileType TileType => tileType;

        private Image tileImage;
        private Unit linkedUnit;
        private GridController gridController;
        public GridController GridController => gridController;

        public bool IsPossibleAction => gridController.AUnitIsCurrentlySelected && !gridController.SelectedUnit.HasActed && tileImage.sprite != gridController.NormalSprite;
        public bool LinkedUnitCanBeAttacked => IsOccupiedByAUnit && linkedUnit.IsEnemy && IsPossibleAction;
        public bool LinkedUnitCanBeSelected => IsOccupiedByAUnit && !linkedUnit.IsEnemy && !linkedUnit.HasActed;
        private bool IsWalkable => tileType != TileType.Obstacle;
        public bool IsAvailable => IsWalkable && !IsOccupiedByAUnit;
        private bool IsOccupiedByAUnit => linkedUnit != null;
        private Vector2Int positionInGrid;
        public Vector3 WorldPosition => transform.position;
        public Vector2Int LogicalPosition => positionInGrid;
        public Unit LinkedUnit => linkedUnit;
        private readonly int costToMove;

        public int CostToMove => costToMove;
        private readonly float defenseRate;
        public float DefenseRate => defenseRate;
        

        protected Tile(TileType tileType, int costToMove = TileValues.DEFAULT_COST_TO_MOVE, float defenseRate = TileValues.DEFAULT_DEFENSE_RATE)
        {
            this.tileType = tileType;
            this.costToMove = costToMove;
            this.defenseRate = defenseRate;
        }
        
        protected virtual void Awake()
        {
            tileButton = GetComponent<Button>();
            tileImage = GetComponent<Image>();
            gridController = transform.parent.GetComponent<GridController>();
        }

        protected void Start()
        {
            int index = transform.GetSiblingIndex();
            positionInGrid.x = index % Finder.GridController.NbColumns;
            positionInGrid.y = index / Finder.GridController.NbLines;
        }

        public void DisplayMoveActionPossibility()
        {
            tileImage.sprite = gridController.AvailabilitySprite;
        }

        public void DisplaySelectedTile()
        {
            tileImage.sprite = gridController.HealableTileSprite;
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
            if (!IsWalkable) 
                return false;
            this.linkedUnit = unit;
            Harmony.Finder.LevelController.ReevaluateAllMovementCosts();
            return IsOccupiedByAUnit;
        }

        public bool UnlinkUnit()
        {
            if (!IsOccupiedByAUnit) return false;
            linkedUnit = null;
            return true;
        }

        /// <summary>
        /// Verifies if a tile is adjacent on a X or Y axis to this tile
        /// Authors: Jérémie Bertrand, Zacharie Lavigne
        /// </summary>
        /// <param name="otherTile">The other tile to verify adjacency</param>
        /// <param name="range">The threshold of adjacency</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The range must be greater than 0</exception>
        public bool IsWithinRange(Tile otherTile, int range)
        {
            if (range <= 0)
                throw  new ArgumentException("Range should be higher than zero");
            return Math.Abs(this.LogicalPosition.x - otherTile.LogicalPosition.x) + Math.Abs(this.LogicalPosition.y - otherTile.LogicalPosition.y) <= range;
        }
    }
}

