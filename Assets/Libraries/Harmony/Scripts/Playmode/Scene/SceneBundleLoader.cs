using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Harmony
{
    /// <summary>
    /// Scene Bundle loader. Used to load scene bundles.
    /// </summary>
    public class SceneBundleLoader : MonoBehaviour
    {
        private bool isLoadingWorld;
        private bool isUnloadingWorld;

        /// <summary>
        /// Triggers when a scene bundle loading starts.
        /// </summary>
        public event EventHandler<SceneBundle> OnLoadingStarted;

        /// <summary>
        /// Triggers when a scene bundle loading ends.
        /// </summary>
        public event EventHandler<SceneBundle> OnLoadingEnded;

        /// <summary>
        /// Triggers when a scene bundle unloading starts.
        /// </summary>
        public event EventHandler<SceneBundle> OnUnloadingStarted;

        /// <summary>
        /// Triggers when a scene bundle unloading ends.
        /// </summary>
        public event EventHandler<SceneBundle> OnUnloadingEnded;

        /// <summary>
        /// Is a Scene Bundle currently loading ?
        /// </summary>
        public bool IsLoadingWorld => isLoadingWorld;

        /// <summary>
        /// Is a Scene Bundle currently unloading ?
        /// </summary>
        public bool IsUnloadingWorld1 => isUnloadingWorld;

        /// <summary>
        /// Tells if a Scene Bundle is loading or unloading.
        /// </summary>
        public bool IsLoadingOrUnloadingWorld => isLoadingWorld || isUnloadingWorld;

        /// <summary>
        /// Load a Scene Bundle.
        ///
        /// This is additive, meaning it loads theses scenes and keeps the ones loaded.
        /// If one of the scenes in this bundle are already loaded, they are ignored.
        /// 
        /// This is also asynchronous. Calling this starts a new thread.
        /// </summary>
        /// <param name="sceneBundle">Scene Bundle</param>
        public void Load(SceneBundle sceneBundle)
        {
            StartCoroutine(LoadRoutine(sceneBundle));
        }

        /// <summary>
        /// Unload a Scene Bundle.
        ///
        /// If one of the scenes in this bundle is not loaded, it is ignored.
        /// 
        /// This is also asynchronous. Calling this starts a new thread.
        /// </summary>
        /// <param name="sceneBundle">Scene Bundle</param>
        public void Unload(SceneBundle sceneBundle)
        {
            StartCoroutine(UnloadRoutine(sceneBundle));
        }

        /// <summary>
        /// Unload a Scene Bundle, and immediatly after loads an other Scene Bundle. See <see cref="Load"/> and
        /// <see cref="Unload"/> for details about how this is done.
        /// </summary>
        /// <param name="oldSceneBundle">Scene Bundle to unload.</param>
        /// <param name="newSceneBundle">Scene Bundle to load.</param>
        public void Switch(SceneBundle oldSceneBundle, SceneBundle newSceneBundle)
        {
            StartCoroutine(SwitchRoutine(oldSceneBundle, oldSceneBundle));
        }

        /// <summary>
        /// Reload a Scene Bundle.
        ///
        /// If one of the scenes in this bundle is not loaded, it is loaded. This keeps every scene
        /// currently loaded that is not in this bundle untouched.
        /// 
        /// This is asynchronous. Calling this starts a new thread.
        /// </summary>
        /// <param name="sceneBundle">Scene Bundle</param>
        public void Reload(SceneBundle sceneBundle)
        {
            Switch(sceneBundle, sceneBundle);
        }

        private IEnumerator LoadRoutine(SceneBundle sceneBundle)
        {
            NotifyLoadStart(sceneBundle);

            foreach (var scene in sceneBundle.Scenes)
            {
                var sceneToLoad = SceneManager.GetSceneByName(scene.Name);
                if (!sceneToLoad.isLoaded)
                    yield return SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
            }

            if (sceneBundle.SetFirstAsActive && sceneBundle.Scenes.Any())
            {
                var sceneToMakeActive = SceneManager.GetSceneByName(sceneBundle.Scenes[0].Name);
                if (sceneToMakeActive.isLoaded)
                    SceneManager.SetActiveScene(sceneToMakeActive);
            }

            NotifyLoadEnd(sceneBundle);
        }

        private IEnumerator UnloadRoutine(SceneBundle sceneBundle)
        {
            NotifyUnloadStart(sceneBundle);

            foreach (var scene in sceneBundle.Scenes)
            {
                var sceneToUnload = SceneManager.GetSceneByName(scene.Name);
                if (sceneToUnload.isLoaded)
                    yield return SceneManager.UnloadSceneAsync(scene.Name);
            }

            NotifyUnloadEnd(sceneBundle);
        }

        private IEnumerator SwitchRoutine(SceneBundle oldSceneBundle, SceneBundle newSceneBundle)
        {
            yield return UnloadRoutine(oldSceneBundle);

            yield return LoadRoutine(newSceneBundle);
        }

        private void NotifyLoadStart(SceneBundle sceneBundle)
        {
            isLoadingWorld = true;

            if (OnLoadingStarted != null) OnLoadingStarted(sceneBundle);
        }

        private void NotifyLoadEnd(SceneBundle sceneBundle)
        {
            isLoadingWorld = false;

            if (OnLoadingEnded != null) OnLoadingEnded(sceneBundle);
        }

        private void NotifyUnloadStart(SceneBundle sceneBundle)
        {
            isUnloadingWorld = true;

            if (OnUnloadingStarted != null) OnUnloadingStarted(sceneBundle);
        }

        private void NotifyUnloadEnd(SceneBundle sceneBundle)
        {
            isUnloadingWorld = false;

            if (OnUnloadingEnded != null) OnUnloadingEnded(sceneBundle);
        }
    }
}