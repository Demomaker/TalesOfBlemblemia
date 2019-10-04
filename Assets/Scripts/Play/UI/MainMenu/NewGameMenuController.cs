using JetBrains.Annotations;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Game
{
    public class NewGameMenuController : MonoBehaviour, IMenuController
    {
        [Header("Buttons")] 
        [SerializeField] private Dropdown difficultyDropdownMenu;
        [SerializeField] private InputField playerNameInputField;
        [SerializeField] private Button startNewGameButton;
        [SerializeField] private Button returnToMainMenuButton;
        
        [Header("Controls")]
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0;

        private MenusController menusController;

        private void Awake()
        {
            menusController = Finder.MenusController;
        }

        public void Update()
        {
            if (Input.GetKeyDown(confirmKey))
                UIExtensions.SelectedButton?.Click();
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
            menusController.ReturnFromNewGameMenu();
        }
    }
}