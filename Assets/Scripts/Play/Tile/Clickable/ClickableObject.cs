﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// Personalized clickable to detect if a tile has been clicked on
    /// Author: Zacharie Lavigne
    /// </summary>
    public class ClickableObject : MonoBehaviour, IPointerClickHandler
    {
        private Tile tile;

        private void Awake()
        {
            tile = GetComponentInParent<Tile>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ClickButton clickButton = ClickButton.LeftClick;
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                clickButton = ClickButton.RightClick;
            }
            
            EventSystem.current.SetSelectedGameObject(null);
            if (tile == null) return;
            var gridControllerSelectedUnit = tile.GridController.SelectedUnit;

            if (clickButton == ClickButton.LeftClick && tile.LinkedUnitCanBeSelectedByPlayer)
            {
                if (clickButton == ClickButton.RightClick && tile.GridController.SelectedUnit == tile.LinkedUnit)
                {
                    if (tile == gridControllerSelectedUnit.CurrentTile)
                    {
                        gridControllerSelectedUnit.Rest();
                        tile.GridController.DeselectUnit();
                    }
                }
                else
                {
                    tile.GridController.SelectUnit(tile.LinkedUnit);
                    tile.GridController.DisplayPossibleActionsFrom(tile);
                }
                return;
            }
            
            if (clickButton == ClickButton.RightClick)
            {
                if (tile.LinkedUnitCanBeAttackedByPlayer)
                {
                    if (!gridControllerSelectedUnit.Attack(tile.LinkedUnit))
                        gridControllerSelectedUnit.AttackDistantUnit(tile.LinkedUnit);
                    if (tile.LinkedUnit.NoHealthLeft)
                    {
                        tile.LinkedUnit.Die();
                        gridControllerSelectedUnit.MoveByAction(new Action(gridControllerSelectedUnit.PrepareMove(tile)));
                    }
                }
                else if (tile.LinkedUnitCanBeRecruitedByPlayer)
                {
                    if (!gridControllerSelectedUnit.RecruitUnit(tile.LinkedUnit))
                        gridControllerSelectedUnit.RecruitDistantUnit(tile.LinkedUnit);
                }
                else if (gridControllerSelectedUnit.WeaponType == WeaponType.HealingStaff && tile.LinkedUnitCanBeHealedByPlayer)
                {
                    if (!gridControllerSelectedUnit.HealUnit(tile.LinkedUnit))
                        gridControllerSelectedUnit.HealDistantUnit(tile.LinkedUnit);
                }
                else if (tile.IsPossibleAction)
                {
                    gridControllerSelectedUnit.MoveByAction(new Action(gridControllerSelectedUnit.PrepareMove(tile), ActionType.Rest));
                    //gridControllerSelectedUnit.MoveToTileAndAct(tile, ActionType.Rest);
                }
            }
            else if (tile.IsPossibleAction && tile.IsAvailable)
            {
                gridControllerSelectedUnit.MoveByAction(new Action(gridControllerSelectedUnit.PrepareMove(tile)));
                //gridControllerSelectedUnit.MoveToTileAndAct(tile);
            }

            tile.GridController.DeselectUnit();

        }
    }
}