namespace Game
{
    /// <summary>
    /// Constant values used by the AiController class
    /// Author: Zacharie Lavigne
    /// </summary>
    public static class AiControllerValues
    {
        public static float BASE_TARGET_ACTION_SCORE = BASE_CHOICE_ACTION_SCORE + 3f;
        public static float ADJACENT_TARGET_CHOICE_MOD = 3f;
        public static float INACCESSIBLE_TARGET_CHOICE_MOD = -12f;
        public const float POTENTIAL_DEATH_CHOICE_MOD = -4f;
        public const float DAMAGE_RECEIVE_CHOICE_MOD = -0.8f;
        public const float SPEAR_ATTACKING_FORTRESS_CHOICE_MOD = -2f;
        public const float SPEAR_ATTACKING_FOREST_CHOICE_MOD = -1f;
        public const float AXE_ATTACKING_FORTRESS_CHOICE_MOD = -4f;
        public const float AXE_ATTACKING_FOREST_CHOICE_MOD = -3f;
        public const float SWORD_ATTACKING_FORTRESS_CHOICE_MOD = -3f;
        public const float SWORD_ATTACKING_FOREST_CHOICE_MOD = -2f;
        public const float ATTACKING_AXE_WITH_ADVANTAGE_CHOICE_MOD = 3f;
        public const float ATTACKING_SPEAR_WITH_ADVANTAGE_CHOICE_MOD = 1f;
        public const float ATTACKING_SWORD_WITH_ADVANTAGE_CHOICE_MOD = 2f;
        public const float ATTACKING_AXE_WITHOUT_ADVANTAGE_CHOICE_MOD = -1f;
        public const float ATTACKING_SPEAR_WITHOUT_ADVANTAGE_CHOICE_MOD = -2f;
        public const float ATTACKING_SWORD_WITHOUT_ADVANTAGE_CHOICE_MOD = -2f;
        public const float TURN_MULTIPLIER_FOR_DISTANCE_CHOICE_MOD = -2f;
        public const float TURN_ADDER_FOR_DISTANCE_CHOICE_MOD = 3f;
        public const int KILLING_ENEMY_CHOICE_MOD = 3;
        public const float BASE_CHOICE_ACTION_SCORE = 20f;
        public const float HEALTH_MOD_FOR_RESTING = 1.33f; 
    }
}