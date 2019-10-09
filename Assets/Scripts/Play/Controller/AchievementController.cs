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
            Achievements.Add(new Achievement(Constants.AchievementName.COMPLETE_CAMPAIGN_ON_EASY,  () => Harmony.Finder.GameController.AllLevelsCompleted && Harmony.Finder.GameController.DifficultyLevel == DifficultyLevel.Easy));
            Achievements.Add(new Achievement(Constants.AchievementName.COMPLETE_CAMPAIGN_ON_MEDIUM,  () => Harmony.Finder.GameController.AllLevelsCompleted && Harmony.Finder.GameController.DifficultyLevel == DifficultyLevel.Medium));
            Achievements.Add(new Achievement(Constants.AchievementName.COMPLETE_CAMPAIGN_ON_HARD,  () => Harmony.Finder.GameController.AllLevelsCompleted && Harmony.Finder.GameController.DifficultyLevel == DifficultyLevel.Hard));
            Achievements.Add(new Achievement(Constants.AchievementName.DEFEAT_BLACK_KNIGHT,  () => Harmony.Finder.GameController.LevelsCompleted.Contains(Constants.LEVEL_5_SCENE_NAME)));
            Achievements.Add(new Achievement(Constants.AchievementName.REACH_FINAL_LEVEL_WITH_8_PLAYERS,  () => HumanPlayer.Instance.NumberOfUnits > 8 && Harmony.Finder.GameController.CurrentLevelName == Constants.LEVEL_8_SCENE_NAME));
            Achievements.Add(new Achievement(Constants.AchievementName.FINISH_A_LEVEL_WITHOUT_UNIT_LOSS,  () => !HumanPlayer.Instance.HasLostAUnitInCurrentLevel && Harmony.Finder.GameController.LevelsCompleted.Contains(Harmony.Finder.GameController.CurrentLevelName)));
            Achievements.Add(new Achievement(Constants.AchievementName.SAVE_ALL_RECRUITABLES_FROM_ALTERNATE_PATH,  () => HumanPlayer.Instance.NumberOfRecruitedUnitsFromAlternatePath >= Constants.NUMBER_OF_RECRUITABLES_ON_ALTERNATE_PATH));
            Achievements.Add(new Achievement(Constants.AchievementName.FINISH_CAMPAIGN_WITHOUT_UNIT_LOSS,  () => !HumanPlayer.Instance.HasEverLostAUnit && Harmony.Finder.GameController.LevelsCompleted.Count >= Harmony.Finder.GameController.Levels.Count ));
            //Achievement de test, TODO: Enlever dans la version officielle
            Achievements.Add(new Achievement("1 == 1!",  () => 1 == 1));
            nameText.text = Constants.ACHIEVEMENT_GET_STRING;
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
            achievement.AchievementHasBeenShown = true;
            StartAchievement(achievement);
        }
        
        public void StartAchievement(Achievement achievement)
        {
            animator.SetBool("IsOpen",true);
            AchievementBeingShown = true;
            StartCoroutine(TypeAchievementText(achievement.AchievementName));
            //
        }

        IEnumerator TypeAchievementText(string text)
        {
            nameText.text = "";
            descriptionText.text = "";
            yield return new WaitForSeconds(1);
            for (int i = 0; i < Constants.ACHIEVEMENT_GET_STRING.Length; i++)
            {
                nameText.text += Constants.ACHIEVEMENT_GET_STRING[i];
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