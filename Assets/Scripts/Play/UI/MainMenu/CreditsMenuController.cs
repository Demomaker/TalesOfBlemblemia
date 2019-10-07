using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class CreditsMenuController : MonoBehaviour, IMenuController
    {
        [Header("Buttons")] 
        [SerializeField] private Button returnToMainMenuButton = null;
        
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
        public void ReturnToMainMenu()
        {
            menusController.ReturnFromNewGameMenu();
        }
    }
}