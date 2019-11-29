using System;

namespace Game
{
    //Author: Jérémie Bertrand, Mike Bédard
    public enum LevelID
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

    public static class LevelIDExt
    {
        public static string GetLevelName(this LevelID levelId, GameSettings gameSettings)
        {
            switch (levelId)
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
                    throw new ArgumentOutOfRangeException(nameof(levelId), levelId, null);
            }
        }
    }
}