using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    //Author: Zacharie Lavigne
    [Findable("PlayerClickManager")]
    public class PlayerClickManager : MonoBehaviour
    {
        private Action unitTurnAction;
        private Tile tileToConfirm;
        private Unit playerUnit;
        private GridController grid;
        
        public bool ActionIsSet => unitTurnAction != null;
        public Tile TileToConfirm => tileToConfirm;
        public Action UnitTurnAction => unitTurnAction;

        public void SetAction(Unit selectedPlayerUnit, Tile target, ClickButton clickButton)
        {
            if (grid == null) grid = Harmony.Finder.GridController;
            
            playerUnit = selectedPlayerUnit;
            tileToConfirm = target;

            var path = new List<Tile>();
            path.Add(selectedPlayerUnit.CurrentTile);
            if (clickButton == ClickButton.LeftClick)
            {
                switch (target.LeftClickType)
                {
                    case ClickType.MoveTo:
                        if (target.IsAvailable) unitTurnAction = new Action(selectedPlayerUnit.UnitMover.PrepareMove(target, false));
                        break;
                    case ClickType.None:
                        break;
                    default:
                        throw new Exception("Player click manager should only manage actions clicks, not selecting or other");
                }
            }
            else if (clickButton == ClickButton.RightClick)
            {
                switch (target.RightClickType)
                {
                    case ClickType.MoveTo:
                        unitTurnAction = new Action(selectedPlayerUnit.UnitMover.PrepareMove(target, false));
                        break;
                    case ClickType.Rest:
                        unitTurnAction = new Action(selectedPlayerUnit.UnitMover.PrepareMove(target, false), ActionType.Rest);
                        break;
                    case ClickType.Attack:
                        if (selectedPlayerUnit.TargetIsInRange(target.LinkedTargetable))
                        {
                            unitTurnAction = new Action(path, ActionType.Attack, target.LinkedTargetable);
                        }
                        else
                        {
                            var adjacentTile =
                                grid.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                            if (adjacentTile != null)
                                unitTurnAction = new Action(selectedPlayerUnit.UnitMover.PrepareMove(adjacentTile, false),
                                    ActionType.Attack, target.LinkedTargetable);
                        }
                        break;
                    case ClickType.Heal:
                        if (selectedPlayerUnit.TargetIsInRange(target.LinkedUnit))
                        {
                            unitTurnAction = new Action(path, ActionType.Heal, target.LinkedUnit);
                        }
                        else
                        {
                            var adjacentTile =
                                grid.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                            if (adjacentTile != null)
                                unitTurnAction = new Action(selectedPlayerUnit.UnitMover.PrepareMove(adjacentTile, false),
                                    ActionType.Heal, target.LinkedUnit);
                        }
                        break;
                    case ClickType.Recruit:
                        if (selectedPlayerUnit.TargetIsInRange(target.LinkedUnit))
                        {
                            unitTurnAction = new Action(path, ActionType.Recruit, target.LinkedUnit);
                        }
                        else
                        {
                            var adjacentTile =
                                grid.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                            if (adjacentTile != null)
                                unitTurnAction = new Action(selectedPlayerUnit.UnitMover.PrepareMove(adjacentTile, false),
                                    ActionType.Recruit, target.LinkedUnit);
                        }
                        break;
                    case ClickType.None:
                        break;
                    default:
                        //break;
                        throw new Exception("Player click manager should only manage actions clicks, not selecting or other");
                }
            }
            grid.RemoveActionPath();
            if (unitTurnAction != null)
                grid.DisplayAction(unitTurnAction, selectedPlayerUnit);
        }

        public ActionType ExecuteAction()
        {
            ActionType actionType = unitTurnAction.ActionType;
            playerUnit.MoveByAction(unitTurnAction);
            Reset();
            return actionType;
        }

        public void Reset()
        {
            playerUnit = null;
            tileToConfirm = null;
            unitTurnAction = null;
        }
    }
}