
using System;
using System.Collections;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
 { 
     [Findable("GameController")]
     public class GameController : MonoBehaviour
     {
         [SerializeField] private string startingLevelName;
         private Coroutine lastLevelCoroutine;
         private string lastLoadedLevelName = null;
         private void Start()
         {
             LoadLevel(startingLevelName);
         }

         public void UnloadLevel(string levelname)
         {
             StartCoroutine(UnloadLevelCoroutine(levelname));
         }
         
         public void LoadLevel(string levelname)
         {
             if(lastLoadedLevelName != null)
                 UnloadLevel(lastLoadedLevelName);
             lastLoadedLevelName = levelname;
             if(lastLevelCoroutine != null)
                 StopCoroutine(lastLevelCoroutine);
             lastLevelCoroutine = StartCoroutine(LoadLevelCoroutine(levelname));
         }
    
         private IEnumerator LoadLevelCoroutine(string levelname)
         {
             if(!SceneManager.GetSceneByName(levelname).isLoaded)
                 yield return SceneManager.LoadSceneAsync(levelname, LoadSceneMode.Additive);
             SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelname));
         }

         private IEnumerator UnloadLevelCoroutine(string levelname)
         {
             if (SceneManager.GetSceneByName(levelname).isLoaded)
                 yield return SceneManager.UnloadSceneAsync(levelname);
         }
     }
 }
