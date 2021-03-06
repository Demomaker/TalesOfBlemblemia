﻿//Author: Pierre-Luc Maltais
namespace Game
{
    public class BattleInfos
    {
        public int MaxHp { get; set; }

        public int CurrentHealth { get; set; }
        
        public int DamageTaken { get; set; }
        
        public bool CriticalHit { get; set; }
        
        
        public void ChangeInfos(int maxHp, int healthBeforeCombat, int critModifier = 1, int damageTaken = 0)
        {
            MaxHp = maxHp;
            CurrentHealth = healthBeforeCombat;
            DamageTaken = damageTaken;
            CriticalHit = critModifier - 1 != 0;
        }
        
    }
}