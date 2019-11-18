using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game.UI.Achievement
{
    /// <summary>
    /// Achievement
    /// Author : Mike Bédard
    /// </summary>
    public class Achievement
    {
        private string achievementName;
        private Func<Boolean> completeCondition;
        private bool achievementHasBeenShown = false;

        public string AchievementName => achievementName;
        public bool AchievementCompleted => completeCondition();
        public bool AchievementHasBeenShown => achievementHasBeenShown;

        public Achievement(string achievementName, Func<Boolean> completeCondition)
        {
            this.achievementName = achievementName;
            this.completeCondition = completeCondition;
        }

        public void SetAchievementHasBeenShown()
        {
            achievementHasBeenShown = true;
        }
    }
}