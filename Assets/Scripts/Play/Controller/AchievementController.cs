using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Controls the achievements and sends messages if achievements are completed
    /// Author : Mike Bédard, Antoine Lessard
    /// </summary>
    public class AchievementController : MonoBehaviour
    {
        private static readonly int IS_OPEN = Animator.StringToHash("IsOpen");
        [SerializeField] private Text achievementUnlockedText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Animator animator;
        [SerializeField] private Button skipButton;
        
        private List<AchievementInfo> achievements = new List<AchievementInfo>();
        private bool achievementBeingShown;
        private bool skipAchievementShow = false;
        private GameSettings gameSettings;
        private GameController gameController;
        private SaveController saveController;

        private bool CompletedCampaignOnEasy =>
            gameController.AllLevelsCompleted && (gameController.DifficultyLevel == DifficultyLevel.Easy ||
                                                  gameController.DifficultyLevel == DifficultyLevel.Medium ||
                                                  gameController.DifficultyLevel == DifficultyLevel.Hard);
        private bool CompletedCampaignOnMedium =>
            gameController.AllLevelsCompleted && (gameController.DifficultyLevel == DifficultyLevel.Medium || gameController.DifficultyLevel == DifficultyLevel.Hard);
        private bool CompletedCampaignOnHard =>
            gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Hard;
        private bool BlackKnightDefeated => gameController.PreviousLevelName == gameSettings.DarkTowerSceneName;
        private bool ReachedFinalLevelWithAllPlayableUnits
        {
            get
            {
                const int numberOfUnitsNeededForAchievement = 8;
                return HumanPlayer.Instance.NumberOfUnits > numberOfUnitsNeededForAchievement &&
                       gameController.CurrentLevelName == gameSettings.MorktressSceneName;
            }
        }

        private bool FinishedALevelWithoutUnitLoss =>
            !HumanPlayer.Instance.HasLostAUnitInCurrentLevel &&
            gameController.PreviousLevelName == gameController.CurrentLevelName &&
            !string.IsNullOrEmpty(gameController.PreviousLevelName) &&
            gameController.PreviousLevelName != gameSettings.TutorialSceneName;
        private bool SavedAllRecruitablesFromAlternatePath =>
            HumanPlayer.Instance.NumberOfRecruitedUnitsFromAlternatePath >=
            gameSettings.NumberOfRecruitablesOnAlternatePath;
        private bool FinishedCampaignWithoutUnitLoss =>
            !HumanPlayer.Instance.HasEverLostAUnit && gameController.AllLevelsCompleted;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            gameController = Harmony.Finder.GameController;
            saveController = Finder.SaveController;
            achievementUnlockedText.text = gameSettings.AchievementUnlockedString;
        }

        private void Start()
        {
            achievements = saveController.Achievements;
        }

        private void OnEnable()
        {
            skipButton.onClick.AddListener(SkipAchievementShow);
        }

        private void OnDisable()
        {
            skipButton.onClick.RemoveListener(SkipAchievementShow);
        }

        private void SkipAchievementShow()
        {
            skipAchievementShow = true;
        }

        private void Update()
        {
            CheckIfAchievementsCompleted();
        }

        private void CheckIfAchievementsCompleted()
        {
            AchievementInfo achievement;
            
            if (CompletedCampaignOnEasy)
            {
                achievement = achievements.Find(info => info.AchievementName == gameSettings.CompleteCampaignOnEasy);
                CheckIfAlreadyCompleted(achievement);
            }

            if (CompletedCampaignOnMedium)
            {
                achievement = achievements.Find(info => info.AchievementName == gameSettings.CompleteCampaignOnMedium);
                CheckIfAlreadyCompleted(achievement);
            }

            if (CompletedCampaignOnHard)
            {
                achievement = achievements.Find(info => info.AchievementName == gameSettings.CompleteCampaignOnHard);
                CheckIfAlreadyCompleted(achievement);
            }

            if (BlackKnightDefeated)
            {
                achievement = achievements.Find(info => info.AchievementName == gameSettings.DefeatBlackKnight);
                CheckIfAlreadyCompleted(achievement);
            }

            if (ReachedFinalLevelWithAllPlayableUnits)
            {
                achievement = achievements.Find(info => info.AchievementName == gameSettings.ReachFinalLevelWith8Players);
                CheckIfAlreadyCompleted(achievement);
            }

            if (FinishedALevelWithoutUnitLoss)
            {
                achievement = achievements.Find(info => info.AchievementName == gameSettings.FinishALevelWithoutUnitLoss);
                CheckIfAlreadyCompleted(achievement);
            }

            if (FinishedCampaignWithoutUnitLoss)
            {
                achievement = achievements.Find(info => info.AchievementName == gameSettings.FinishCampaignWithoutUnitLoss);
                CheckIfAlreadyCompleted(achievement);
            }

            if (SavedAllRecruitablesFromAlternatePath)
            {
                achievement = achievements.Find(info => info.AchievementName == gameSettings.SaveAllRecruitablesFromAlternatePath);
                CheckIfAlreadyCompleted(achievement);
            }
        }

        private void CheckIfAlreadyCompleted(AchievementInfo achievement)
        {
            if (achievement.Achieved) return;
            achievement.Achieved = true;
            saveController.UpdateAchievements(achievements);
            StartAchievement(achievement);
        }

        private void StartAchievement(AchievementInfo achievement)
        {
            animator.SetBool(IS_OPEN,true);
            achievementBeingShown = true;
            StartCoroutine(TypeAchievementText(achievement.AchievementName));
        }

        private IEnumerator TypeAchievementText(string text)
        {
            achievementUnlockedText.text = "";
            descriptionText.text = "";
            const float secondsBeforeTypingStart = 1;
            const float secondsBeforeTitleCharacterPrint = 0.1f;
            const float secondsBeforeTextCharacterPrint = 0.05f;
            yield return new WaitForSeconds(secondsBeforeTypingStart);
            foreach (var character in gameSettings.AchievementUnlockedString)
            {
                if (skipAchievementShow) break;
                achievementUnlockedText.text += character;
                yield return new WaitForSeconds(secondsBeforeTitleCharacterPrint);
            }

            achievementUnlockedText.text = gameSettings.AchievementUnlockedString;
            foreach (var character in text)
            {
                if (skipAchievementShow) break;
                descriptionText.text += character;
                yield return new WaitForSeconds(secondsBeforeTextCharacterPrint);
            }
            descriptionText.text = text;
            EndAchievementShow();
        }
    
        private void EndAchievementShow()
        {
            animator.SetBool(IS_OPEN,false);
            achievementBeingShown = false;
            skipAchievementShow = false;
        }
    }
}