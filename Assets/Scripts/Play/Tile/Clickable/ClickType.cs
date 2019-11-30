using System;

namespace Game
{
    //Author: Jérémie Bertrand
    public enum ClickType
    {
        Select,
        Deselect,
        MoveTo,
        Rest,
        Attack,
        Heal,
        Recruit,
        ConfirmMoveTo,
        ConfirmRest,
        ConfirmAttack,
        ConfirmRecruit,
        ConfirmHeal,
        None
    }
    
    public static class ClickTypeExt
    {
        public static string GetString(this ClickType clickType, GameSettings gameSettings)
        {
            switch (clickType)
            {
                case ClickType.Select:
                    return gameSettings.SelectText;
                case ClickType.Deselect:
                    return gameSettings.DeselectText;
                case ClickType.MoveTo:
                    return gameSettings.MoveToText;
                case ClickType.Rest:
                    return gameSettings.RestText;
                case ClickType.Attack:
                    return gameSettings.AttackText;
                case ClickType.Heal:
                    return gameSettings.HealText;
                case ClickType.Recruit:
                    return gameSettings.RecruitText;
                case ClickType.ConfirmMoveTo:
                    return gameSettings.ConfirmMoveToText;
                case ClickType.ConfirmRest:
                    return gameSettings.ConfirmRestText;
                case ClickType.ConfirmAttack:
                    return gameSettings.ConfirmAttackText;
                case ClickType.ConfirmRecruit:
                    return gameSettings.ConfirmRecruitText;
                case ClickType.ConfirmHeal:
                    return gameSettings.ConfirmHealText;
                case ClickType.None:
                    return gameSettings.NoneText;
                default:
                    throw new ArgumentOutOfRangeException(nameof(clickType), clickType, null);
            }
        }
    }
}