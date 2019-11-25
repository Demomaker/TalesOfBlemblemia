using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Author: Antoine Lessard
namespace Game
{
    public class NewGameMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private TMP_Dropdown difficultyDropdownMenu;
        [SerializeField] private TMP_InputField playerNameInputField;
        [SerializeField] private Button startNewGameButton;
        [SerializeField] private Button returnToMainMenuButton;

        private const int NUMBER_OF_MENUS_TO_GO_BACK_TO_MAIN = 2;
        private LevelLoader levelLoader;
        
        private Navigator navigator;
        private SaveController saveController;
        private Canvas newGameScreen;
        private GameSettings gameSettings;
        private GameController gameController;

        private int saveSlotSelectedNumber;

        private void Awake()
        {
            levelLoader = Harmony.Finder.LevelLoader;
            gameSettings = Harmony.Finder.GameSettings;
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            newGameScreen = GetComponent<Canvas>();
            saveSlotSelectedNumber = 0;
            gameController = Finder.GameController;
        }

        public void Enter(int saveSlotNumber)
        {
            navigator.Enter(newGameScreen);
            saveSlotSelectedNumber = saveSlotNumber;
        }
        
        [UsedImplicitly]
        public void StartNewGame()
        {
            saveController.SaveSelected = saveSlotSelectedNumber;
            
            //if the player did not enter a name, player name will be Franklem
            if (playerNameInputField.text == "")
            {
                playerNameInputField.text = "Franklem";
            }

            var saves = saveController.GetSaves();
            
            saveController.ResetSave();
            
            saves[saveSlotSelectedNumber - 1].Username = playerNameInputField.text;
            saves[saveSlotSelectedNumber - 1].DifficultyLevel = difficultyDropdownMenu.options[difficultyDropdownMenu.value].text;

            switch (difficultyDropdownMenu.options[difficultyDropdownMenu.value].text)
            {
                case "Easy":
                    gameController.DifficultyLevel = DifficultyLevel.Easy;
                    break;
                case "Hard":
                    gameController.DifficultyLevel = DifficultyLevel.Hard;
                    break;
                default:
                    gameController.DifficultyLevel = DifficultyLevel.Medium;
                    break;
            }
            
            //TODO:check pour le fix
            saves[saveSlotSelectedNumber - 1].LevelName = gameSettings.ParabeneForestSceneName;
            gameController.PreviousLevelName = gameSettings.SnowyPeaksSceneName;
            
            saveController.UpdateSave(saveSlotSelectedNumber);
            levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            playerNameInputField.text = "";
            saveSlotSelectedNumber = 0;
            navigator.Leave(NUMBER_OF_MENUS_TO_GO_BACK_TO_MAIN);
        }
    }
}