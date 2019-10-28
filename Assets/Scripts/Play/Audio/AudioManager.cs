using System;
using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private int numberOfSFXThatCanBePlayedAtTheSameTime = 10;
        [SerializeField] private float lowPitchRange = .95f;               
        [SerializeField] private float highPitchRange = 1.05f;
        private AudioClips audioClips;
        private AudioSource[] sfxSources;
        private AudioSource musicSource;
        private bool playSFX;
        private bool playMusic;
        private float mainVolume = 100;
        private float musicVolume = 100;
        private float sfxVolume = 100;

        void Awake ()
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            sfxSources = new AudioSource[numberOfSFXThatCanBePlayedAtTheSameTime];
            for (int i = 0; i < numberOfSFXThatCanBePlayedAtTheSameTime; i++)
            {
                sfxSources[i] = gameObject.AddComponent<AudioSource>();
            }

            audioClips = Finder.AudioClips;
            if(audioClips == null) audioClips = new NullAudioClips();
        }

        private void OnEnable()
        {
            OnHurt.Notify += PlayHurtSound;
            OnAttack.Notify += PlayAttackSound;
            OnDodge.Notify += PlayDodgeSound;
            OnUnitMove.Notify += PlayUnitMovementSound;
            OnUnitDeath.Notify += PlayUnitDeathSound;
            OnPlayerUnitLoss.Notify += PlayUnitLossMusic;
            OnLevelVictory.Notify += PlayLevelVictoryMusic;
            OnLevelChange.Notify += PlayBackgroundMusicOfLevel;
            OnOverworldEnter.Notify += PlayOverworldBackgroundMusic;
            OnMainMenuEnter.Notify += PlayMainMenuBackgroundMusic;
            OnButtonClick.Notify += PlayButtonClickSound;
            OnSFXToggle.Notify += ToggleSFX;
            OnMusicToggle.Notify += ToggleMusic;
            OnMainVolumeChange.Notify += ChangeMainVolume;
            OnMusicVolumeChange.Notify += ChangeMusicVolume;
            OnSFXVolumeChange.Notify += ChangeSFXVolume;
        }

        private void OnDisable()
        {
            OnHurt.Notify -= PlayHurtSound;
            OnAttack.Notify -= PlayAttackSound;
            OnDodge.Notify -= PlayDodgeSound;
            OnUnitMove.Notify -= PlayUnitMovementSound;
            OnUnitDeath.Notify -= PlayUnitDeathSound;
            OnPlayerUnitLoss.Notify -= PlayUnitLossMusic;
            OnLevelVictory.Notify -= PlayLevelVictoryMusic;
            OnLevelChange.Notify -= PlayBackgroundMusicOfLevel;
            OnOverworldEnter.Notify -= PlayOverworldBackgroundMusic;
            OnMainMenuEnter.Notify -= PlayMainMenuBackgroundMusic;
            OnButtonClick.Notify -= PlayButtonClickSound;
            OnSFXToggle.Notify -= ToggleSFX;
            OnMusicToggle.Notify -= ToggleMusic;
            OnMainVolumeChange.Notify -= ChangeMainVolume;
            OnMusicVolumeChange.Notify -= ChangeMusicVolume;
            OnSFXVolumeChange.Notify -= ChangeSFXVolume;
        }

        private void UpdateMusicVolume()
        {
            musicSource.volume = mainVolume * musicVolume / (Constants.PERCENT * Constants.PERCENT);
        }

        private void UpdateSoundVolume()
        {
            float totalSFXVolume = mainVolume * sfxVolume / (Constants.PERCENT * Constants.PERCENT);
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

        private void PlayOverworldBackgroundMusic(OverworldController overworld)
        {
            StopCurrentMusic();
            PlayMusic(audioClips.OverworldMusic);
        }

        private void PlayMainMenuBackgroundMusic(MainMenuController mainMenu)
        {
            StopCurrentMusic();
            PlayMusic(audioClips.MainMenuMusic);
        }
    }
}