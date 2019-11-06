﻿using JetBrains.Annotations;
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
            saveSlotSelectedNumber = saveSlotNumber;
        }
        
        [UsedImplicitly]
        public void StartNewGame()
        {
            //if the player did not enter a name, player name will be Franklem
            if (playerNameInputField.text == "")
            {
                playerNameInputField.text = "Franklem";
            }

            var saves = saveController.GetSaves();
            
            saves[saveSlotSelectedNumber - 1].Username = playerNameInputField.text;
            saves[saveSlotSelectedNumber - 1].DifficultyLevel = difficultyDropdownMenu.options[difficultyDropdownMenu.value].text;

            
            saveController.UpdateSave(saveSlotSelectedNumber);
            saveController.SaveSelected = saveSlotSelectedNumber;
            SceneManager.LoadSceneAsync(Constants.OVERWORLD_SCENE_NAME, LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(Constants.MAINMENU_SCENE_NAME);
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