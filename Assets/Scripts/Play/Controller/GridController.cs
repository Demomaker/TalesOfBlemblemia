﻿using System;
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

        public Unit SelectedUnit { get; private set; } = null;
        public Sprite AvailabilitySprite => movementTileSprite;
        public Sprite NormalSprite => normalTileSprite;
        public Sprite SelectedSprite => selectedTileSprite;
        public Sprite AttackableTileSprite => attackableTileSprite;
        public Sprite HealableTileSprite => healableTileSprite;
        public Sprite RecruitableTileSprite => recruitableTileSprite;

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
            var linkedUnit = fromTile.LinkedUnit;
            var movementCosts = linkedUnit.MovementCosts;
            Tile tile = null;
            for (int i = 0; i < movementCosts.GetLength(0); i++)
            {
                for (int j = 0; j < movementCosts.GetLength(1); j++)
                {
                    if (movementCosts[i, j] > 0)
                    {
                        tile = GetTile(i, j);
                        if (tile.IsAvailable && movementCosts[i, j] <= linkedUnit.MovesLeft)
                        {
                            tile.DisplayMoveActionPossibility();
                        }
                        else if (tile.LinkedUnit != null && linkedUnit.TargetIsInRange(tile.LinkedUnit)  || FindAvailableAdjacentTile(tile, linkedUnit) != null))
                        {
                            if (tile.LinkedUnit.IsEnemy)
                            {
                                tile.DisplayAttackActionPossibility();
                            }
                            else if (tile.LinkedUnit.IsRecruitable)
                            {
                                tile.DisplayRecruitActionPossibility();
                            }
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
            
            Tile tileToLeft = Finder.GridController.GetTile(tilePosX - 1, tilePosY);
            if (tileToLeft != null && tileToLeft.IsAvailable && movementCosts[tilePosX - 1, tilePosY] <= unitMovesLeft)
                return tileToLeft;
            Tile tileAbove = Finder.GridController.GetTile(tilePosX, tilePosY - 1);
            if (tileAbove != null && tileAbove.IsAvailable && movementCosts[tilePosX, tilePosY - 1] <= unitMovesLeft)
                return tileAbove;
            Tile tileToRight = Finder.GridController.GetTile(tilePosX + 1, tilePosY);
            if (tileToRight != null && tileToRight.IsAvailable && movementCosts[tilePosX + 1, tilePosY] <= unitMovesLeft)
                return tileToRight;
            Tile tileUnder = Finder.GridController.GetTile(tilePosX, tilePosY + 1);
            if (tileUnder != null && tileUnder.IsAvailable && movementCosts[tilePosX, tilePosY + 1] <= unitMovesLeft)
                return tileUnder;
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
