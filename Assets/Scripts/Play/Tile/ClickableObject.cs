﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game
{
    public class ClickableObject : MonoBehaviour, IPointerClickHandler
    {
        private Tile tile;

        private void Awake()
        {
            tile = GetComponentInParent<Tile>();
        }

        private enum ClickButton
        {
            LeftClick,
            RightClick
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            ClickButton clickButton = ClickButton.LeftClick;
            if (eventData.button == PointerEventData.InputButton.Left)
                Debug.Log("Left click");
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("Right click");
                clickButton = ClickButton.RightClick;
            }

            
            EventSystem.current.SetSelectedGameObject(null);


            var gridControllerSelectedUnit = tile.GridController.SelectedUnit;
            
            if (tile.LinkedUnitCanBeSelected)
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
            
            if (tile.LinkedUnitCanBeAttacked)
            {
                if (clickButton == ClickButton.LeftClick)
                    gridControllerSelectedUnit.Attack(tile.LinkedUnit);
                if (clickButton == ClickButton.RightClick)
                    if (!gridControllerSelectedUnit.Attack(tile.LinkedUnit))
                        gridControllerSelectedUnit.AttackDistantUnit(tile, tile.LinkedUnit);
                if (tile.LinkedUnit.NoHealthLeft)
                {
                    tile.LinkedUnit.Die();
                    gridControllerSelectedUnit.MoveTo(tile);
                }
            }
            else if (tile.IsPossibleAction)
            {
                if (clickButton == ClickButton.LeftClick)
                    gridControllerSelectedUnit.MoveTo(tile);
                if (clickButton == ClickButton.RightClick)
                    gridControllerSelectedUnit.MoveToTileAndAct(tile, ActionType.Rest);
            }

            tile.GridController.DeselectUnit();
        }
    }
}