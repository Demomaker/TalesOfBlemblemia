using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class OptionsMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider mainVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Controls")] 
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode returnToMainMenuKey = KeyCode.Escape;

        private Canvas optionsCanvas;

        private void Awake()
        {
            optionsCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            optionsCanvas.enabled = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(confirmKey))
                UIExtensions.SelectedButton?.Click();
            else if (Input.GetKeyDown(returnToMainMenuKey))
                ReturnToMainMenu();
        }

        [UsedImplicitly]
        public void ApplyChanges()
        {
            //SAUVEGARDER LES CHANGEMENTS
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            //RETOUR AU MAIN MENU
        }
    }
}