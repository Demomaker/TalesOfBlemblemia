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
            tile = GetComponent<Tile>();
            uiController = Harmony.Finder.UIController;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //Player cannot play while a cinematic is playing or if another of its units is moving
            if (tile == null || Harmony.Finder.LevelController.PlayerUnitIsMovingOrAttacking) return;
            
            var gridController = Finder.GridController;
            ClickButton clickButton = ReadClick(eventData);
            EventSystem.current.SetSelectedGameObject(null);
            var selectedPlayerUnit = tile.GridController.SelectedUnit;

            if (tile.LinkedUnitCanBeSelectedByPlayer && tile.LinkedUnit != selectedPlayerUnit)
            {
                if ( clickButton == ClickButton.LeftClick || (clickButton == ClickButton.RightClick && selectedPlayerUnit?.WeaponType != WeaponType.HealingStaff))
                {
                    SelectUnit(gridController);
                    return;
                }
            }
            
            if (selectedPlayerUnit != null)
            {
                if (!tile.IsPossibleAction)
                {
                    DeselectUnit(gridController);
                }
                else if (tile == selectedPlayerUnit.CurrentTile)
                {
                    if (clickButton == ClickButton.RightClick)
                        selectedPlayerUnit.Rest();
                    DeselectUnit(gridController);
                }
                else if (!PlayerClickManager.ActionIsSet || PlayerClickManager.TileToConfirm != tile)
                {
                    PlayerClickManager.SetAction(selectedPlayerUnit, tile, clickButton);
                }
                else if (PlayerClickManager.TileToConfirm == tile)
                {
                    PlayerClickManager.ExecuteAction();
                    DeselectUnit(gridController);
                }
                else
                {
                    DeselectUnit(gridController);
                }
            }
        }

        private void DeselectUnit(GridController gridController)
        {
            gridController.DeselectUnit();
            gridController.RemoveActionPath();
            PlayerClickManager.Reset();
        }

        private void SelectUnit(GridController gridController)
        {
            DeselectUnit(gridController);
            PlayerClickManager.Reset();
            gridController.SelectUnit(tile.LinkedUnit);
            gridController.DisplayPossibleActionsFrom(tile);
        }

        private ClickButton ReadClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                return ClickButton.RightClick;
            return ClickButton.LeftClick;
        }
    }
}