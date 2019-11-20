using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Button skipButton;
        
        private static readonly int IS_OPEN = Animator.StringToHash("IsOpen");
        private readonly List<Achievement> achievements = new List<Achievement>();
        private bool achievementBeingShown;
        private bool skipAchievementShow = false;
        private GameSettings gameSettings;
        private GameController gameController;

        private bool CompletedCampaignOnEasy =>
            gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Easy;
        private bool CompletedCampaignOnMedium =>
            gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Medium;
        private bool CompletedCampaignOnHard =>
            gameController.AllLevelsCompleted && gameController.DifficultyLevel == DifficultyLevel.Hard;
        private bool BlackKnightDefeated => gameController.PreviousLevelName == gameSettings.DarkTowerSceneName;
        private bool ReachedFinalLevelWithCertainAmountOfUnits
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
            achievements.Add(new Achievement(gameSettings.ReachFinalLevelWith8Players,  () => ReachedFinalLevelWithCertainAmountOfUnits));
            achievements.Add(new Achievement(gameSettings.FinishALevelWithoutUnitLoss,  () => FinishedALevelWithoutUnitLoss));
            achievements.Add(new Achievement(gameSettings.SaveAllRecruitablesFromAlternatePath,  () => SavedAllRecruitablesFromAlternatePath));
            achievements.Add(new Achievement(gameSettings.FinishCampaignWithoutUnitLoss,  () => FinishedCampaignWithoutUnitLoss));
            nameText.text = gameSettings.AchievementGetString;
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
            CheckIfAchievementCompleted();
        }

        private void CheckIfAchievementCompleted()
        {
            foreach (var achievement in achievements.Where(achievement => achievement.AchievementCompleted).Where(achievement => !achievement.AchievementHasBeenShown && !achievementBeingShown))
            {
                ShowAchievement(achievement);
            }
        }

        private void ShowAchievement(Achievement achievement)
        {
            achievement.SetAchievementHasBeenShown();
            StartAchievement(achievement);
        }

        private void StartAchievement(Achievement achievement)
        {
            animator.SetBool(IS_OPEN,true);
            achievementBeingShown = true;
            StartCoroutine(TypeAchievementText(achievement.AchievementName));
        }

        private IEnumerator TypeAchievementText(string text)
        {
            nameText.text = "";
            descriptionText.text = "";
            const float secondsBeforeTypingStart = 1;
            const float secondsBeforeTitleCharacterPrint = 0.1f;
            const float secondsBeforeTextCharacterPrint = 0.2f;
            yield return new WaitForSeconds(secondsBeforeTypingStart);
            foreach (var character in gameSettings.AchievementGetString)
            {
                if (skipAchievementShow) break;
                nameText.text += character;
                yield return new WaitForSeconds(secondsBeforeTitleCharacterPrint);
            }

            nameText.text = gameSettings.AchievementGetString;
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