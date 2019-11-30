using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Behaviour of a tile
    /// Author: Jérémie Bertrand, Zacharie Lavigne
    /// </summary>
    public abstract class Tile : MonoBehaviour
    {
        [SerializeField] private TileType tileType;
        private TileSprite tileSprite;

        private Image tileImage;
        private SpriteRenderer tilePathSprite;
        private Unit linkedUnit;
        private Door linkedDoor;
        private GridController gridController;
        private UIController uiController;
        private ClickType leftClickType = ClickType.None;
        private ClickType rightClickType = ClickType.None;
        private Vector2Int positionInGrid;
        private EnemyRangeController enemyRangeController;
        private PlayerClickManager playerClickManager;

        public TileType TileType => tileType;
        public ClickType LeftClickType => leftClickType;
        public ClickType RightClickType => rightClickType;
        public GridController GridController => gridController;
        public bool IsPossibleAction => gridController.AUnitIsCurrentlySelected && !gridController.SelectedUnit.HasActed && tileImage.sprite != gridController.NormalSprite;
        public bool LinkedUnitCanBeAttackedByPlayer => IsOccupiedByAUnit && linkedUnit.IsEnemy && IsPossibleAction;
        public bool LinkedDoorCanBeAttackedByPlayer => IsOccupiedByADoor && IsPossibleAction && !linkedDoor.IsEnemyTarget;
        public bool LinkedUnitCanBeRecruitedByPlayer => IsOccupiedByAUnit && linkedUnit.IsRecruitable && IsPossibleAction;
        public bool LinkedUnitCanBeHealedByPlayer => IsOccupiedByAUnit && !linkedUnit.IsEnemy && IsPossibleAction;
        public bool LinkedUnitCanBeSelectedByPlayer => IsOccupiedByAUnit && linkedUnit.IsPlayer && !linkedUnit.HasActed;
        public bool IsWalkable => tileType != TileType.Obstacle;
        public bool IsAvailable => IsWalkable && !IsOccupiedByAUnitOrDoor;
        public bool IsOccupiedByAUnitOrDoor => IsOccupiedByAUnit || IsOccupiedByADoor;
        public bool IsOccupiedByAUnit => linkedUnit != null && linkedUnit.isActiveAndEnabled;
        public bool IsOccupiedByADoor => linkedDoor != null && linkedDoor.isActiveAndEnabled;
        public Vector3 WorldPosition => transform.position;
        public Vector2Int LogicalPosition => positionInGrid;
        public Unit LinkedUnit => linkedUnit;
        public Door LinkedDoor => linkedDoor;
        public Targetable LinkedTargetable => linkedDoor != null ? (Targetable)linkedDoor : linkedUnit;
        public int CostToMove => IsOccupiedByADoor ? TileTypeExt.HIGH_COST_TO_MOVE : tileType.GetCostToMove();
        public float DefenseRate => tileType.GetDefenseRate();
        
        protected Tile(TileType tileType)
        {
            this.tileType = tileType;
        }
        
        protected virtual void Awake()
        {
            tileImage = GetComponent<Image>();
            tileSprite = GetComponent<TileSprite>();
            tilePathSprite = GetComponentInChildren<SpriteRenderer>();
            gridController = transform.parent.GetComponent<GridController>();
            uiController = Harmony.Finder.UIController;
            enemyRangeController = Harmony.Finder.EnemyRangeController;
            playerClickManager = Harmony.Finder.PlayerClickManager;
        }

        protected void Start()
        {
            var index = transform.GetSiblingIndex();
            var nbColumns = gridController.NbColumns;
            positionInGrid.x = index % nbColumns;
            positionInGrid.y = index / nbColumns;
        }

        public void DisplayMoveActionPossibility()
        {
            tileImage.sprite = gridController.AvailabilitySprite;
        }

        public Sprite GetSprite()
        {
            return tileSprite.GetSprite();
        }

        public void DisplaySelectedTile()
        {
            tileImage.sprite = gridController.SelectedSprite;
        }

        public void DisplayEnemyRange()
        {
             tilePathSprite.sprite = gridController.EnemyRangeSprite;
        }

        public void DisplayAttackActionPossibility()
        {
            tileImage.sprite = gridController.AttackableTileSprite;
        }

        public void DisplayRecruitActionPossibility()
        {
            tileImage.sprite = gridController.RecruitableTileSprite;
        }

        public void DisplayHealActionPossibility()
        {
            tileImage.sprite = gridController.HealableTileSprite;
        }

        public void DisplayProtectable()
        {
            tileImage.sprite = gridController.ProtectableTileSprite;
        }

        public void HideActionPossibility()
        {
            tileImage.sprite = gridController.NormalSprite;
        }

        public void HideActionPath()
        {
            if (tilePathSprite.sprite != gridController.EnemyRangeSprite)
                tilePathSprite.sprite = gridController.NormalSprite;
            enemyRangeController.DisplayEnemyRange();
        }

        public void HideEnemyRange()
        {
            if (tilePathSprite.sprite == gridController.EnemyRangeSprite)
                tilePathSprite.sprite = gridController.NormalSprite;
        }

        /// <summary>
        /// Verifies if a tile is adjacent on a X or Y axis to this tile
        /// Authors: Jérémie Bertrand, Zacharie Lavigne
        /// </summary>
        /// <param name="otherTile">The other tile to verify adjacency</param>
        /// <returns>Return if target is within melee range</returns>
        public bool IsWithinRange(Tile otherTile)
        {
            return Math.Abs(LogicalPosition.x - otherTile.LogicalPosition.x) + Math.Abs(LogicalPosition.y - otherTile.LogicalPosition.y) <= 1;
        }

        public void MakeObstacle()
        {
            tileType = TileType.Obstacle;
        }

        public void UnMakeObstacle(TileType previousType)
        {
            tileType = previousType;
        }

        public void OnCursorEnter()
        {
            uiController.UpdateTileInfos(this);
            UpdateClickHint();
        }

        public void UpdateClickHint()
        {
            leftClickType = ClickType.None;
            rightClickType = ClickType.None;
            if (playerClickManager.ActionIsSet && this == playerClickManager.TileToConfirm)
            {
                switch (playerClickManager.UnitTurnAction.ActionType)
                {
                    case ActionType.Rest:
                        rightClickType = leftClickType = ClickType.ConfirmRest;
                        break;
                    case ActionType.Attack:
                        rightClickType = leftClickType = ClickType.ConfirmAttack;
                        break;
                    case ActionType.Nothing:
                        rightClickType = leftClickType = ClickType.ConfirmMoveTo;
                        break;
                    case ActionType.Recruit:
                        rightClickType = leftClickType = ClickType.ConfirmRecruit;
                        break;
                    case ActionType.Heal:
                        rightClickType = leftClickType = ClickType.ConfirmHeal;
                        break;
                }
            }
            else
            {
                if (LinkedUnitCanBeSelectedByPlayer && gridController.SelectedUnit == linkedUnit)
                {
                    leftClickType = ClickType.Deselect;
                    rightClickType = ClickType.Rest;
                }
                else if (LinkedUnitCanBeSelectedByPlayer) leftClickType = rightClickType = ClickType.Select;
                else if (LinkedDoorCanBeAttackedByPlayer || LinkedUnitCanBeAttackedByPlayer) rightClickType = ClickType.Attack;
                else if (IsPossibleAction) {
                    leftClickType = ClickType.MoveTo;
                    rightClickType = ClickType.Rest;
                } 
                else if (gridController.AUnitIsCurrentlySelected) leftClickType = rightClickType = ClickType.Deselect;
                if (LinkedUnitCanBeRecruitedByPlayer) rightClickType = ClickType.Attack;
                if (LinkedUnitCanBeHealedByPlayer && gridController.SelectedUnit != linkedUnit) rightClickType = ClickType.Heal;
                if (LinkedUnitCanBeRecruitedByPlayer) rightClickType = ClickType.Recruit;
            }
            uiController.UpdateLeftClickHint(leftClickType);
            uiController.UpdateRightClickHint(rightClickType);
        }

        public void LinkTargetable(Targetable targetable)
        {
            if (targetable.GetType() == typeof(Unit))
                LinkUnit((Unit) targetable);
            else if (targetable.GetType() == typeof(Door))
                LinkDoor((Door) targetable);
            UpdateClickHint();
        }

        private void LinkUnit(Unit unit)
        {
            if (IsWalkable) linkedUnit = unit;
        }
        public void UnlinkUnit()
        {
            linkedUnit = null;
            UpdateClickHint();
        }
        
        private void LinkDoor(Door door)
        {
            if (IsWalkable) linkedDoor = door;
        }
        public void UnlinkDoor()
        {
            linkedDoor = null;
            UpdateClickHint();
        }

        public void DisplayPathPossibility(Tile prevTile, Tile nextTile)
        {
            tilePathSprite.sprite = FindAppropriatePathSprite(prevTile, nextTile);
        }

        private Sprite FindAppropriatePathSprite(Tile prevTile, Tile nextTile)
        {
            var prevPosition = prevTile.LogicalPosition;
            var nextPosition = nextTile.LogicalPosition;
            var currentPosition = this.LogicalPosition;

            //Starting pos
            if (prevPosition == currentPosition)
                return FindAppropriateStartingPathSprite(currentPosition, nextPosition);

            //Mid path
            else if (prevPosition != currentPosition && nextPosition != currentPosition)
                return FindAppropriateMidPathSprite(prevPosition, currentPosition, nextPosition);

            //End path
            else if (currentPosition == nextPosition)
                return FindAppropriateEndPathSprite(prevPosition, currentPosition);
            return null;
        }

        private Sprite FindAppropriateEndPathSprite(Vector2Int prevPosition, Vector2Int currentPosition)
        {
            //Comes from the right (Goes left)
            if (prevPosition.x > currentPosition.x)
                return gridController.LeftArrowPathTileSprite;
            //Comes from the left (Goes right)
            else if (prevPosition.x < currentPosition.x)
                return gridController.RightArrowPathTileSprite;
            //Comes from below (Goes up)
            else if (prevPosition.y > currentPosition.y)
                return gridController.UpArrowPathTileSprite;
            //Comes from above (Goes down)
            else if (prevPosition.y < currentPosition.y) 
                return gridController.DownArrowPathTileSprite;
            return null;
        }

        private Sprite FindAppropriateMidPathSprite(Vector2Int prevPosition, Vector2Int currentPosition, Vector2Int nextPosition)
        {
            //Goes horizontal
            if (prevPosition.y == currentPosition.y && currentPosition.y == nextPosition.y)
                return gridController.HorizontalPathTileSprite;
            //Goes vertical
            if (prevPosition.x == currentPosition.x && currentPosition.x == nextPosition.x)
                return gridController.VerticalPathTileSprite;
            //Comes from the left
            if (prevPosition.x < LogicalPosition.x)
            {
                //Goes up
                if (currentPosition.y > nextPosition.y)
                    return gridController.UpToLeftPathTileSprite;
                //Goes down
                if (currentPosition.y < nextPosition.y)
                    return gridController.DownToLeftPathTileSprite;
            }
            //Comes from the right
            else if (prevPosition.x > currentPosition.x)
            {
                //Goes up
                if (currentPosition.y > nextPosition.y)
                    return gridController.UpToRightPathTileSprite;
                //Goes down
                if (LogicalPosition.y < nextPosition.y)
                    return gridController.DownToRightPathTileSprite;
            }
            //Comes from above
            if (prevPosition.y < LogicalPosition.y)
            {
                //Goes right
                if (currentPosition.x < nextPosition.x)
                    return gridController.UpToRightPathTileSprite;
                //Goes left
                if (currentPosition.x > nextPosition.x)
                    return gridController.UpToLeftPathTileSprite;
            }
            //Comes from below
            else if (prevPosition.y > currentPosition.y)
            {
                //Goes right
                if (currentPosition.x < nextPosition.x)
                    return gridController.DownToRightPathTileSprite;
                //Goes left
                if (currentPosition.x > nextPosition.x)
                    return gridController.DownToLeftPathTileSprite;
            }
            return null;
        }

        private Sprite FindAppropriateStartingPathSprite(Vector2Int currentPosition, Vector2Int nextPosition)
        {
            //Goes left
            if (currentPosition.x > nextPosition.x)
                return gridController.LeftStartPathTileSprite;
            //Goes right)
            else if (currentPosition.x < nextPosition.x)
                return gridController.RightStartPathTileSprite;
            //Goes up
            else if (currentPosition.y > nextPosition.y)
                return gridController.UpStartPathTileSprite;
            //Comes from above (Goes down)
            else if (currentPosition.y < nextPosition.y) 
                return gridController.DownStartPathTileSprite;
            return null;
        }

        public IEnumerator Blink(Sprite blinkSprite)
        {
            var fadeIn = false;
            var fadeValue = 1f;
            tileImage.sprite = blinkSprite;
            var faded = tileImage.color;
            while (true)
            {
                if (tileImage.sprite != blinkSprite) tileImage.sprite = blinkSprite;
                fadeValue += fadeIn ? 0.01f : -0.01f;

                faded.a = fadeValue;
                tileImage.color = faded;
                if (fadeValue <= 0f) fadeIn = true;
                if (fadeValue >= 1f) fadeIn = false;
                yield return null;
            }

        }

        public void ResetTileImage()
        {
            tileImage.sprite = gridController.NormalSprite;
            var color = tileImage.color;
            color = new Color(color.r, color.g, color.b);
            tileImage.color = color;
        }
    }
}

