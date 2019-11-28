using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// Personalized clickable to detect if a tile has been clicked on
    /// Author: Zacharie Lavigne, Jérémie Bertrand
    /// </summary>
    public class ClickableObject : MonoBehaviour, IPointerClickHandler
    {
        private Tile tile;
        private LevelController levelController;
        private GridController grid;
        
        private void Awake()
        {
            levelController = Harmony.Finder.LevelController;
            tile = GetComponent<Tile>();
            grid = Harmony.Finder.GridController;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //Player cannot play while a cinematic is playing or if another of its units is moving
            if (tile == null || levelController.PlayerCanPlay) return;
            
            var gridController = Harmony.Finder.GridController;
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
                else if (!Harmony.Finder.PlayerClickManager.ActionIsSet || Harmony.Finder.PlayerClickManager.TileToConfirm != tile)
                {
                    Harmony.Finder.PlayerClickManager.SetAction(selectedPlayerUnit, tile, clickButton);
                }
                else if (Harmony.Finder.PlayerClickManager.TileToConfirm == tile)
                {
                    if (Harmony.Finder.PlayerClickManager.ExecuteAction() != ActionType.Nothing) DeselectUnit(gridController);
                    else
                    {
                        SelectUnit(gridController, selectedPlayerUnit, Harmony.Finder.PlayerClickManager.TileToConfirm);
                    }
                }
                else
                {
                    DeselectUnit(gridController);
                }
            }

            tile.UpdateClickHint();
        }

        private void DeselectUnit(GridController gridController)
        {
            gridController.DeselectUnit();
            gridController.RemoveActionPath();
            Harmony.Finder.PlayerClickManager.Reset();
            tile.UpdateClickHint();
        }

        private void SelectUnit(GridController gridController)
        {
            SelectUnit(gridController, tile.LinkedUnit, tile);
        }

        private void SelectUnit(GridController gridController, Unit playerUnit, Tile tileFrom)
        {
            DeselectUnit(gridController);
            Harmony.Finder.PlayerClickManager.Reset();
            gridController.SelectUnit(playerUnit);
            gridController.DisplayPossibleActionsFrom(tileFrom);
            tile.UpdateClickHint();
        }

        private ClickButton ReadClick(PointerEventData eventData)
        {
            return eventData.button == PointerEventData.InputButton.Right ? ClickButton.RightClick : ClickButton.LeftClick;
        }
    }
}