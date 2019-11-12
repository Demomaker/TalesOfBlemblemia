namespace Game
{
    public class Level
    {
        public string PreviousLevel { get; }
        public string LevelName { get; }

        public Level(string previousLevel, string levelName)
        {
            this.PreviousLevel = previousLevel;
            this.LevelName = levelName;
        }
    }
}