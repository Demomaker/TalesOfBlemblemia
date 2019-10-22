namespace Game
{
    /// Author: Zacharie Lavigne
    public class HealingStaff : Weapon
    {
        private void Awake()
        {
            weaponType = WeaponType.HealingStaff;
            advantage = WeaponType.None;
        }
    }
}