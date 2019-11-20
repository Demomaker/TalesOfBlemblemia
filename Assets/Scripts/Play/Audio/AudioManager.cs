using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Manages the sounds and music of the game
    /// Author : Mike Bédard
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private int numberOfSFXThatCanBePlayedAtTheSameTime = 10;
        private GameSettings gameSettings;
        private AudioClips audioClips;
        private AudioSource[] sfxSources;
        private AudioSource musicSource;
        private bool playSFX;
        private bool playMusic;
        private float mainVolume = 100;
        private float musicVolume = 100;
        private float sfxVolume = 100;
        private OnHurt onHurt;
        private OnAttack onAttack;
        private OnDodge onDodge;
        private OnUnitMove onUnitMove;
        private OnUnitDeath onUnitDeath;
        private OnPlayerUnitLoss onPlayerUnitLoss;
        private OnLevelVictory onLevelVictory;
        private OnLevelChange onLevelChange;
        private OnOverWorldEnter onOverWorldEnter;
        private OnMainMenuEnter onMainMenuEnter;
        private OnButtonClick onButtonClick;
        private OnSFXToggle onSFXToggle;
        private OnMusicToggle onMusicToggle;
        private OnMainVolumeChange onMainVolumeChange;
        private OnMusicVolumeChange onMusicVolumeChange;
        private OnSFXVolumeChange onSFXVolumeChange;

        private void Awake ()
        {
            gameSettings = Harmony.Finder.GameSettings;
            musicSource = gameObject.AddComponent<AudioSource>();
            sfxSources = new AudioSource[numberOfSFXThatCanBePlayedAtTheSameTime];
            for (int i = 0; i < numberOfSFXThatCanBePlayedAtTheSameTime; i++)
            {
                sfxSources[i] = gameObject.AddComponent<AudioSource>();
            }

            audioClips = Finder.AudioClips;
            if(audioClips == null) audioClips = new NullAudioClips();
            InitializeEventChannels();
        }

        private void OnEnable()
        {
            EnableEventChannels();
        }

        private void OnDisable()
        {
            DisableEventChannels();
        }

        private void InitializeEventChannels()
        {
            onHurt = Harmony.Finder.OnHurt;
            onAttack = Harmony.Finder.OnAttack;
            onDodge = Harmony.Finder.OnDodge;
            onUnitMove = Harmony.Finder.OnUnitMove;
            onUnitDeath = Harmony.Finder.OnUnitDeath;
            onPlayerUnitLoss = Harmony.Finder.OnPlayerUnitLoss;
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelChange = Harmony.Finder.OnLevelChange;
            onOverWorldEnter = Harmony.Finder.OnOverWorldEnter;
            onMainMenuEnter = Harmony.Finder.OnMainMenuEnter;
            onButtonClick = Harmony.Finder.OnButtonClick;
            onSFXToggle = Harmony.Finder.OnSFXToggle;
            onMusicToggle = Harmony.Finder.OnMusicToggle;
            onMainVolumeChange = Harmony.Finder.OnMainVolumeChange;
            onMusicVolumeChange = Harmony.Finder.OnMusicVolumeChange;
            onSFXVolumeChange = Harmony.Finder.OnSFXVolumeChange;
        }

        private void EnableEventChannels()
        {
            onHurt.Notify += PlayHurtSound;
            onAttack.Notify += PlayAttackSound;
            onDodge.Notify += PlayDodgeSound;
            onUnitMove.Notify += PlayUnitMovementSound;
            onUnitDeath.Notify += PlayUnitDeathSound;
            onPlayerUnitLoss.Notify += PlayUnitLossMusic;
            onLevelVictory.Notify += PlayLevelVictoryMusic;
            onLevelChange.Notify += PlayBackgroundMusicOfLevel;
            onOverWorldEnter.Notify += PlayOverWorldBackgroundMusic;
            onMainMenuEnter.Notify += PlayMainMenuBackgroundMusic;
            onButtonClick.Notify += PlayButtonClickSound;
            onSFXToggle.Notify += ToggleSFX;
            onMusicToggle.Notify += ToggleMusic;
            onMainVolumeChange.Notify += ChangeMainVolume;
            onMusicVolumeChange.Notify += ChangeMusicVolume;
            onSFXVolumeChange.Notify += ChangeSFXVolume;
        }

        private void DisableEventChannels()
        {
            onHurt.Notify -= PlayHurtSound;
            onAttack.Notify -= PlayAttackSound;
            onDodge.Notify -= PlayDodgeSound;
            onUnitMove.Notify -= PlayUnitMovementSound;
            onUnitDeath.Notify -= PlayUnitDeathSound;
            onPlayerUnitLoss.Notify -= PlayUnitLossMusic;
            onLevelVictory.Notify -= PlayLevelVictoryMusic;
            onLevelChange.Notify -= PlayBackgroundMusicOfLevel;
            onOverWorldEnter.Notify -= PlayOverWorldBackgroundMusic;
            onMainMenuEnter.Notify -= PlayMainMenuBackgroundMusic;
            onButtonClick.Notify -= PlayButtonClickSound;
            onSFXToggle.Notify -= ToggleSFX;
            onMusicToggle.Notify -= ToggleMusic;
            onMainVolumeChange.Notify -= ChangeMainVolume;
            onMusicVolumeChange.Notify -= ChangeMusicVolume;
            onSFXVolumeChange.Notify -= ChangeSFXVolume;
        }

        private void UpdateMusicVolume()
        {
            musicSource.volume = mainVolume * musicVolume / (gameSettings.Percent * gameSettings.Percent);
        }

        private void UpdateSoundVolume()
        {
            float totalSFXVolume = mainVolume * sfxVolume / (gameSettings.Percent * gameSettings.Percent);
            foreach (var sfxSource in sfxSources)
            {
                sfxSource.volume = totalSFXVolume;
            }
        }

        private void ToggleSFX(bool toggle)
        {
            playSFX = toggle;
            if (!playSFX)
            {
                foreach (var efxSource in sfxSources)
                {
                    efxSource.Stop();
                }
            }
        }

        private void ToggleMusic(bool toggle)
        {

            playMusic = toggle;
            if (!playMusic)
            {
                musicSource.Pause();
            }
            else
            {
                musicSource.Play();
            }
        }

        private void ChangeMainVolume(float volume)
        {
            mainVolume = volume;
            UpdateMusicVolume();
            UpdateSoundVolume();
        }

        private void ChangeMusicVolume(float volume)
        {
            musicVolume = volume;
            UpdateMusicVolume();
        }

        private void ChangeSFXVolume(float volume)
        {
            sfxVolume = volume;
            UpdateSoundVolume();
        }
        
        //Used to play single sound clips.
        private void PlaySFX(AudioClip clip, Vector2 position = new Vector2())
        {
            if (audioClips is NullAudioClips) return;
            for (int i = 0; i < numberOfSFXThatCanBePlayedAtTheSameTime; i++)
            {
                if (sfxSources[i].isPlaying == false)
                {
                    sfxSources[i].transform.position = position;
                    //Set the clip of our efxSource audio source to the clip passed in as a parameter.
                    sfxSources[i].clip = clip;
                    //Set the efxSource to no loop, so that the sound doesn't repeat.
                    sfxSources[i].loop = false;
                    //Play the clip.
                    if (playSFX)
                        sfxSources[i].Play();
                    break;
                }
            }
        }
        
        //Used to play music clips
        private void PlayMusic(AudioClip clip)
        {
            if (audioClips is NullAudioClips) return;
            StopCurrentMusic();
            
            //Set the clip of our musicSource audio source to the clip passed in as a parameter.
            musicSource.clip = clip;
            
            //Set the musicSource as a loop, so that the music restarts on end.
            musicSource.loop = true;
            
            //Play the clip.
            if (playMusic)
                musicSource.Play();
        }

        private void StopCurrentMusic()
        {
            if(musicSource.clip != null && musicSource.isPlaying)
                musicSource.Stop();
            
            musicSource.loop = false;
        }

        private void PlayHurtSound(Unit unit)
        {
            PlaySFX(audioClips.HurtSound, unit.transform.position);
        }

        private void PlayAttackSound(Unit unit)
        {
            switch (unit.Gender)
            {
                case UnitGender.Male :
                    PlaySFX(audioClips.MaleAttackSound, unit.transform.position);
                    break;
                case UnitGender.Female :
                    PlaySFX(audioClips.FemaleAttackSound, unit.transform.position);
                    break;
                case UnitGender.Mork :
                    PlaySFX(audioClips.MorkAttackSound, unit.transform.position);
                    break;
            }
        }

        private void PlayDodgeSound(Unit unit)
        {
            PlaySFX(audioClips.DodgeSound, unit.transform.position);
        }

        private void PlayUnitMovementSound(Unit unit)
        {
            PlaySFX(audioClips.UnitMoveSound, unit.transform.position);
        }

        private void PlayUnitDeathSound(Unit unit)
        {
            PlaySFX(audioClips.UnitDeathSound, unit.transform.position);
        }

        private void PlayButtonClickSound(Button button)
        {
            PlaySFX(audioClips.ButtonClickSound, button.transform.position);
        }

        private void PlayUnitLossMusic(Unit unit)
        {
            if(musicSource.clip != audioClips.SadMusic)
                PlayMusic(audioClips.SadMusic);
        }

        private void PlayLevelVictoryMusic(LevelController levelController)
        {
            if(musicSource.clip != audioClips.LevelVictoryMusic)
                PlayMusic(audioClips.LevelVictoryMusic);
        }

        private void PlayBackgroundMusicOfLevel(LevelController level)
        {
            StopCurrentMusic();
            PlayMusic(level.BackgroundMusic);
        }

        private void PlayOverWorldBackgroundMusic(OverWorldController overWorld)
        {
            StopCurrentMusic();
            PlayMusic(audioClips.OverWorldMusic);
        }

        private void PlayMainMenuBackgroundMusic(MainMenuController mainMenu)
        {
            StopCurrentMusic();
            PlayMusic(audioClips.MainMenuMusic);
        }
    }
}