using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class CreditsMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button returnToMainMenuButton = null;

        private MenusController menusController;

        private void Awake()
        {
            menusController = Finder.MenusController;
        }

        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            menusController.ReturnFromNewGameMenu();
        }
    }
}