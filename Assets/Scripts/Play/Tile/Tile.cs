﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

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
        private TileSprite tileSprite;

        private Image tileImage;
        private Unit linkedUnit;
        private Door linkedDoor;
        private GridController gridController;
        public GridController GridController => gridController;
        private readonly int costToMove;
        private readonly float defenseRate;
        
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
        private Vector2Int positionInGrid;
        public Vector3 WorldPosition => transform.position;
        public Vector2Int LogicalPosition => positionInGrid;
        public Unit LinkedUnit => linkedUnit;
        public Door LinkedDoor => linkedDoor;
        public int CostToMove
        {
            get
            {
                if (tileType == TileType.Obstacle || IsOccupiedByADoor)
                {
                    return int.MaxValue;
                }

                return costToMove;
            }
        }
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
            tileSprite = GetComponent<TileSprite>();
            gridController = transform.parent.GetComponent<GridController>();
        }

        protected void Start()
        {
            int index = transform.GetSiblingIndex();
            positionInGrid.x = index % Finder.GridController.NbColumns;
            positionInGrid.y = index / Finder.GridController.NbColumns;
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
            Harmony.Finder.UIController.ModifyPlayerUi(this);
        }

        public void LinkTargetable(Targetable targetable)
        {
            if (targetable.GetType() == typeof(Unit))
                LinkUnit((Unit) targetable);
            else if (targetable.GetType() == typeof(Door))
                LinkDoor((Door) targetable);
        }

        public bool LinkUnit(Unit unit)
        {
            if (!IsWalkable) 
                return false;
            this.linkedUnit = unit;
            return IsOccupiedByAUnitOrDoor;
        }
        public bool UnlinkUnit()
        {
            if (!IsOccupiedByAUnitOrDoor) return false;
            linkedUnit = null;
            return true;
        }
        
        public bool LinkDoor(Door door)
        {
            if (!IsWalkable) 
                return false;
            this.linkedDoor = door;
            return IsOccupiedByAUnitOrDoor;
        }
        public bool UnlinkDoor()
        {
            if (!IsOccupiedByAUnitOrDoor) return false;
            linkedUnit = null;
            return true;
        }

        public IEnumerator Blink(Sprite blinkSprite)
        {
            const bool isBlinking = true;
            var fadeIn = false;
            var fadeValue = 1f;
            tileImage.sprite = blinkSprite;
            var faded = tileImage.color;
            while (isBlinking)
            {
                if (tileImage.sprite != blinkSprite) tileImage.sprite = blinkSprite;
                if (fadeIn) fadeValue+=0.01f;
                else
                {
                    fadeValue-=0.01f;
                }

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
            tileImage.color = new Color(tileImage.color.r, tileImage.color.g, tileImage.color.b, 1f);
        }
    }
}

