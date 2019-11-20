using UnityEngine;

namespace Game
{
    /// <summary>
    /// Audio clips that represent the sounds and music of the game
    /// Author : Mike Bédard
    /// </summary>
    public class AudioClips : MonoBehaviour
    {
        [SerializeField] private AudioClip maleAttackSound;
        [SerializeField] private AudioClip femaleAttackSound;
        [SerializeField] private AudioClip morkAttackSound;
        [SerializeField] private AudioClip hurtSound;
        [SerializeField] private AudioClip dodgeSound;
        [SerializeField] private AudioClip unitMoveSound;
        [SerializeField] private AudioClip unitDeathSound;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip overWorldMusic;
        [SerializeField] private AudioClip sadMusic;
        [SerializeField] private AudioClip levelVictoryMusic;
        [SerializeField] private AudioClip mainMenuMusic;

        private GameSettings gameSettings = null;
        public AudioClip MaleAttackSound => maleAttackSound;
        public AudioClip FemaleAttackSound => femaleAttackSound;
        public AudioClip MorkAttackSound => morkAttackSound;
        public AudioClip HurtSound => hurtSound;
        public AudioClip DodgeSound => dodgeSound;
        public AudioClip UnitMoveSound => unitMoveSound;
        public AudioClip UnitDeathSound => unitDeathSound;
        public AudioClip ButtonClickSound => buttonClickSound;
        public AudioClip OverWorldMusic => overWorldMusic;
        public AudioClip SadMusic => sadMusic;
        public AudioClip LevelVictoryMusic => levelVictoryMusic;
        public AudioClip MainMenuMusic => mainMenuMusic;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            Resources.LoadAll(gameSettings.AudioPath);
        }
    }
}
