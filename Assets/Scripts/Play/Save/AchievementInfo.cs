namespace Game
{
    //Author : Antoine Lessard
    public class AchievementInfo
    {
        public string AchievementName { get; }
        public string AchievementDescription { get; }
        public bool Achieved { get; set; }

        public AchievementInfo(string achievementName, string achievementDescription, bool achieved)
        {
            AchievementName = achievementName;
            AchievementDescription = achievementDescription;
            Achieved = achieved;
        }
    }
}