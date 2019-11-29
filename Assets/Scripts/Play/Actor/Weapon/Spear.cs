namespace Game
{
    /// Author: Zacharie Lavigne, Jérémie Bertrand
    public class Spear : Weapon
    {
        private void Awake()
        {
            weaponType = WeaponType.Spear;
            advantage = WeaponTypeExt.SPEAR_ADVANTAGE;
        }
    }
}