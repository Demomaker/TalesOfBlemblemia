using System;

namespace Game
{
    public static class LevelID
    {
        public static string GetLevelNameFromLevelID(this LevelIDValue levelId)
        {
            var gameSettings = Harmony.Finder.GameSettings;
            switch (levelId)
            {
                case LevelIDValue.Level1:
                    return gameSettings.TutorialSceneName;
                case LevelIDValue.Level2:
                    return gameSettings.JimsterburgSceneName;
                case LevelIDValue.Level3:
                    return gameSettings.ParabeneForestSceneName;
                case LevelIDValue.Level4:
                    return gameSettings.BlemburgCitadelSceneName;
                case LevelIDValue.Level5:
                    return gameSettings.RinfretVillageSceneName;
                case LevelIDValue.Level6:
                    return gameSettings.DarkTowerSceneName;
                case LevelIDValue.Level7:
                    return gameSettings.TulipValleySceneName;
                case LevelIDValue.Level8:
                    return gameSettings.MorktressSceneName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(levelId), levelId, null);
            }
        }
        public enum LevelIDValue
        {
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
            Level6,
            Level7,
            Level8
        }
    }
}