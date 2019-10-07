using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PauseOptionsMenuController : MonoBehaviour, IMenuController
    {
        [Header("Buttons")] 
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider mainVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button returnToPauseMenuButton;
        [SerializeField] private Button applyChangesButton;

        [Header("Controls")] 
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode returnToPauseMenuKey = KeyCode.Escape;

        private GameUiController gameUiController;

        private void Awake()
        {
            gameUiController = Finder.GameUiController;
        }

        public void Update()
        {
            if (Input.GetKeyDown(confirmKey))
                UIExtensions.SelectedButton?.Click();
            else if (Input.GetKeyDown(returnToPauseMenuKey))
                ReturnToPauseMenu();
        }

        [UsedImplicitly]
        public void ApplyChanges()
        {
            //SAUVEGARDER LES CHANGEMENTS
        }
        
        [UsedImplicitly]
        public void ReturnToPauseMenu()
        {
            gameUiController.ReturnFromOptionsMenu();
        }
    }
}