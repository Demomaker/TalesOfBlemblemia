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
        
        [Header("Controls")]
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0;

        private MenusController menusController;
        private SaveController saveController;

        private int saveSlotSelectedNumber;

        public int SaveSlotSelectedNumber
        {
            get => saveSlotSelectedNumber;
            set => saveSlotSelectedNumber = value;
        }

        private void Awake()
        {
            menusController = Finder.MenusController;
            saveController = Finder.SaveController;
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
            //CHARGER NOUVELLE SCENE
            DontDestroyOnLoad(saveController);
            SceneManager.LoadScene("ParabeneForest");
            SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            menusController.ReturnFromNewGameMenu();
        }
    }
}