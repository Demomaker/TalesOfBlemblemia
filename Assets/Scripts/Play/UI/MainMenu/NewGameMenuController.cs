using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

namespace Game
{
    public class NewGameMenuController : MonoBehaviour, IMenuController
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

        private void Awake()
        {
            menusController = Finder.MenusController;
            saveController = Finder.SaveController;
        }

        public void Update()
        {
            if (Input.GetKeyDown(confirmKey))
                UIExtensions.SelectedButton?.Click();
        }

        [UsedImplicitly]
        public void StartNewGame()
        {
            //if the player did not enter a name, player name will be Franklem
            if (playerNameInputField.text == "")
            {
                playerNameInputField.text = "Franklem";
            }
            
            saveController.saveInfos.username = playerNameInputField.text;
            saveController.saveInfos.difficultyLevel = difficultyDropdownMenu.options[difficultyDropdownMenu.value].text;
            
            saveController.CreateSave();
            //CHARGER NOUVELLE SCENE
            DontDestroyOnLoad(saveController);
            SceneManager.LoadScene("ParabeneForest");
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            menusController.ReturnFromNewGameMenu();
        }
    }
}