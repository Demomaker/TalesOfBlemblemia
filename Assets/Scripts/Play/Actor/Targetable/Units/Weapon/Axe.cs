using System;

namespace Game
{
    /// Auteur: Zacharie Lavigne, Jérémie Bertrand
    public class Axe : Weapon
    {
        private void Awake()
        {
            weaponType = WeaponType.Axe;
            advantage = WeaponTypeExt.AXE_ADVANTAGE;
        }
    }
}