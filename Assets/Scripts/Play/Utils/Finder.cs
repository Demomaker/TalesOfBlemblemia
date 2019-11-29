using UnityEngine;
using Random = System.Random;

namespace Game
{
    //Authors: Jérémie Bertrand & Mike Bédard
    public static class Finder
    {
        private static Random random;

        //Author: Zacharie Lavigne
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
        
        //Author : Antoine Lessard
        public static MenusController MenusController => FindWithTag<MenusController>(Tags.MAIN_CONTROLLER);
        public static SaveController SaveController => FindWithTag<SaveController>(Tags.SAVE_CONTROLLER);
        public static PauseController PauseController => FindWithTag<PauseController>(Tags.PAUSE_CONTROLLER);
        public static Navigator Navigator => FindWithTag<Navigator>(Tags.NAVIGATOR);
        
        private static T FindWithTag<T>(string tag)
        {
            return GameObject.FindWithTag(tag).GetComponent<T>();
        }
    }
}