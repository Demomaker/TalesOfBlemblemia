using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    //Author : Antoine Lessard
    public class AchievementsMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button returnToMainMenuButton;

        [Header("Achievements")] 
        [SerializeField] private Image[] achievements;
        
        private Navigator navigator;
        private SaveController saveController;
        private GameSettings gameSettings;
        private GameController gameController;
        private Canvas achievementsScreen;

        private void Awake()
        {
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            gameController = Finder.GameController;
            gameSettings = Harmony.Finder.GameSettings;
            achievementsScreen = GetComponent<Canvas>();
        }

        private void Start()
        {
            var achievementsInfo = saveController.Achievements;

            var achievementsCounter = 0;

            foreach (var container in achievements)
            {
                container.transform.Find(gameSettings.NameString).GetComponent<TMP_Text>().text =
                    achievementsInfo[achievementsCounter].AchievementName;
                container.transform.Find(gameSettings.DescriptionString).GetComponent<TMP_Text>().text =
                    achievementsInfo[achievementsCounter].AchievementDescription;

                if (!achievementsInfo[achievementsCounter].Achieved)
                {
                    var tempColor = container.color;
                    tempColor = gameSettings.PaleAlpha;
                    container.color = tempColor;
                }

                ++achievementsCounter;
            }
        }

        public void Enter()
        {
            navigator.Enter(achievementsScreen);
        }

        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            navigator.Leave();
        }
    }
}