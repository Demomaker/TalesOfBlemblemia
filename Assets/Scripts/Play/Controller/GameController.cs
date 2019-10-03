
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
 { 
     [Findable("GameController")]
     public class GameController : MonoBehaviour
     {
         public List<string> LevelsCompleted = new List<string>();
         [SerializeField] private string startingLevelName;
         private Coroutine lastLevelCoroutine;
         private string lastLoadedLevelName = null;
         private void Start()
         {
             ResetCompletedLevels();
             LoadLevel(startingLevelName);
         }

         private void ResetCompletedLevels()
         {
             LevelsCompleted.Clear();
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
