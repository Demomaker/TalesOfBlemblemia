namespace Game
{
    /// Author: Zacharie Lavigne
    public class Sword : Weapon
    {
        private void Awake()
        {
            weaponType = WeaponType.Sword;
            advantage = SWORD_ADVANTAGE;
        }
    }
}