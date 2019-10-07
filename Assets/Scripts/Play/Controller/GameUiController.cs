using UnityEngine;

namespace Game
{
    public class GameUiController : MonoBehaviour
    {
        [Header("Canvas")] 
        [SerializeField] private Canvas pauseMenuCanvas;
        [SerializeField] private Canvas optionsCanvas;

        private PauseMenuController pauseMenuController;
        private OptionsMenuController optionsMenuController;

        private IMenuController activeMenuController;

        private void Awake()
        {
            pauseMenuController = GetComponent<PauseMenuController>();
            optionsMenuController = GetComponent<OptionsMenuController>();
        }

        private void Start()
        {
            pauseMenuCanvas.enabled = false;
            optionsCanvas.enabled = false;
            
            activeMenuController = pauseMenuController;
        }

        public void Update()
        {
            if (activeMenuController == null && Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0;
                activeMenuController = pauseMenuController;
                pauseMenuCanvas.enabled = true;
            }
            activeMenuController?.Update();
        }

        public void GoToOptionsMenu()
        {
            pauseMenuCanvas.enabled = false;
            optionsCanvas.enabled = true;
            activeMenuController = optionsMenuController;
        }

        public void ReturnFromOptionsMenu()
        {
            optionsCanvas.enabled = false;
            pauseMenuCanvas.enabled = true;
            activeMenuController = pauseMenuController;
        }

        public void UnpauseGame()
        {
            pauseMenuCanvas.enabled = false;
            activeMenuController = null;
            Time.timeScale = 1;
        }
    }
}