using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game.UI.Achievement
{
    public class Achievement
    {
        private string achievementName;
        private Func<Boolean> completeCondition;

        public string AchievementName => achievementName;
        public bool AchievementCompleted => completeCondition();
        public bool AchievementHasBeenShown = false;

        public Achievement(string achievementName, Func<Boolean> completeCondition)
        {
            this.achievementName = achievementName;
            this.completeCondition = completeCondition;
        }
    }
}