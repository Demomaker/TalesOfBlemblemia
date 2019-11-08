namespace Game
{
    public class BattleInfos
    {
        public int MaxHp { get; set; }

        public int HealthBeforeCombat { get; set; }
        
        public int DamageTaken { get; set; }
        
        
        public void ChangeInfos(int maxHp, int healthBeforeCombat, int damageTaken = 0)
        {
            MaxHp = maxHp;
            HealthBeforeCombat = healthBeforeCombat;
            DamageTaken = damageTaken;
        }
        
    }
}