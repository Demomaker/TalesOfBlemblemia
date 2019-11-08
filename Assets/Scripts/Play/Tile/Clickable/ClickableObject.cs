using System;
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
        private UIController uiController;

        private void Awake()
        {
            uiController = Harmony.Finder.UIController;
            tile = GetComponentInParent<Tile>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Harmony.Finder.LevelController.CinematicController.IsPlayingACinematic) return;
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
            if (clickButton == ClickButton.RightClick && gridControllerSelectedUnit != null)
            {
                if (tile.LinkedUnitCanBeAttackedByPlayer)
                {
                    if (!gridControllerSelectedUnit.TargetIsInRange(tile.LinkedUnit))
                    {
                        gridControllerSelectedUnit.AttackDistantTargetable(tile.LinkedUnit);
                    }
                    else
                    {
                       gridControllerSelectedUnit.Attack(tile.LinkedUnit);
                       uiController.LaunchBattleReport(false);
                    }
                        
                    if (tile.LinkedUnit.NoHealthLeft)
                    {
                        gridControllerSelectedUnit.MoveByAction(new Action(gridControllerSelectedUnit.PrepareMove(tile)));
                    }
                }
                else if (tile.LinkedDoorCanBeAttackedByPlayer)
                {
                    if (!gridControllerSelectedUnit.TargetIsInRange(tile.LinkedDoor))
                        gridControllerSelectedUnit.AttackDistantTargetable(tile.LinkedDoor);
                    if (tile.LinkedDoor.NoHealthLeft)
                    {
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
                }
            }
            else if (tile.IsPossibleAction && tile.IsAvailable)
            {
                gridControllerSelectedUnit.MoveByAction(new Action(gridControllerSelectedUnit.PrepareMove(tile)));
            }

            tile.GridController.DeselectUnit();

        }
    }
}