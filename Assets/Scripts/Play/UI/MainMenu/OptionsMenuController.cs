using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class OptionsMenuController : MonoBehaviour, IMenuController
    {
        [Header("Buttons")] 
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider mainVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button returnToMainMenuButton;
        [SerializeField] private Button applyChangesButton;

        [Header("Controls")] 
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode returnToMainMenuKey = KeyCode.Escape;

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
        public void ApplyChanges()
        {
            //SAUVEGARDER LES CHANGEMENTS
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            menusController.ReturnFromOptionsMenu();
        }
    }
}