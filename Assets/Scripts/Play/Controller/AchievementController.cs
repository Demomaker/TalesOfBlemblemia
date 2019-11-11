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

        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Animator animator;
        private List<Achievement> Achievements  = new List<Achievement>();
        private bool AchievementBeingShown;
        private GameSettings gameSettings;
        private GameController gameController;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            gameController = Harmony.Finder.GameController;
            Achievements.Add(new Achievement(gameSettings.CompleteCampaignOnEasy,  () => gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Easy));
            Achievements.Add(new Achievement(gameSettings.CompleteCampaignOnMedium,  () => gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Medium));
            Achievements.Add(new Achievement(gameSettings.CompleteCampaignOnHard,  () => gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Hard));
            Achievements.Add(new Achievement(gameSettings.DefeatBlackKnight,  () => gameController.PreviousLevelName == gameSettings.DarkTowerSceneName));
            Achievements.Add(new Achievement(gameSettings.ReachFinalLevelWith8Players,  () => HumanPlayer.Instance.NumberOfUnits > 8 && gameController.CurrentLevelName == gameSettings.MorktressSceneName));
            Achievements.Add(new Achievement(gameSettings.FinishALevelWithoutUnitLoss,  () => !HumanPlayer.Instance.HasLostAUnitInCurrentLevel && gameController.PreviousLevelName == gameController.CurrentLevelName && !string.IsNullOrEmpty(gameController.PreviousLevelName)));
            Achievements.Add(new Achievement(gameSettings.SaveAllRecruitablesFromAlternatePath,  () => HumanPlayer.Instance.NumberOfRecruitedUnitsFromAlternatePath >= gameSettings.NumberOfRecruitablesOnAlternatePath));
            Achievements.Add(new Achievement(gameSettings.FinishCampaignWithoutUnitLoss,  () => !HumanPlayer.Instance.HasEverLostAUnit && gameController.AllLevelsCompleted ));
            nameText.text = gameSettings.AchievementGetString;
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
            for (int i = 0; i < gameSettings.AchievementGetString.Length; i++)
            {
                nameText.text += gameSettings.AchievementGetString[i];
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