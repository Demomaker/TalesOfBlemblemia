namespace Game
{
    //Author : Antoine Lessard
    public class AchievementInfo
    {
        public string AchievementName { get; set; }
        public string AchievementDescription { get; set; }
        public bool Achieved { get; set; }

        public AchievementInfo(string achievementName, string achievementDescription, bool achieved)
        {
            AchievementName = achievementName;
            AchievementDescription = achievementDescription;
            Achieved = achieved;
        }
    }
}