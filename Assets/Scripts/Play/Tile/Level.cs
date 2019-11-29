namespace Game
{
    //Author: Mike Bédard
    public class Level
    {
        public string PreviousLevel { get; }
        public string LevelName { get; }

        public Level(string previousLevel, string levelName)
        {
            PreviousLevel = previousLevel;
            LevelName = levelName;
        }
    }
}