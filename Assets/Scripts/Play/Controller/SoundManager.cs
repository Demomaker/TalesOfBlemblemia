using UnityEngine;

namespace Game
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource efxSource;                    //Drag a reference to the audio source which will play the sound effects.
        public AudioSource musicSource;                    //Drag a reference to the audio source which will play the music.
        public static SoundManager instance = null;        //Allows other scripts to call functions from SoundManager.                
        public float lowPitchRange = .95f;                //The lowest a sound effect will be randomly pitched.
        public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.


        void Awake ()
        {
            //Check if there is already an instance of SoundManager
            if (instance == null)
                //if not, set it to this.
                instance = this;
            //If instance already exists:
            else if (instance != this)
                //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
                Destroy (gameObject);

            //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad (gameObject);
        }


        //Used to play single sound clips.
        public void PlaySingle(AudioClip clip)
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            efxSource.clip = clip;
            //Play the clip.
            efxSource.Play();
        }
        
        //Used to play music clips
        public void PlayMusic(AudioClip clip)
        {
            //Set the clip of our musicSource audio source to the clip passed in as a parameter.
            musicSource.clip = clip;
            
            //Set the musicSource has a loop, so that the music restarts on end.
            musicSource.loop = true;
            
            //Play the clip.
            musicSource.Play();

            Debug.Log("Currently Playing : " + clip.name);
        }

        public void StopCurrentSingle()
        {
            if(efxSource.clip != null && efxSource.isPlaying)
                efxSource.Stop();
        }

        public void StopCurrentMusic()
        {
            if(musicSource.clip != null && musicSource.isPlaying)
                musicSource.Stop();
            
            musicSource.loop = false;
        }
    }
}