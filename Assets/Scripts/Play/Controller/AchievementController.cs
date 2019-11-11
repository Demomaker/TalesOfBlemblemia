using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI.Achievement;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class AchievementController : MonoBehaviour
    {

        [SerializeField] private Text nameText = null;
        [SerializeField] private Text descriptionText = null;
        [SerializeField] private Animator animator = null;
        private List<Achievement> Achievements  = new List<Achievement>();
        private bool AchievementBeingShown = false;

        private void Awake()
        {
            Achievements.Add(new Achievement(Harmony.Finder.GameSettings.CompleteCampaignOnEasy,  () => Harmony.Finder.GameController.AllLevelsCompleted && Harmony.Finder.GameController.DifficultyLevel == DifficultyLevel.Easy));
            Achievements.Add(new Achievement(Harmony.Finder.GameSettings.CompleteCampaignOnMedium,  () => Harmony.Finder.GameController.AllLevelsCompleted && Harmony.Finder.GameController.DifficultyLevel == DifficultyLevel.Medium));
            Achievements.Add(new Achievement(Harmony.Finder.GameSettings.CompleteCampaignOnHard,  () => Harmony.Finder.GameController.AllLevelsCompleted && Harmony.Finder.GameController.DifficultyLevel == DifficultyLevel.Hard));
            Achievements.Add(new Achievement(Harmony.Finder.GameSettings.DefeatBlackKnight,  () => Harmony.Finder.GameController.LevelsCompleted.Contains(Harmony.Finder.GameSettings.Level5SceneName)));
            Achievements.Add(new Achievement(Harmony.Finder.GameSettings.ReachFinalLevelWith8Players,  () => HumanPlayer.Instance.NumberOfUnits > 8 && Harmony.Finder.GameController.CurrentLevelName == Harmony.Finder.GameSettings.MorktressSceneName));
            Achievements.Add(new Achievement(Harmony.Finder.GameSettings.FinishALevelWithoutUnitLoss,  () => !HumanPlayer.Instance.HasLostAUnitInCurrentLevel && Harmony.Finder.GameController.LevelsCompleted.Contains(Harmony.Finder.GameController.CurrentLevelName)));
            Achievements.Add(new Achievement(Harmony.Finder.GameSettings.SaveAllRecruitablesFromAlternatePath,  () => HumanPlayer.Instance.NumberOfRecruitedUnitsFromAlternatePath >= Harmony.Finder.GameSettings.NumberOfRecruitablesOnAlternatePath));
            Achievements.Add(new Achievement(Harmony.Finder.GameSettings.FinishCampaignWithoutUnitLoss,  () => !HumanPlayer.Instance.HasEverLostAUnit && Harmony.Finder.GameController.LevelsCompleted.Count >= Harmony.Finder.GameController.Levels.Count ));
            nameText.text = Harmony.Finder.GameSettings.AchievementGetString;
        }

        private void Update()
        {
            CheckIfAchievementCompleted();
        }

        public void CheckIfAchievementCompleted()
        {
            foreach (var achievement in Achievements)
            {
                if (achievement.AchievementCompleted)
                {
                    if(!achievement.AchievementHasBeenShown && !AchievementBeingShown)
                            ShowAchievement(achievement);
                }
            }
        }

        public void ShowAchievement(Achievement achievement)
        {
            achievement.SetAchievementHasBeenShown();
            StartAchievement(achievement);
        }
        
        public void StartAchievement(Achievement achievement)
        {
            animator.SetBool("IsOpen",true);
            AchievementBeingShown = true;
            StartCoroutine(TypeAchievementText(achievement.AchievementName));
        }

        IEnumerator TypeAchievementText(string text)
        {
            nameText.text = "";
            descriptionText.text = "";
            yield return new WaitForSeconds(1);
            for (int i = 0; i < Harmony.Finder.GameSettings.AchievementGetString.Length; i++)
            {
                nameText.text += Harmony.Finder.GameSettings.AchievementGetString[i];
                yield return new WaitForSeconds(0.1f);
            }
            for(int i = 0; i < text.Length; i++)
            {
                descriptionText.text += text[i];
                yield return new WaitForSeconds(0.2f);
            }
            EndAchievementShow();
        }
    
        private void EndAchievementShow()
        {
            animator.SetBool("IsOpen",false);
            AchievementBeingShown = false;
        }
    }
}