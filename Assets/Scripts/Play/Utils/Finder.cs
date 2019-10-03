using Harmony;
using UnityEngine;

namespace Game
{
    public static class Finder
    {
        public static MenusController MenusController => FindWithTag<MenusController>(Tags.MAIN_CONTROLLER);

        private static T FindWithTag<T>(string tag)
        {
            return GameObject.FindWithTag(tag).GetComponent<T>();
        }
    }
}