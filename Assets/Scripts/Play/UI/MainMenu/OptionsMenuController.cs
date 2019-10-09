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
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Button returnToMainMenuButton;
        [SerializeField] private Button applyChangesButton;

        [Header("Controls")] 
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0;

        private MenusController menusController;
        private SaveController saveController;

        private void Awake()
        {
            menusController = Finder.MenusController;
            saveController = Finder.SaveController;
        }

        private void Start()
        {
            InitializeSettingsValues();
        }

        public void Update()
        {
            if (Input.GetKeyDown(confirmKey))
                UIExtensions.SelectedButton?.Click();
        }

        [UsedImplicitly]
        public void ApplyChanges()
        {
            UpdateSettings();
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            menusController.ReturnFromOptionsMenu();
        }

        #region ValuesSetup
        private void InitializeSettingsValues()
        {
            musicToggle.isOn = saveController.playerSettings.MusicToggle;
            sfxToggle.isOn = saveController.playerSettings.SfxToggle;
            mainVolumeSlider.value = saveController.playerSettings.MainVolume;
            musicVolumeSlider.value = saveController.playerSettings.MusicVolume;
            sfxVolumeSlider.value = saveController.playerSettings.SfxVolume;
        }
        
        private void UpdateSettings()
        {
            saveController.playerSettings.MusicToggle = musicToggle.isOn;
            saveController.playerSettings.SfxToggle = sfxToggle.isOn;
            saveController.playerSettings.MainVolume = (int) mainVolumeSlider.value;
            saveController.playerSettings.MusicVolume = (int) musicVolumeSlider.value;
            saveController.playerSettings.SfxVolume = (int) sfxVolumeSlider.value;
            
            saveController.UpdateSettings();
        }
        #endregion
        
    }
}