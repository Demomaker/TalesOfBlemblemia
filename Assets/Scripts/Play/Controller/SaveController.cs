using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Game
{
    public class SaveController : MonoBehaviour
    {
        public SaveInfos saveSlot1;
        public SaveInfos saveSlot2;
        public SaveInfos saveSlot3;
        public PlayerSettings playerSettings;
        private SaveGameRepo saveGameRepo;
        private CharacterStatusRepo characterStatusRepo;
        private SaveSettingsRepo saveSettingsRepo;
        private SqliteConnection connection;
        private int saveSelected;

        public int SaveSelected
        {
            get => saveSelected;
            set => saveSelected = value;
        }

        public void Awake()
        {
            var playableCharactersDictionary = CreateBaseCharacterDictionary();
            InitiateSaveController(Constants.DEFAULT_USERNAME, DifficultyLevel.Medium.ToString(),
                Constants.LEVEL_1_SCENE_NAME, playableCharactersDictionary);
        }


        #region Initiate

        private static Dictionary<string, bool> CreateBaseCharacterDictionary()
        {
            Dictionary<string, bool> playableCharactersDictionary = new Dictionary<string, bool>
            {
                {Constants.FRANKLEM_NAME, false},
                {Constants.MYRIAM_NAME, false},
                {Constants.BRAM_NAME, false},
                {Constants.RASS_NAME, false},
                {Constants.ULRIC_NAME, false},
                {Constants.JEBEDIAH_NAME, false},
                {Constants.THOMAS_NAME, false},
                {Constants.ABRAHAM_NAME, false}
            };
            return playableCharactersDictionary;
        }
        
        
        private void InitiateSaveController(string username, string difficultyLevel, string levelName,
            Dictionary<string, bool> characterStatus)
        {
#if UNITY_EDITOR
            var path = "URI=file:" + Path.Combine(Application.dataPath, "StreamingAssets", "SaveGame.db");
#else
            var path = "URI=file:" + Path.Combine(Application.persistentDataPath, "Database.db");
#endif
            
            connection = new SqliteConnection(path);
            connection.Open();
            InitiateSaveInfo(username, difficultyLevel, levelName, characterStatus);
            InitiateSettingsInfo();
            InitiateRepos();
            //Check if there are any settings in the database
            CheckForExistingSettings();
            //Check if saves were already created in the database
            CheckForExistingSaves();
        }

        private void InitiateSettingsInfo()
        {
            playerSettings = new PlayerSettings(1, Constants.DEFAULT_TOGGLE_VALUE, Constants.DEFAULT_TOGGLE_VALUE,
                Constants.DEFAULT_SLIDER_VALUE, Constants.DEFAULT_SLIDER_VALUE, Constants.DEFAULT_SLIDER_VALUE);
        }

        private void InitiateSaveInfo(string username, string difficultyLevel, string levelName,
            Dictionary<string, bool> characterStatus)
        {
            saveSlot1 = new SaveInfos(1, username, difficultyLevel, levelName, characterStatus);
            saveSlot2 = new SaveInfos(2, username, difficultyLevel, levelName, characterStatus);
            saveSlot3 = new SaveInfos(3, username, difficultyLevel, levelName, characterStatus);
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
        
        /// <summary>
        /// Check for existing saves in the database, if there are, we load those saves, otherwise, we create them as "new saves"
        /// </summary>
        private void CheckForExistingSaves()
        {
            List<SaveInfos> saves = saveGameRepo.FindAll();

            if (saves.Count == 0)
            {
                CreateSave(saveSlot1);
                CreateSave(saveSlot2);
                CreateSave(saveSlot3);
            }
            else
            {
                saveSlot1 = saves[0];
                saveSlot2 = saves[1];
                saveSlot3 = saves[2];
            }
        }

        public void UpdateSettings()
        {
            saveSettingsRepo.Update(playerSettings);
        }
        
        #region CreateSave

        private void CreateSave(SaveInfos saveSlot)
        {
            saveGameRepo.Insert(saveSlot);
            foreach (var character in saveSlot.characterInfos)
            {
                characterStatusRepo.Insert(character);
            }
        }

        #endregion

        #region DeleteSave

        public void DeleteSave(SaveInfos saveSlot)
        {
            characterStatusRepo.Delete(saveSlot.id);
            saveGameRepo.Delete(saveSlot.id);
        }

        #endregion

        #region UpdateSave

        public void UpdateSave(int saveSlotNumber)
        {
            switch (saveSlotNumber)
            {
                case 1:
                    saveGameRepo.Update(saveSlot1);
                    foreach (var character in saveSlot1.characterInfos)
                    {
                        characterStatusRepo.Update(character);
                    }

                    break;
                case 2:
                    saveGameRepo.Update(saveSlot2);
                    foreach (var character in saveSlot1.characterInfos)
                    {
                        characterStatusRepo.Update(character);
                    }

                    break;
                case 3:
                    saveGameRepo.Update(saveSlot3);
                    foreach (var character in saveSlot1.characterInfos)
                    {
                        characterStatusRepo.Update(character);
                    }

                    break;
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