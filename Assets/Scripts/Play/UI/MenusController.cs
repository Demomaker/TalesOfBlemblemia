using UnityEngine;

namespace Game
{
    public class MenusController : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField] private Canvas mainMenuCanvas;
        [SerializeField] private Canvas newGameMenuCanvas;
        [SerializeField] private Canvas loadGameMenuCanvas;
        [SerializeField] private Canvas optionsMenuCanvas;
        [SerializeField] private Canvas creditsMenuCanvas;
        [SerializeField] private Canvas saveSlotSelectionCanvas;

        private MainMenuController mainMenuController;
        private NewGameMenuController newGameMenuController;
        private LoadGameMenuController loadGameMenuController;
        private OptionsMenuController optionsMenuController;
        private CreditsMenuController creditsMenuController;
        private SaveSlotSelectionController saveSlotSelectionController;

        private void Awake()
        {
            mainMenuController = mainMenuCanvas.GetComponent<MainMenuController>();
            newGameMenuController = newGameMenuCanvas.GetComponent<NewGameMenuController>();
            loadGameMenuController = loadGameMenuCanvas.GetComponent<LoadGameMenuController>();
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
        }

        public void GoToSaveSelectionMenu()
        {
            mainMenuCanvas.enabled = false;
            saveSlotSelectionCanvas.enabled = true;

        }
        public void GoToNewGameMenu(int saveNumber)
        {
            saveSlotSelectionCanvas.enabled = false;
            newGameMenuController.SaveSlotSelectedNumber = saveNumber;
            newGameMenuCanvas.enabled = true;
            
        }
        
        public void GoToLoadGameMenu()
        {
            mainMenuCanvas.enabled = false;
            loadGameMenuCanvas.enabled = true;
        }
        
        public void GoToOptionsMenu()
        {
            mainMenuCanvas.enabled = false;
            optionsMenuCanvas.enabled = true;
        }

        public void GoToCreditsMenu()
        {
            mainMenuCanvas.enabled = false;
            creditsMenuCanvas.enabled = true;
        }

        public void ReturnFromNewGameMenu()
        {
            newGameMenuCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
        }

        public void ReturnFromOptionsMenu()
        {
            optionsMenuCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
        }

        public void ReturnFromLoadGameMenu()
        {
            loadGameMenuCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
        }
    }
}