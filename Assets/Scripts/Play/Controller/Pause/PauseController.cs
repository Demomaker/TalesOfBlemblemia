using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PauseController : MonoBehaviour
    {
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
                if (Input.GetKeyDown(pauseButton)) 
                    Resume();
            }
        }

        private void Pause()
        {
            pauseMenuCanvas.enabled = true;
            Time.timeScale = 0;
            isPaused = true;
        }

        public void Resume()
        {
            pauseMenuCanvas.enabled = false;
            Time.timeScale = 1;
            isPaused = false;
        }

        public void GoToOptions()
        {
            pauseMenuCanvas.enabled = false;
            optionsMenuCanvas.enabled = true;
        }
    }
}