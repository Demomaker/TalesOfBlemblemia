using System;

namespace Game
{
    /// Auteur: Zacharie Lavigne
    public class Axe : Weapon
    {
        private void Awake()
        {
            weaponType = WeaponType.Axe;
            advantage = AXE_ADVANTAGE;
        }
    }
}