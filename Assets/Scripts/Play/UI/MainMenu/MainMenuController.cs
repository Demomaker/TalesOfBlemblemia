using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button newGameButton = null;
        [SerializeField] private Button loadGameButton = null;
        [SerializeField] private Button optionsButton = null;
        [SerializeField] private Button creditsButton = null;
        [SerializeField] private Button exitGameButton = null;

        [Header("Controls")] 
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0; 
        [SerializeField] private KeyCode exitKey = KeyCode.Escape;

        private Canvas mainMenuCanvas;

        private void Awake()
        {
            mainMenuCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            mainMenuCanvas.enabled = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(confirmKey))
                UIExtensions.SelectedButton?.Click();
            else if(Input.GetKeyDown(exitKey))
                Exit();
        }

        [UsedImplicitly]
        public void StartNewGame()
        {
            //Start nouvelle scene du jeu
        }

        [UsedImplicitly]
        public void LoadGame()
        {
            //Interface avec les saves
        }

        [UsedImplicitly]
        public void Options()
        {
            //Interface d'options;
        }

        [UsedImplicitly]
        public void Credits()
        {
            //Interface de credits
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