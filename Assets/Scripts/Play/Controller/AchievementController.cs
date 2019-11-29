using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Controls the achievements and sends messages if achievements are completed
    /// Author : Mike Bédard, Antoine Lessard
    /// </summary>
    [Findable("AchievementController")]
    public class AchievementController : MonoBehaviour
    {
        private static readonly int IS_OPEN = Animator.StringToHash("IsOpen");
        
        [SerializeField] private Text achievementUnlockedText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Animator animator;
        [SerializeField] private Button skipButton;
        
        private List<AchievementInfo> achievements = new List<AchievementInfo>();
        private bool achievementBeingShown;
        private bool skipAchievementShow;
        private GameSettings gameSettings;
        private SaveController saveController;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
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
        
        public void UnlockAchievement(string achievementName)
        {
            var achievement = achievements.Find(info => info.AchievementName == achievementName);
            if (achievement == null || achievement.Achieved) return;
            achievement.Achieved = true;
            saveController.UpdateAchievements(achievements);
            ShowAchievement(achievement);
        }

        private void SkipAchievementShow()
        {
            skipAchievementShow = true;
        }
        private void ShowAchievement(AchievementInfo achievement)
        {
            animator.SetBool(IS_OPEN,true);
            achievementBeingShown = true;
            Harmony.Finder.CoroutineStarter.StartCoroutine(TypeAchievementText(achievement.AchievementName));
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
            animator.SetBool(IS_OPEN,false);
            achievementBeingShown = false;
            skipAchievementShow = false;
        }
    }
}