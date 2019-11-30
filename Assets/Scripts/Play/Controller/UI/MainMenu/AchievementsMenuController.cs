using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    //Author : Antoine Lessard
    public class AchievementsMenuController : MonoBehaviour
    {
        [Header("Achievements")] 
        [SerializeField] private Image[] achievements;
        
        private Navigator navigator;
        private SaveController saveController;
        private GameSettings gameSettings;
        private Canvas achievementsScreen;

        private void Awake()
        {
            navigator = Harmony.Finder.Navigator;
            saveController = Harmony.Finder.SaveController;
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
                    //We have to store the color to change it, otherwise it is not possible because it is a struct
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