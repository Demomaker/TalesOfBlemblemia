namespace Game
{
    // Author: Jérémie Bertrand
    public enum TileType 
    {
        Empty = 0,
        Obstacle = 1,
        Forest = 2,
        Fortress = 3,
    }
    
    public static class TileTypeExt
    {
        public const float LOW_DEFENSE_RATE = .0f;
        public const float MEDIUM_DEFENSE_RATE = .15f;
        public const float HIGH_DEFENSE_RATE = .33f;
        public const int LOW_COST_TO_MOVE = 1;
        public const int MEDIUM_COST_TO_MOVE = 2;
        public const int HIGH_COST_TO_MOVE = int.MaxValue;

        public static float GetDefenseRate(this TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Forest:
                    return MEDIUM_DEFENSE_RATE;
                case TileType.Fortress:
                    return HIGH_DEFENSE_RATE;
                default:
                    return LOW_DEFENSE_RATE;
            }
        }
        
        public static int GetCostToMove(this TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Forest:
                    return MEDIUM_COST_TO_MOVE;
                case TileType.Obstacle:
                    return HIGH_COST_TO_MOVE;
                default:
                    return LOW_COST_TO_MOVE;
            }
        }
    }
}