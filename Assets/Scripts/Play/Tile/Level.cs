namespace Game
{
    public class Level
    {
        private string previousLevel;
        private string levelName;

        public string PreviousLevel => previousLevel;
        public string LevelName => levelName;

        public Level(string previousLevel, string levelName)
        {
            this.previousLevel = previousLevel;
            this.levelName = levelName;
        }
    }
}