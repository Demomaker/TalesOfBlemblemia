using System;

namespace Game
{
    /// <summary>
    /// Level name
    /// Author Mike Bedard, Correction: Pierre-Luc Maltais et Antoine Lessard.
    /// </summary>
    public static class LevelName
    {
        #region LevelName-related Functions
        public static string GetLevelNameFromLevelID(this LevelID levelID)
        {
            var gameSettings = Harmony.Finder.GameSettings;
            switch (levelID)
            {
                case LevelID.SnowyPeaks:
                    return gameSettings.SnowyPeaksSceneName;
                case LevelID.Jimsterburg:
                    return gameSettings.JimsterburgSceneName;
                case LevelID.ParabeneForest:
                    return gameSettings.ParabeneForestSceneName;
                case LevelID.BlemburgCitadel:
                    return gameSettings.BlemburgCitadelSceneName;
                case LevelID.DarkTower:
                    return gameSettings.DarkTowerSceneName;
                case LevelID.RinfretVillage:
                    return gameSettings.RinfretVillageSceneName;
                case LevelID.TulipValley:
                    return gameSettings.TulipValleySceneName;
                case LevelID.Morktress:
                    return gameSettings.MorktressSceneName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(levelID), levelID, null);
            }
        }
        #endregion LevelName-related Functions
    }
}