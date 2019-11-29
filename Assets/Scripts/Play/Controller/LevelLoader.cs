using System.Collections;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    // Author: Jérémie Bertrand
    [Findable("LevelLoader")]
    [RequireComponent(typeof(Animator))]
    public class LevelLoader : MonoBehaviour
    {
        private const string FADE_IN_TRIGGER = "FadeIn";
        private const string FADE_OUT_TRIGGER = "FadeOut";
        
        private Animator animator;
        private string loadedLevel;
        private LoadSceneMode sceneMode = LoadSceneMode.Single;
        private GameSettings gameSettings;
        private bool fadeOutCompleted;
        private bool canLoadNewLevel = true;
        private static readonly int FadeOut = Animator.StringToHash(FADE_OUT_TRIGGER);
        private static readonly int FadeIn = Animator.StringToHash(FADE_IN_TRIGGER);

        public bool CanLoadNewLevel => canLoadNewLevel;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            gameSettings = Harmony.Finder.GameSettings;
        }

        public void FadeToLevel(string levelName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (SceneManager.GetSceneByName(levelName).isLoaded || !canLoadNewLevel) return;
            
            Harmony.Finder.CoroutineStarter.StartCoroutine(FadeToLevelAsync(levelName, loadSceneMode));
            
            if (SceneManager.GetSceneByName(loadedLevel).name != gameSettings.OverworldSceneName)
            {
                animator.SetTrigger(FadeOut);
            }
        }

        public void LoadLevel(string levelName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (SceneManager.GetSceneByName(levelName).isLoaded || !canLoadNewLevel) return;
            SceneManager.LoadScene(levelName, loadSceneMode);
            loadedLevel = levelName;
            sceneMode = loadSceneMode;
        }

        [UsedImplicitly]
        public void OnFadeComplete ()
        {
            fadeOutCompleted = true;
        }

        private IEnumerator FadeToLevelAsync(string levelName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            canLoadNewLevel = false;
            var scene = SceneManager.LoadSceneAsync(levelName, loadSceneMode);
            scene.allowSceneActivation = false;
            while (scene.progress < 0.9f)
            {
                yield return null;
            }

            if (sceneMode == LoadSceneMode.Additive)
            {
                if (SceneManager.GetSceneByName(loadedLevel).name == gameSettings.OverworldSceneName)
                {
                    var overWorldController = Harmony.Finder.OverWorldController;
                    while (overWorldController.CharacterIsMoving)
                    {
                        yield return null;
                    }
                    animator.SetTrigger(FadeOut);
                }
                SceneManager.UnloadSceneAsync(loadedLevel);
                while (SceneManager.GetSceneByName(loadedLevel).isLoaded)
                {
                    yield return null;
                }
            }

            loadedLevel = levelName;
            sceneMode = loadSceneMode;
            
            while (!fadeOutCompleted)
            {
                yield return null;
            }
            
            scene.allowSceneActivation = true;

            animator.SetTrigger(FadeIn);
            
            fadeOutCompleted = false;
            canLoadNewLevel = true;
        }
    }
}