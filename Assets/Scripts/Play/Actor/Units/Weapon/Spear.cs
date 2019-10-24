namespace Game
{
    /// Author: Zacharie Lavigne
    public class Spear : Weapon
    {
        private void Awake()
        {
            weaponType = WeaponType.Spear;
            advantage = SPEAR_ADVANTAGE;
        }
    }
}