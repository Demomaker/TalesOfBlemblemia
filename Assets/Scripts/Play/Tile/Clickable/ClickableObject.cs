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

        private void Awake()
        {
            tile = GetComponent<Tile>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //Player cannot play while a cinematic is playing
           if (tile == null) return;

           var gridController = Finder.GridController;
           
            ClickButton clickButton = ReadClick(eventData);
            
            EventSystem.current.SetSelectedGameObject(null);
            
            var selectedPlayerUnit = tile.GridController.SelectedUnit;

            if (tile.LinkedUnitCanBeSelectedByPlayer && tile.LinkedUnit != selectedPlayerUnit)
            {
                SelectUnit(gridController);
                return;
            }
            
            if (selectedPlayerUnit != null)
            {
                if (tile == selectedPlayerUnit.CurrentTile)
                {
                    selectedPlayerUnit.Rest();
                    tile.GridController.DeselectUnit();
                }
                else if (!PlayerClickManager.ActionIsSet || PlayerClickManager.TileToConfirm != tile)
                {
                    PlayerClickManager.SetAction(selectedPlayerUnit, tile, clickButton);
                }
                else if (PlayerClickManager.TileToConfirm == tile)
                {
                    PlayerClickManager.ExecuteAction();
                    gridController.DeselectUnit();
                }
                else
                {
                    PlayerClickManager.Reset();
                    gridController.DeselectUnit();
                }
            }
            


        }

        private void SelectUnit(GridController gridController)
        {
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