﻿using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    //Author: Antoine Lessard
    public class PauseMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button returnToMainMenuButton;

        [Header("Controller")] 
        [SerializeField] private OptionsMenuController optionsMenuController;
        
        private Navigator navigator;
        private Canvas pauseMenuScreen;
        private PauseController pauseController;

        private void Start()
        {
            navigator = Finder.Navigator;
            pauseMenuScreen = GetComponent<Canvas>();
            pauseController = Finder.PauseController;
        }

        public void Enter()
        {
            navigator.Enter(pauseMenuScreen);
        }

        public void Leave()
        {
            navigator.Leave();
        }
        
        [UsedImplicitly]
        public void ResumeGame()
        {
            pauseController.Resume();
        }

        [UsedImplicitly]
        public void GoToOptionsMenu()
        {
            optionsMenuController.Enter();
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            pauseController.Resume();
            SceneManager.LoadSceneAsync("Main");
        }
    }
}