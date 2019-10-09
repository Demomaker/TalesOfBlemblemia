using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Game
{
    public class SaveController : MonoBehaviour
    {
        private SaveInfos saveInfos;
        private SaveGameRepo saveGameRepo;
        private CharacterStatusRepo characterStatusRepo;
        private SqliteConnection connection;

        #region Test
        public void Awake()
        {
            Dictionary<string, bool> temp = new Dictionary<string, bool>{{"Franklem",false}};
            InitiateSaveController(1,"pluc12345",DifficultyLevel.Medium.ToString(),"village 1",temp);
            FindAll();
        }
        
        #endregion
        

        #region Initiate

        public void InitiateSaveController(int id, string username, string difficultyLevel, string levelName,
            Dictionary<string, bool> characterStatus)
        {
            string connectionString = "Data Source=C:\\Users\\pierr\\Desktop\\session5\\Synthese\\starter-game\\Assets\\Database\\SaveGame.db; Version=3";
            connection = new SqliteConnection(connectionString);
            connection.Open();
            InitiateSaveInfos(id, username, difficultyLevel, levelName, characterStatus);
            InitiateRepos();
        }

        private void InitiateSaveInfos(int id, string username, string difficultyLevel, string levelName,
            Dictionary<string, bool> characterStatus)
        {
            saveInfos = new SaveInfos(id,username,difficultyLevel,levelName,characterStatus);
        }

        private void InitiateRepos()
        {
            saveGameRepo = new SaveGameRepo(connection);
            characterStatusRepo = new CharacterStatusRepo(connection);
        }

        #endregion

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