using System.Collections;
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
        private PlayerClickManager playerClickManager;
        private Tile tile;
        private LevelController levelController;
        private GridController grid;
        
        private void Awake()
        {
            playerClickManager = Harmony.Finder.PlayerClickManager;
            levelController = Harmony.Finder.LevelController;
            tile = GetComponent<Tile>();
            grid = Harmony.Finder.GridController;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            tile.UpdateClickHint();
            //Player cannot play while a cinematic is playing or if another of its units is moving
            if (tile == null || !levelController.PlayerCanPlay) return;
            
            ClickButton clickButton = ReadClick(eventData);
            EventSystem.current.SetSelectedGameObject(null);
            var selectedPlayerUnit = tile.GridController.SelectedUnit;

            if (tile.LinkedUnitCanBeSelectedByPlayer && tile.LinkedUnit != selectedPlayerUnit)
            {
                if ( clickButton == ClickButton.LeftClick || (clickButton == ClickButton.RightClick && selectedPlayerUnit?.WeaponType != WeaponType.HealingStaff))
                {
                    SelectUnit(grid);
                    return;
                }
            }
            if (selectedPlayerUnit != null)
            {
                if (!tile.IsPossibleAction)
                {
                    DeselectUnit(grid);
                }
                else if (tile == selectedPlayerUnit.CurrentTile)
                {
                    if (clickButton == ClickButton.RightClick)
                        selectedPlayerUnit.Rest();
                    DeselectUnit(grid);
                }
                else if (!playerClickManager.ActionIsSet || playerClickManager.TileToConfirm != tile)
                {
                    playerClickManager.SetAction(selectedPlayerUnit, tile, clickButton);
                }
                else if (playerClickManager.TileToConfirm == tile)
                {
                    if (playerClickManager.ExecuteAction() != ActionType.Nothing) DeselectUnit(grid);
                    else
                    {
                        Harmony.Finder.CoroutineStarter.StartCoroutine(SelectAfterMove(grid, selectedPlayerUnit, tile));
                    }
                }
                else
                {
                    DeselectUnit(grid);
                }
            }

            tile.UpdateClickHint();
        }

        private void DeselectUnit(GridController gridController)
        {
            gridController.DeselectUnit();
            gridController.RemoveActionPath();
            playerClickManager.Reset();
            tile.UpdateClickHint();
        }

        private void SelectUnit(GridController gridController)
        {
            SelectUnit(gridController, tile.LinkedUnit, tile);
        }

        private void SelectUnit(GridController gridController, Unit playerUnit, Tile tileFrom)
        {
            DeselectUnit(gridController);
            playerClickManager.Reset();
            gridController.SelectUnit(playerUnit);
            gridController.DisplayPossibleActionsFrom(tileFrom);
            tile.UpdateClickHint();
        }

        private ClickButton ReadClick(PointerEventData eventData)
        {
            return eventData.button == PointerEventData.InputButton.Right ? ClickButton.RightClick : ClickButton.LeftClick;
        }

        public IEnumerator SelectAfterMove(GridController gridController, Unit playerUnit, Tile tileFrom)
        {
            DeselectUnit(gridController);
            while (playerUnit.CurrentTile != tileFrom)
            {
                yield return null;
            }
            SelectUnit(gridController, playerUnit, tileFrom);
        }
    }
}