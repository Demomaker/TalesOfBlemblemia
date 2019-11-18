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


        public static void SetAction(Unit selectedPlayerUnit, Tile target, ClickButton clickButton)
        {
            playerUnit = selectedPlayerUnit;
            tileToConfirm = target;
            
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
            }

            Finder.GridController.RemoveActionPath();
            if (unitTurnAction != null)
                    Finder.GridController.DisplayAction(unitTurnAction, selectedPlayerUnit);
        }

        public static void ExecuteAction()
        {
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