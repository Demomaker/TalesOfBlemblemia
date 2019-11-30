using UnityEngine;
using Random = System.Random;

namespace Game
{
    //Authors: Zacharie Lavigne
    public static class Finder
    {
        private static Random random;
        
        public static Random Random
        {
            get
            {
                if (random == null)
                    random = new Random();
                return random;
            }
        }
        public static AudioClips AudioClips => FindWithTag<AudioClips>(Tags.SOUND_MANAGER);
        
        //Author : Benjamin Lemelin
        private static T FindWithTag<T>(string tag)
        {
            return GameObject.FindWithTag(tag).GetComponent<T>();
        }
    }
}