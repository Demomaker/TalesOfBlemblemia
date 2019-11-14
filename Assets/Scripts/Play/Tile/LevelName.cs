using System;

namespace Game
{
    public static class LevelName
    {
        public static string GetLevelNameFromLevelID(this LevelNameEnum levelNameEnum)
        {
            var gameSettings = Harmony.Finder.GameSettings;
            switch (levelNameEnum)
            {
                case LevelNameEnum.SnowyPeaks:
                    return gameSettings.TutorialSceneName;
                case LevelNameEnum.Jimsterburg:
                    return gameSettings.JimsterburgSceneName;
                case LevelNameEnum.ParabeneForest:
                    return gameSettings.ParabeneForestSceneName;
                case LevelNameEnum.BlemburgCitadel:
                    return gameSettings.BlemburgCitadelSceneName;
                case LevelNameEnum.DarkTower:
                    return gameSettings.DarkTowerSceneName;
                case LevelNameEnum.RinfretVillage:
                    return gameSettings.RinfretVillageSceneName;
                case LevelNameEnum.TulipValley:
                    return gameSettings.TulipValleySceneName;
                case LevelNameEnum.Morktress:
                    return gameSettings.MorktressSceneName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(levelNameEnum), levelNameEnum, null);
            }
        }
        public enum LevelNameEnum
        {
            SnowyPeaks,
            Jimsterburg,
            ParabeneForest,
            BlemburgCitadel,
            DarkTower,
            RinfretVillage,
            TulipValley,
            Morktress
        }
    }
}