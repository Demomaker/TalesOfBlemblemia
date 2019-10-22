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

        private MenusController menusController;

        private void Awake()
        {
            menusController = Finder.MenusController;
        }

        [UsedImplicitly]
        public void StartNewGame()
        {
            //menu newgame
            menusController.GoToSaveSelectionMenu();
        }

        [UsedImplicitly]
        public void LoadGame()
        {
            //Interface avec les saves
            menusController.GoToLoadGameMenu();
        }

        [UsedImplicitly]
        public void Options()
        {
            //Interface d'options;
            menusController.GoToOptionsMenu();
        }

        [UsedImplicitly]
        public void Credits()
        {
            //Interface de credits
            menusController.GoToCreditsMenu();
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