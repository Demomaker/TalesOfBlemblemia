using Harmony;
using UnityEngine;

namespace Game
{
    [Findable("PauseController")]
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
        private CinematicController cinematicController;
        private LevelController levelController;

        private void Awake()
        {
            isPaused = false;
            levelController = Harmony.Finder.LevelController;
        }

        private void Start()
        {
            pauseMenuCanvas.enabled = false;
            optionsMenuCanvas.enabled = false;
            cinematicController = levelController.CinematicController;
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
            if (cinematicController.IsPlayingACinematic) return;
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