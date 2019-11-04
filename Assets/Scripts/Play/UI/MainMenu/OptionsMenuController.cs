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
        private OnSFXToggle onSfxToggle;
        private OnSFXVolumeChange onSfxVolumeChange;
        private OnMusicToggle onMusicToggle;
        private OnMusicVolumeChange onMusicVolumeChange;
        private OnMainVolumeChange onMainVolumeChange;

        private Canvas optionsScreen;

        private void Awake()
        {
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            optionsScreen = GetComponent<Canvas>();
            onSfxToggle = Harmony.Finder.OnSFXToggle;
            onMusicToggle = Harmony.Finder.OnMusicToggle;
            onMainVolumeChange = Harmony.Finder.OnMainVolumeChange;
            onMusicVolumeChange = Harmony.Finder.OnMusicVolumeChange;
            onSfxVolumeChange = Harmony.Finder.OnSFXVolumeChange;
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
            onMusicToggle.Publish(musicToggle.isOn);
            onSfxToggle.Publish(sfxToggle.isOn);
            onMainVolumeChange.Publish(mainVolumeSlider.value);
            onMusicVolumeChange.Publish(musicVolumeSlider.value);
            onSfxVolumeChange.Publish(sfxVolumeSlider.value);
            
        }
        
        private void UpdateSettings()
        {
            if(saveController.playerSettings.MusicToggle != musicToggle.isOn) onMusicToggle.Publish(musicToggle.isOn);
            if(saveController.playerSettings.SfxToggle != sfxToggle.isOn) onSfxToggle.Publish(sfxToggle.isOn);
            if(saveController.playerSettings.MainVolume != (int) mainVolumeSlider.value) onMainVolumeChange.Publish(mainVolumeSlider.value);
            if(saveController.playerSettings.MusicVolume != (int) musicVolumeSlider.value) onMusicVolumeChange.Publish(musicVolumeSlider.value);
            if(saveController.playerSettings.SfxVolume != (int) sfxVolumeSlider.value) onSfxVolumeChange.Publish(sfxVolumeSlider.value);
            
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