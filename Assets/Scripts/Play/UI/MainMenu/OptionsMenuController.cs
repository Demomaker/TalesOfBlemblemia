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