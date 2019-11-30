namespace Game
{
    /// Author: Zacharie Lavigne, Jérémie Bertrand
    public class Sword : Weapon
    {
        private void Awake()
        {
            weaponType = WeaponType.Sword;
            advantage = WeaponTypeExt.SWORD_ADVANTAGE;
        }
    }
}