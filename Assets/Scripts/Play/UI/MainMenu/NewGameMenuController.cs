using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        
        private Navigator navigator;
        private SaveController saveController;
        private Canvas newGameScreen;

        private int saveSlotSelectedNumber;

        public int SaveSlotSelectedNumber
        {
            get => saveSlotSelectedNumber;
            set => saveSlotSelectedNumber = value;
        }

        private void Awake()
        {
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            newGameScreen = GetComponent<Canvas>();
            saveSlotSelectedNumber = 0;
        }

        public void Enter(int saveSlotNumber)
        {
            navigator.Enter(newGameScreen);
        }
        
        [UsedImplicitly]
        public void StartNewGame()
        {
            //if the player did not enter a name, player name will be Franklem
            if (playerNameInputField.text == "")
            {
                playerNameInputField.text = "Franklem";
            }

            switch (saveSlotSelectedNumber)
            {
                case 1:
                    saveController.saveSlot1.username = playerNameInputField.text;
                    saveController.saveSlot1.difficultyLevel = difficultyDropdownMenu.options[difficultyDropdownMenu.value].text;
                    break;
                case 2:
                    saveController.saveSlot2.username = playerNameInputField.text;
                    saveController.saveSlot2.difficultyLevel = difficultyDropdownMenu.options[difficultyDropdownMenu.value].text;
                    break;
                case 3:
                    saveController.saveSlot3.username = playerNameInputField.text;
                    saveController.saveSlot3.difficultyLevel = difficultyDropdownMenu.options[difficultyDropdownMenu.value].text;
                    break;
            }
            
            saveController.UpdateSave(saveSlotSelectedNumber);
            DontDestroyOnLoad(saveController);
            SceneManager.LoadScene(Constants.OVERWORLD_SCENE_NAME);

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