using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

//Author: Antoine Lessard
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
            onMusicToggle.Publish(musicToggle.isOn);
            onSfxToggle.Publish(sfxToggle.isOn);
            onMainVolumeChange.Publish(mainVolumeSlider.value);
            onMusicVolumeChange.Publish(musicVolumeSlider.value);
            onSfxVolumeChange.Publish(sfxVolumeSlider.value);
        }
        
        private void UpdateSettings()
        {
            PlayerSettings playerSettings = saveController.PlayerSettings;
            
            if(playerSettings.MusicToggle != musicToggle.isOn) onMusicToggle.Publish(musicToggle.isOn);
            if(playerSettings.SfxToggle != sfxToggle.isOn) onSfxToggle.Publish(sfxToggle.isOn);
            if(playerSettings.MainVolume != (int) mainVolumeSlider.value) onMainVolumeChange.Publish(mainVolumeSlider.value);
            if(playerSettings.MusicVolume != (int) musicVolumeSlider.value) onMusicVolumeChange.Publish(musicVolumeSlider.value);
            if(playerSettings.SfxVolume != (int) sfxVolumeSlider.value) onSfxVolumeChange.Publish(sfxVolumeSlider.value);
            
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