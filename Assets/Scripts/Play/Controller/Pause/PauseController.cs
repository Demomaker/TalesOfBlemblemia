using UnityEngine;

namespace Game
{
    //Author: Antoine Lessard
    public class PauseController : MonoBehaviour
    {
        [Header("Controller")] 
        [SerializeField] private PauseMenuController pauseMenuController;
        
        [Header("Canvases")] 
        [SerializeField] private Canvas pauseMenuCanvas;
        [SerializeField] private Canvas optionsMenuCanvas;

        [Header("Controls")] 
        [SerializeField] private KeyCode pauseButton = KeyCode.Escape;

        private bool isPaused;

        private void Awake()
        {
            isPaused = false;
        }

        private void Start()
        {
            pauseMenuCanvas.enabled = false;
            optionsMenuCanvas.enabled = false;
        }

        private void Update()
        {
            if (!isPaused)
            {
                if (Input.GetKeyDown(pauseButton)) 
                    Pause();
            }
            else
            {
                if (Input.GetKeyDown(pauseButton) && pauseMenuCanvas.enabled) 
                    Resume();
            }
        }

        private void Pause()
        {
            pauseMenuController.Enter();
            Time.timeScale = 0;
            isPaused = true;
        }

        public void Resume()
        {
            pauseMenuController.Leave();
            Time.timeScale = 1;
            isPaused = false;
        }
    }
}