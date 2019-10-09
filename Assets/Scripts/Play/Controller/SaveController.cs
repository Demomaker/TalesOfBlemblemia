using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Game
{
    public class SaveController : MonoBehaviour
    {
        public SaveInfos saveInfos;
        public PlayerSettings playerSettings;
        private SaveGameRepo saveGameRepo;
        private CharacterStatusRepo characterStatusRepo;
        private SaveSettingsRepo saveSettingsRepo;
        private SqliteConnection connection;

        #region Test
        public void Awake()
        {
            Dictionary<string, bool> temp = new Dictionary<string, bool>{{"Franklem",false}};
            InitiateSaveController(1,"pluc12345",DifficultyLevel.Medium.ToString(),Constants.LEVEL_1_SCENE_NAME,temp);
        }
        
        #endregion
        

        #region Initiate

        public void InitiateSaveController(int id, string username, string difficultyLevel, string levelName,
            Dictionary<string, bool> characterStatus)
        {
#if UNITY_EDITOR
            var path = "URI=file:" + Path.Combine(Application.dataPath, "StreamingAssets", "SaveGame.db");
#else
            var path = "URI=file:" + Path.Combine(Application.persistentDataPath, "Database.db");
#endif
            
            connection = new SqliteConnection(path);
            connection.Open();
            InitiateSaveInfo(id, username, difficultyLevel, levelName, characterStatus);
            InitiateSettingsInfo();
            InitiateRepos();
            //Check if there are any settings in the database
            CheckForExistingSettings();
        }

        private void InitiateSettingsInfo()
        {
            playerSettings = new PlayerSettings(0, Constants.DEFAULT_TOGGLE_VALUE, Constants.DEFAULT_TOGGLE_VALUE,
                Constants.DEFAULT_SLIDER_VALUE, Constants.DEFAULT_SLIDER_VALUE, Constants.DEFAULT_SLIDER_VALUE);
        }

        private void InitiateSaveInfo(int id, string username, string difficultyLevel, string levelName,
            Dictionary<string, bool> characterStatus)
        {
            saveInfos = new SaveInfos(id,username,difficultyLevel,levelName,characterStatus);
        }

        private void InitiateRepos()
        {
            saveGameRepo = new SaveGameRepo(connection);
            characterStatusRepo = new CharacterStatusRepo(connection);
            saveSettingsRepo = new SaveSettingsRepo(connection);
        }

        #endregion

        //Author : Antoine Lessard
        /// <summary>
        /// Checks if there are any settings in the database, if there are, we load those settings and apply them, otherwise, we insert the default settings
        /// </summary>
        private void CheckForExistingSettings()
        {
            List<PlayerSettings> settings = saveSettingsRepo.FindAll();
            
            if (settings.Count == 0)
            {
                saveSettingsRepo.Insert(playerSettings);
            }
            else
            {
                //There should always be only one set of settings
                playerSettings = settings.First();
            }
        }

        public void UpdateSettings()
        {
            saveSettingsRepo.Update(playerSettings);
        }
        
        #region CreateSave

        public void CreateSave()
        {
            saveGameRepo.Insert(saveInfos);
            foreach (var character in saveInfos.characterInfos)
            {
                characterStatusRepo.Insert(character);
            }
        }

        #endregion

        #region DeleteSave

        public void DeleteSave()
        {
            foreach (var character in saveInfos.characterInfos)
            {
                characterStatusRepo.Delete(saveInfos.id);
            }
            saveGameRepo.Delete(saveInfos.id);
        }

        #endregion

        #region UpdateSave

        public void UpdateSave()
        {
            saveGameRepo.Update(saveInfos);
            foreach (var character in saveInfos.characterInfos)
            {
                characterStatusRepo.Update(character);
            }
        }

        #endregion

        #region FindAll

        public List<SaveInfos> FindAll()
        {
            List<SaveInfos> result = saveGameRepo.FindAll();
            List<CharacterInfo> characterInfos = characterStatusRepo.FindAll();

            foreach (var characterInfo in characterInfos)
            {
                switch (characterInfo.saveId)
                {
                    case 1:
                        result[0].characterInfos.Add(characterInfo);
                        break;
                    case 2:
                        result[1].characterInfos.Add(characterInfo);
                        break;
                    default:
                        result[2].characterInfos.Add(characterInfo);
                        break;
                }
            }
            return result;
        }

        #endregion

        public void OnDestroy()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}