namespace Game
{
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
        public static string GetString(this ClickType clickType)
        {
            switch (clickType)
            {
                case ClickType.Select:
                    return "Select";
                case ClickType.Deselect:
                    return "Deselect";
                case ClickType.MoveTo:
                    return "Move To";
                case ClickType.Rest:
                    return "Rest";
                case ClickType.Attack:
                    return "Attack";
                case ClickType.Heal:
                    return "Heal";
                case ClickType.ConfirmMoveTo:
                    return "Confirm Move To";
                case ClickType.ConfirmRest:
                    return "Confirm Rest";
                case ClickType.ConfirmAttack:
                    return "Confirm Attack";
                case ClickType.ConfirmRecruit:
                    return "Confirm Recruit";
                case ClickType.ConfirmHeal:
                    return "Confirm Heal";
            }
            return "";
        }
    }
}