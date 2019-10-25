﻿using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Controllers")] 
        [SerializeField] private SaveSlotSelectionController saveSlotSelectionController;
        [SerializeField] private LoadGameMenuController loadGameMenuController;
        [SerializeField] private OptionsMenuController optionsMenuController;
        [SerializeField] private CreditsMenuController creditsMenuController;

        [Header("Buttons")] 
        [SerializeField] private Button newGameButton = null;
        [SerializeField] private Button loadGameButton = null;
        [SerializeField] private Button optionsButton = null;
        [SerializeField] private Button creditsButton = null;
        [SerializeField] private Button exitGameButton = null;

        private Canvas mainMenuCanvas;
        private Navigator navigator;

        private void Awake()
        {
            navigator = Finder.Navigator;
            mainMenuCanvas = GetComponent<Canvas>();
        }

        public void Enter()
        {
            navigator.Enter(mainMenuCanvas);
        }

        public void Leave()
        {
            navigator.Leave();
        }

        [UsedImplicitly]
        public void StartNewGame()
        {
            saveSlotSelectionController.Enter();
        }

        [UsedImplicitly]
        public void LoadGame()
        {
            loadGameMenuController.Enter();
        }

        [UsedImplicitly]
        public void Options()
        {
            optionsMenuController.Enter();
        }

        [UsedImplicitly]
        public void Credits()
        {
            creditsMenuController.Enter();
        }

        [UsedImplicitly]
        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}