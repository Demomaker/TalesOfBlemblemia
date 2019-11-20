namespace Game
{
    //Author : Antoine Lessard
    public class AchievementInfo
    {
        public string AchievementName { get; set; }
        public bool Achieved { get; set; }

        public AchievementInfo(string achievementName, bool achieved)
        {
            AchievementName = achievementName;
            Achieved = achieved;
        }
    }
}