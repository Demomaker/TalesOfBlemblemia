namespace Game
{
    /// <summary>
    /// Level
    /// Author : Mike Bédard
    /// </summary>
    public class Level
    {
        #region Accessors
        public string PreviousLevel { get; }
        public string LevelName { get; }
        #endregion Accessors
        #region Constructors
        public Level(string previousLevel, string levelName)
        {
            this.PreviousLevel = previousLevel;
            this.LevelName = levelName;
        }
        #endregion Constructors
    }
}