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
        #region Fields
        private string achievementName;
        private Func<Boolean> completeCondition;
        private bool achievementHasBeenShown = false;
        #endregion Fields
        #region Accessors
        public string AchievementName => achievementName;
        public bool AchievementCompleted => completeCondition();
        public bool AchievementHasBeenShown => achievementHasBeenShown;
        #endregion Accessors
        #region Constructors
        public Achievement(string achievementName, Func<Boolean> completeCondition)
        {
            this.achievementName = achievementName;
            this.completeCondition = completeCondition;
        }
        #endregion Constructors
        #region Achievement-related Functions
        public void SetAchievementHasBeenShown()
        {
            achievementHasBeenShown = true;
        }
        #endregion Achievement-related Functions
    }
}