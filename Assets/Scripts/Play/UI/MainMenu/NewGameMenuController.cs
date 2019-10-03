using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class NewGameMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Dropdown difficultyDropdownMenu;
        [SerializeField] private InputField playerNameInputField;
        [SerializeField] private Button startNewGameButton;
        [SerializeField] private Button returnToMainMenuButton;
        
        [Header("Controls")]
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0; 
        [SerializeField] private KeyCode returnToMainMenuKey = KeyCode.Escape;

        private Canvas newGameMenuCanvas;

        private void Awake()
        {
            newGameMenuCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            newGameMenuCanvas.enabled = true;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(confirmKey))
                UIExtensions.SelectedButton?.Click();
            else if (Input.GetKeyDown(returnToMainMenuKey))
                ReturnToMainMenu();
        }

        [UsedImplicitly]
        public void StartNewGame()
        {
            //SAUVEGARDER PRÉFÉRENCES DU JOUEUR
            //CHARGER NOUVELLE SCENE
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            
        }
    }
}