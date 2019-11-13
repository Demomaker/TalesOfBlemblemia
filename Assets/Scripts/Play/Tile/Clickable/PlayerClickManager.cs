using System;
using System.Collections.Generic;

namespace Game
{
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
                if (target.LinkedUnitCanBeAttackedByPlayer)
                {
                    if (selectedPlayerUnit.TargetIsInRange(target.LinkedUnit))
                    {
                        var path = new List<Tile>();
                        path.Add(selectedPlayerUnit.CurrentTile);
                        unitTurnAction = new Action(path, ActionType.Attack, target.LinkedUnit);
                    }
                    else
                    {
                        var adjacentTile = Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                        if (adjacentTile != null)
                            unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile), ActionType.Attack, target.LinkedUnit);
                    }
                }
                else if (target.LinkedDoorCanBeAttackedByPlayer)
                {
                    if (selectedPlayerUnit.TargetIsInRange(target.LinkedDoor))
                    {
                        var path = new List<Tile>();
                        path.Add(selectedPlayerUnit.CurrentTile);
                        unitTurnAction = new Action(path, ActionType.Attack, target.LinkedDoor);
                    }
                    else
                    {
                        var adjacentTile = Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                        if (adjacentTile != null)
                            unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile), ActionType.Attack, target.LinkedDoor);
                    }
                }
                else if (target.LinkedUnitCanBeRecruitedByPlayer)
                {
                    if (selectedPlayerUnit.TargetIsInRange(target.LinkedDoor))
                    {
                        var path = new List<Tile>();
                        path.Add(selectedPlayerUnit.CurrentTile);
                        unitTurnAction = new Action(path, ActionType.Attack, target.LinkedUnit);
                    }
                    else
                    {
                        var adjacentTile = Finder.GridController.FindAvailableAdjacentTile(target, selectedPlayerUnit);
                        if (adjacentTile != null)
                            unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(adjacentTile), ActionType.Recruit, target.LinkedUnit);
                    }
                }
                else if (selectedPlayerUnit.WeaponType == WeaponType.HealingStaff && target.LinkedUnitCanBeHealedByPlayer)
                {
                    if (!selectedPlayerUnit.HealUnit(target.LinkedUnit))
                        selectedPlayerUnit.HealDistantUnit(target.LinkedUnit);
                }
                else if (target.IsPossibleAction)
                {
                    unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(target));
                }
            }
            else if (target.IsPossibleAction && target.IsAvailable)
            {
                unitTurnAction = new Action(selectedPlayerUnit.PrepareMove(target));
            }

            Finder.GridController.RemoveActionPath();
            try
            {
                if (unitTurnAction != null)
                    Finder.GridController.DisplayAction(unitTurnAction, selectedPlayerUnit);
            }
            catch
            {
                int i;
            }
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