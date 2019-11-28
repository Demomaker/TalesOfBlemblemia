using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Author: Antoine Lessard
namespace Game
{
    public class LoadGameMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button[] saveSlots;
        
        private Navigator navigator;
        private SaveController saveController;
        private GameSettings gameSettings;
        private Canvas loadGameScreen;
        private LevelLoader levelLoader;
        private GameController gameController;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            loadGameScreen = GetComponent<Canvas>();
            levelLoader = Harmony.Finder.LevelLoader;
            gameController = Harmony.Finder.GameController;
        }
        
        private void Start()
        {
            var saveCounter = 0;

            var saves = saveController.GetSaves();
            
            foreach (var saveSlot in saveSlots)
            {
                saveSlot.transform.Find(gameSettings.NameString).GetComponent<TMP_Text>().text = saves[saveCounter].Username;
                saveSlot.transform.Find(gameSettings.StageString).GetComponent<TMP_Text>().text = saveSlot.name = saves[saveCounter].LevelName;
                saveSlot.transform.Find(gameSettings.DifficultyString).GetComponent<TMP_Text>().text =
                    saves[saveCounter].DifficultyLevel;

                ++saveCounter;
            }
        }

        public void Enter()
        {
            navigator.Enter(loadGameScreen);
        }

        [UsedImplicitly]
        public void SaveSlotSelected(int saveSlotId)
        {
            saveSlotId = Mathf.Clamp(saveSlotId, 0, 2);
            saveController.SaveSelected = saveSlotId + 1;
            var difficultyLevel = saveSlots[saveSlotId].transform.Find(gameSettings.DifficultyString)
                .GetComponent<TMP_Text>().text;
            switch (difficultyLevel)
            {
                case "Easy":
                    gameController.DifficultyLevel = DifficultyLevel.Easy;
                    break;
                case "Medium":
                    gameController.DifficultyLevel = DifficultyLevel.Medium;
                    break;
                case "Hard":
                    gameController.DifficultyLevel = DifficultyLevel.Hard;
                    break;
                default:
                    gameController.DifficultyLevel = DifficultyLevel.Medium;
                    break;
            }

            gameController.PreviousLevelName = saveSlots[saveSlotId].transform.Find(gameSettings.StageString)
                .GetComponent<TMP_Text>().text;

            LoadOverWorld();
        }

        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            navigator.Leave();
        }

        private void LoadOverWorld()
        {
            levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
        }
    }
}