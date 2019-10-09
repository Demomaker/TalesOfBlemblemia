using UnityEngine;

namespace Game
{
    public class MenusController : MonoBehaviour
    {
        [SerializeField] private Canvas mainMenuCanvas;
        [SerializeField] private Canvas newGameMenuCanvas;
        [SerializeField] private Canvas loadGameMenuCanvas;
        [SerializeField] private Canvas optionsMenuCanvas;
        [SerializeField] private Canvas creditsMenuCanvas;

        private MainMenuController mainMenuController;
        private NewGameMenuController newGameMenuController;
        private OptionsMenuController optionsMenuController;
        private CreditsMenuController creditsMenuController;

        private IMenuController activeMenuController;
        
        private void Awake()
        {
            mainMenuController = mainMenuCanvas.GetComponent<MainMenuController>();
            newGameMenuController = newGameMenuCanvas.GetComponent<NewGameMenuController>();
            optionsMenuController = optionsMenuCanvas.GetComponent<OptionsMenuController>();
            creditsMenuController = creditsMenuCanvas.GetComponent<CreditsMenuController>();
        }

        private void Start()
        {
            mainMenuCanvas.enabled = true;
            newGameMenuCanvas.enabled = false;
            loadGameMenuCanvas.enabled = false;
            optionsMenuCanvas.enabled = false;
            creditsMenuCanvas.enabled = false;

            activeMenuController = mainMenuController;
        }

        private void Update()
        {
            activeMenuController.Update();
        }

        public void GoToNewGameMenu()
        {
            mainMenuCanvas.enabled = false;
            newGameMenuCanvas.enabled = true;
            activeMenuController = newGameMenuController;
        }
        
        public void GoToLoadGameMenu()
        {
            mainMenuCanvas.enabled = false;
            loadGameMenuCanvas.enabled = true;
            //activeMenuController = loadGameMenuController;
        }
        
        public void GoToOptionsMenu()
        {
            mainMenuCanvas.enabled = false;
            optionsMenuCanvas.enabled = true;
            activeMenuController = optionsMenuController;
        }

        public void GoToCreditsMenu()
        {
            mainMenuCanvas.enabled = false;
            creditsMenuCanvas.enabled = true;
            activeMenuController = creditsMenuController;
        }

        public void ReturnFromNewGameMenu()
        {
            newGameMenuCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
            activeMenuController = mainMenuController;
        }

        public void ReturnFromOptionsMenu()
        {
            optionsMenuCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
            activeMenuController = mainMenuController;
        }

        public void ReturnFromLoadGameMenu()
        {
            loadGameMenuCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
            activeMenuController = mainMenuController;
        }
    }
}