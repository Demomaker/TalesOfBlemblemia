using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PauseMenuController : MonoBehaviour, IMenuController
    {
        [Header("Buttons")] 
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button returnToMainMenuButton;

        [Header("Controls")] 
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0;

        private GameUiController gameUiController;

        private void Awake()
        {
            gameUiController = Finder.GameUiController;
        }

        public void Update()
        {
            if (Input.GetKeyDown(confirmKey))
                UIExtensions.SelectedButton?.Click();
        }

        [UsedImplicitly]
        public void ResumeGame()
        {
            gameUiController.UnpauseGame();
        }
        
        [UsedImplicitly]
        public void Options()
        {
            gameUiController.GoToOptionsMenu();
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            //RETURN À LA SELECTION DE NIVEAUX
        }
    }
}