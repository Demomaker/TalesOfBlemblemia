namespace Game
{
    /// <summary>
    /// Constant values used by the AiController class
    /// Author: Zacharie Lavigne
    /// </summary>
    public class AiControllerValues
    {
        private float baseTargetActionScore = 20f + 3f;
        private float adjacentTargetChoiceMod = 6f;
        private float inaccessibleTargetChoiceMod = -15f;
        private float potentialDeathChoiceMod = -5f;
        private float damageReceiveChoiceMod = -0.8f;
        private float spearAttackingFortressChoiceMod = -2f;
        private float spearAttackingForestChoiceMod = -1f;
        private float axeAttackingFortressChoiceMod = -4f;
        private float axeAttackingForestChoiceMod = -3f;
        private float swordAttackingFortressChoiceMod = -3f;
        private float swordAttackingForestChoiceMod = -2f;
        protected float attackingAxeWithAdvantageChoiceMod = 4f;
        protected float attackingSpearWithAdvantageChoiceMod = 2f;
        protected float attackingSwordWithAdvantageChoiceMod = 3f;
        protected float attackingAxeWithoutAdvantageChoiceMod = -2f;
        protected float attackingSpearWithoutAdvantageChoiceMod = -3f;
        protected float attackingSwordWithoutAdvantageChoiceMod = -2.5f;
        private float turnMultiplierForDistanceChoiceMod = -4f;
        private float turnAdderForDistanceChoiceMod = 4f;
        private float killingEnemyChoiceMod = 12f;
        private float baseChoiceActionScore = 20f;
        private float healthModForResting = 1.33f;

        public float BaseTargetActionScore => baseTargetActionScore;
        public float AdjacentTargetChoiceMod => adjacentTargetChoiceMod;
        public float InaccessibleTargetChoiceMod => inaccessibleTargetChoiceMod;
        public float PotentialDeathChoiceMod => potentialDeathChoiceMod;
        public float DamageReceiveChoiceMod => damageReceiveChoiceMod;
        public float SpearAttackingFortressChoiceMod => spearAttackingFortressChoiceMod;
        public float SpearAttackingForestChoiceMod => spearAttackingForestChoiceMod;
        public float AxeAttackingFortressChoiceMod => axeAttackingFortressChoiceMod;
        public float AxeAttackingForestChoiceMod => axeAttackingForestChoiceMod;
        public float SwordAttackingFortressChoiceMod => swordAttackingFortressChoiceMod;
        public float SwordAttackingForestChoiceMod => swordAttackingForestChoiceMod;
        public float AttackingAxeWithAdvantageChoiceMod => attackingAxeWithAdvantageChoiceMod;
        public float AttackingSpearWithAdvantageChoiceMod => attackingSpearWithAdvantageChoiceMod;
        public float AttackingSwordWithAdvantageChoiceMod => attackingSwordWithAdvantageChoiceMod;
        public float AttackingAxeWithoutAdvantageChoiceMod => attackingAxeWithoutAdvantageChoiceMod;
        public float AttackingSpearWithoutAdvantageChoiceMod => attackingSpearWithoutAdvantageChoiceMod;
        public float AttackingSwordWithoutAdvantageChoiceMod => attackingSwordWithoutAdvantageChoiceMod;
        public float TurnMultiplierForDistanceChoiceMod => turnMultiplierForDistanceChoiceMod;
        public float TurnAdderForDistanceChoiceMod => turnAdderForDistanceChoiceMod;
        public float KillingEnemyChoiceMod => killingEnemyChoiceMod;
        public float BaseChoiceActionScore => baseChoiceActionScore;
        public float HealthModForResting => healthModForResting;}
}