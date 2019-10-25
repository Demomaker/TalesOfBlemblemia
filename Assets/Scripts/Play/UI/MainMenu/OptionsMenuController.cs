using System;
using Harmony;
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
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Button returnButton;
        [SerializeField] private Button applyChangesButton;
        
        private SaveController saveController;
        private Navigator navigator;

        private Canvas optionsScreen;

        private void Awake()
        {
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            optionsScreen = GetComponent<Canvas>();
        }

        private void Start()
        {
            InitializeSettingsValues();
        }

        public void Enter()
        {
            navigator.Enter(optionsScreen);
            InitializeSettingsValues();
        }

        [UsedImplicitly]
        public void ApplyChanges()
        {
            UpdateSettings();
        }
        
        [UsedImplicitly]
        public void Leave()
        {
            navigator.Leave();
        }

        #region ValuesSetup
        private void InitializeSettingsValues()
        {
            musicToggle.isOn = saveController.PlayerSettings.MusicToggle;
            sfxToggle.isOn = saveController.PlayerSettings.SfxToggle;
            mainVolumeSlider.value = saveController.PlayerSettings.MainVolume;
            musicVolumeSlider.value = saveController.PlayerSettings.MusicVolume;
            sfxVolumeSlider.value = saveController.PlayerSettings.SfxVolume;
        }
        
        private void UpdateSettings()
        {
            PlayerSettings playerSettings = saveController.PlayerSettings;
            playerSettings.MusicToggle = musicToggle.isOn;
            playerSettings.SfxToggle = sfxToggle.isOn;
            playerSettings.MainVolume = (int) mainVolumeSlider.value;
            playerSettings.MusicVolume = (int) musicVolumeSlider.value;
            playerSettings.SfxVolume = (int) sfxVolumeSlider.value;
            
            saveController.UpdateSettings(playerSettings);
        }
        #endregion
        
    }
}