using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Game
{
    public class SaveController : MonoBehaviour
    {
        private SaveInfos saveSlot1;
        private SaveInfos saveSlot2;
        private SaveInfos saveSlot3;
        private PlayerSettings playerSettings;
        private GameSettings gameSettings;
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
        
        public SaveInfos GetCurrentSaveSelectedInfos()
        {
            SaveInfos currentSave = new SaveInfos();
            switch (saveSelected)
            {
                case 1:
                    currentSave = saveSlot1;
                    break;
                case 2:
                    currentSave = saveSlot2;
                    break;
                case 3:
                    currentSave = saveSlot3;
                    break;
            }

            return currentSave;
        }

        public PlayerSettings PlayerSettings => playerSettings;


        public void Awake()
        {
            var playableCharactersDictionary = CreateBaseCharacterDictionary();
            gameSettings = Harmony.Finder.GameSettings;
            InitiateSaveController(gameSettings.DefaultUsername, DifficultyLevel.Medium.ToString(),
                gameSettings.TutorialSceneName, playableCharactersDictionary);
        }


        #region Initiate

        private static Dictionary<string, bool> CreateBaseCharacterDictionary()
        {
            var gameSettings = Harmony.Finder.GameSettings;
            Dictionary<string, bool> playableCharactersDictionary = new Dictionary<string, bool>
            {
                {gameSettings.FranklemName, true},
                {gameSettings.MyriamName, true},
                {gameSettings.BramName, true},
                {gameSettings.RassName, true},
                {gameSettings.UlricName, true},
                {gameSettings.JebediahName, true},
                {gameSettings.ThomasName, true},
                {gameSettings.AbrahamName, true}
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
            playerSettings = new PlayerSettings(1, gameSettings.DefaultToggleValue, gameSettings.DefaultToggleValue,
                gameSettings.DefaultSliderValue, gameSettings.DefaultSliderValue,gameSettings.DefaultSliderValue);
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
            var settings = saveSettingsRepo.FindAll();
            
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

        public void UpdateSettings(PlayerSettings playerSettings)
        {
            this.playerSettings = playerSettings;
            saveSettingsRepo.Update(this.playerSettings);
        }
        
        #region CreateSave

        private void CreateSave(SaveInfos saveSlot)
        {
            saveGameRepo.Insert(saveSlot);
            foreach (var character in saveSlot.CharacterInfos)
            {
                characterStatusRepo.Insert(character);
            }
        }

        #endregion

        #region DeleteSave

        public void DeleteSave(SaveInfos saveSlot)
        {
            characterStatusRepo.Delete(saveSlot.Id);
            saveGameRepo.Delete(saveSlot.Id);
        }

        #endregion

        #region UpdateSave

        public void UpdateSave(int saveSlotNumber)
        {
            switch (saveSlotNumber)
            {
                case 1:
                    saveGameRepo.Update(saveSlot1);
                    foreach (var character in saveSlot1.CharacterInfos)
                    {
                        characterStatusRepo.Update(character);
                    }

                    break;
                case 2:
                    saveGameRepo.Update(saveSlot2);
                    foreach (var character in saveSlot1.CharacterInfos)
                    {
                        characterStatusRepo.Update(character);
                    }

                    break;
                case 3:
                    saveGameRepo.Update(saveSlot3);
                    foreach (var character in saveSlot1.CharacterInfos)
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
                switch (characterInfo.SaveId)
                {
                    case 1:
                        result[0].CharacterInfos.Add(characterInfo);
                        break;
                    case 2:
                        result[1].CharacterInfos.Add(characterInfo);
                        break;
                    default:
                        result[2].CharacterInfos.Add(characterInfo);
                        break;
                }
            }
            return result;
        }

        public SaveInfos[] GetSaves()
        {
            return new SaveInfos[]{saveSlot1, saveSlot2, saveSlot3};
        }
        #endregion

        public void ResetSave()
        {
            var playableCharactersDictionary = CreateBaseCharacterDictionary();

            SaveInfos cleanSave = new SaveInfos(1, gameSettings.DefaultUsername, DifficultyLevel.Medium.ToString(),
                gameSettings.TutorialSceneName, playableCharactersDictionary);
            
            switch (saveSelected)
            {
                case 1:
                    saveSlot1 = cleanSave;
                    UpdateSave(saveSelected);
                    break;
                case 2:
                    cleanSave.Id = 2;
                    saveSlot2 = cleanSave;
                    UpdateSave(saveSelected);
                    break;
                case 3:
                    cleanSave.Id = 3;
                    saveSlot3 = cleanSave;
                    UpdateSave(saveSelected);
                    break;
            }
        }

        public void OnDestroy()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}