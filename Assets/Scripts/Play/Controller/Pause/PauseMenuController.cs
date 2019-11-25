using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    //Author: Antoine Lessard
    public class PauseMenuController : MonoBehaviour
    {
        [Header("Controller")] 
        [SerializeField] private OptionsMenuController optionsMenuController;
        
        private Navigator navigator;
        private Canvas pauseMenuScreen;
        private PauseController pauseController;
        private LevelLoader levelLoader;
        private GameSettings gameSettings;

        private void Start()
        {
            navigator = Finder.Navigator;
            pauseMenuScreen = GetComponent<Canvas>();
            pauseController = Finder.PauseController;
            levelLoader = Harmony.Finder.LevelLoader;
            gameSettings = Harmony.Finder.GameSettings;
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
            levelLoader.FadeToLevel(gameSettings.MainmenuSceneName, LoadSceneMode.Additive);
        }

        [UsedImplicitly]
        public void ReturnToOverWorld()
        {
            pauseController.Resume();
            levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
        }
    }
}