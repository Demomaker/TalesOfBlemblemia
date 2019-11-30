using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Game
{
    //Authors : Pierre-Luc Maltais & Antoine Lessard
    public class SaveGameRepo : IRepository<SaveInfos>
    {
        private readonly DbConnection connection;

        public SaveGameRepo(DbConnection connection)
        {
            this.connection = connection;
        }
        
        public void Insert(SaveInfos myObject)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO SaveGame(savegame_id,player_name,difficulty_level,level_name) VALUES (?,?,?,?)";

                var id = command.CreateParameter();
                id.Value = myObject.Id;
                command.Parameters.Add(id);

                var playerName = command.CreateParameter();
                playerName.Value = myObject.Username;
                command.Parameters.Add(playerName);

                var difficultyLevel = command.CreateParameter();
                difficultyLevel.Value = myObject.DifficultyLevel;
                command.Parameters.Add(difficultyLevel);

                var levelName = command.CreateParameter();
                levelName.Value = myObject.LevelName;
                command.Parameters.Add(levelName);

                command.ExecuteNonQuery();
            }
        }

        public List<SaveInfos> FindAll()
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM SaveGame";

                var result = new List<SaveInfos>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["savegame_id"]);
                        string playerName = reader["player_name"].ToString();
                        string difficultyLevel = reader["difficulty_level"].ToString();
                        string levelName = reader["level_name"].ToString();
                        SaveInfos saveInfos = new SaveInfos(id, playerName, difficultyLevel, levelName, null);
                        result.Add(saveInfos);
                    }
                }

                return result;
            }
        }

        public void Update(SaveInfos myObject)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE SaveGame SET player_name = ?, difficulty_level = ?, level_name = ? WHERE savegame_id = ?";

                var playerName = command.CreateParameter();
                playerName.Value = myObject.Username;
                command.Parameters.Add(playerName);

                var difficulty = command.CreateParameter();
                difficulty.Value = myObject.DifficultyLevel;
                command.Parameters.Add(difficulty);
            
                var levelName = command.CreateParameter();
                levelName.Value = myObject.LevelName;
                command.Parameters.Add(levelName);

                var id = command.CreateParameter();
                id.Value = myObject.Id;
                command.Parameters.Add(id);
            
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "DELETE FROM SaveGame WHERE savegame_id = ?";

                var saveGameId = command.CreateParameter();
                saveGameId.Value = id;
                command.Parameters.Add(saveGameId);

                command.ExecuteNonQuery();
            }
        }
    }
}