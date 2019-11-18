using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI.Achievement;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Controls the achievements and sends messages if achievements are completed
    /// Author : Mike Bédard
    /// </summary>
    public class AchievementController : MonoBehaviour
    {

        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Animator animator;
        private readonly List<Achievement> achievements = new List<Achievement>();
        private bool AchievementBeingShown;
        private GameSettings gameSettings;
        private GameController gameController;
        
        private bool CompletedCampaignOnEasy =>
            gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Easy;
        private bool CompletedCampaignOnMedium =>
            gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Medium;
        private bool CompletedCampaignOnHard =>
            gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Hard;
        private bool BlackKnightDefeated => gameController.PreviousLevelName == gameSettings.DarkTowerSceneName;
        private bool ReachedFinalLevelWith8Players =>
            HumanPlayer.Instance.NumberOfUnits > 8 &&
            gameController.CurrentLevelName == gameSettings.MorktressSceneName;
        private bool FinishedALevelWithoutUnitLoss =>
            !HumanPlayer.Instance.HasLostAUnitInCurrentLevel &&
            gameController.PreviousLevelName == gameController.CurrentLevelName &&
            !string.IsNullOrEmpty(gameController.PreviousLevelName);
        private bool SavedAllRecruitablesFromAlternatePath =>
            HumanPlayer.Instance.NumberOfRecruitedUnitsFromAlternatePath >=
            gameSettings.NumberOfRecruitablesOnAlternatePath;
        private bool FinishedCampaignWithoutUnitLoss =>
            !HumanPlayer.Instance.HasEverLostAUnit && gameController.AllLevelsCompleted;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            gameController = Harmony.Finder.GameController;
            achievements.Add(new Achievement(gameSettings.CompleteCampaignOnEasy,  () => CompletedCampaignOnEasy));
            achievements.Add(new Achievement(gameSettings.CompleteCampaignOnMedium,  () => CompletedCampaignOnMedium));
            achievements.Add(new Achievement(gameSettings.CompleteCampaignOnHard,  () => CompletedCampaignOnHard));
            achievements.Add(new Achievement(gameSettings.DefeatBlackKnight,  () => BlackKnightDefeated));
            achievements.Add(new Achievement(gameSettings.ReachFinalLevelWith8Players,  () => ReachedFinalLevelWith8Players));
            achievements.Add(new Achievement(gameSettings.FinishALevelWithoutUnitLoss,  () => FinishedALevelWithoutUnitLoss));
            achievements.Add(new Achievement(gameSettings.SaveAllRecruitablesFromAlternatePath,  () => SavedAllRecruitablesFromAlternatePath));
            achievements.Add(new Achievement(gameSettings.FinishCampaignWithoutUnitLoss,  () => FinishedCampaignWithoutUnitLoss));
            nameText.text = gameSettings.AchievementGetString;
        }

        private void Update()
        {
            CheckIfAchievementCompleted();
        }

        public void CheckIfAchievementCompleted()
        {
            foreach (var achievement in achievements)
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