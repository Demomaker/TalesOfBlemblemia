using System;
using System.Collections.Generic;

namespace Game
{
    //Author: Zacharie Lavigne
    public static class PlayerClickManager
    {
        private static Action unitTurnAction;
        private static Tile tileToConfirm;
        private static Unit playerUnit;

        public static bool ActionIsSet => unitTurnAction != null;
        public static Tile TileToConfirm => tileToConfirm;
        public static Action UnitTurnAction => unitTurnAction;

        public static void SetAction(Unit selectedPlayerUnit, Tile target, ClickButton clickButton)
        {
            playerUnit = selectedPlayerUnit;
            tileToConfirm = target;

            var path = new List<Tile>();
            path.Add(selectedPlayerUnit.CurrentTile);
            if (clickButton == ClickButton.LeftClick)
            {
                switch (target.RightClickType)
                {
                    case ClickType.MoveTo:
                        unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(target, false));
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
                        unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(target, false));
                        break;
                    case ClickType.Rest:
                        unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(target, false), ActionType.Rest);
                        break;
                    case ClickType.Attack:
                        if (selectedPlayerUnit.TargetIsInRange(target.LinkedTargetable))
                        {
                            unitTurnAction = new Action(path, ActionType.Attack, target.LinkedTargetable);
                        }
                        else
                        {
                            var adjacentTile =
                                Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                            if (adjacentTile != null)
                                unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile, false),
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
                                Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                            if (adjacentTile != null)
                                unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile, false),
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
                                Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                            if (adjacentTile != null)
                                unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile, false),
                                    ActionType.Recruit, target.LinkedUnit);
                        }
                        break;
                    default:
                        throw new Exception("Player click manager should only manage actions clicks, not selecting or other");
                }
            }
            /*
            if (clickButton == ClickButton.RightClick && selectedPlayerUnit != null)
            {
                var path = new List<Tile>();
                path.Add(selectedPlayerUnit.CurrentTile);
                if (target.LinkedUnitCanBeAttackedByPlayer)
                {
                    if (selectedPlayerUnit.TargetIsInRange(target.LinkedUnit))
                    {
                        unitTurnAction = new Action(path, ActionType.Attack, target.LinkedUnit);
                    }
                    else
                    {
                        var adjacentTile = Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                        if (adjacentTile != null)
                            unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile, false), ActionType.Attack, target.LinkedUnit);
                    }
                }
                else if (target.LinkedDoorCanBeAttackedByPlayer)
                {
                    if (selectedPlayerUnit.TargetIsInRange(target.LinkedDoor))
                    {
                        unitTurnAction = new Action(path, ActionType.Attack, target.LinkedDoor);
                    }
                    else
                    {
                        var adjacentTile = Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                        if (adjacentTile != null)
                            unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile, false), ActionType.Attack, target.LinkedDoor);
                    }
                }
                else if (target.LinkedUnitCanBeRecruitedByPlayer)
                {
                    if (selectedPlayerUnit.TargetIsInRange(target.LinkedUnit))
                    {
                        unitTurnAction = new Action(path, ActionType.Recruit, target.LinkedUnit);
                    }
                    else
                    {
                        var adjacentTile = Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                        if (adjacentTile != null)
                            unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile, false), ActionType.Recruit, target.LinkedUnit);
                    }
                }
                else if (selectedPlayerUnit.WeaponType == WeaponType.HealingStaff && target.LinkedUnitCanBeHealedByPlayer)
                {
                    if (selectedPlayerUnit.TargetIsInRange(target.LinkedUnit))
                    {
                        unitTurnAction = new Action(path, ActionType.Heal, target.LinkedUnit);
                    }
                    else
                    {
                        var adjacentTile = Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                        if (adjacentTile != null)
                            unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile, false), ActionType.Heal, target.LinkedUnit);
                    }
                }
                else if (target.IsPossibleAction)
                {
                    unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(target, false), ActionType.Rest);
                }
            }
            else if (target.IsPossibleAction && target.IsAvailable)
            {
                unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(target, false));
            }*/

            Finder.GridController.RemoveActionPath();
            if (unitTurnAction != null)
                    Finder.GridController.DisplayAction(unitTurnAction, selectedPlayerUnit);
        }

        public static void ExecuteAction()
        {
            playerUnit.RemoveInitialMovement();
            playerUnit.MoveByAction(unitTurnAction);
            Reset();
        }

        public static void Reset()
        {
            playerUnit = null;
            tileToConfirm = null;
            unitTurnAction = null;
        }
    }
}