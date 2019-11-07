using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Game
{
    public class CharacterStatusRepo : Repository<CharacterInfo>
    {
        private DbConnection connection;

        public CharacterStatusRepo(DbConnection connection)
        {
            this.connection = connection;
        }
    
        public void Insert(CharacterInfo myObject)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;

            command.CommandText =
                "INSERT INTO CharacterStatus(character_id,savegame_id,character_name, character_status) VALUES (Null,?,?,?)";

            var id = command.CreateParameter();
            id.Value = myObject.SaveId;
            command.Parameters.Add(id);

            var characterName = command.CreateParameter();
            characterName.Value = myObject.CharacterName;
            command.Parameters.Add(characterName);

            var characterStatus = command.CreateParameter();
            characterStatus.Value = myObject.CharacterStatus;
            command.Parameters.Add(characterStatus);

            command.ExecuteNonQuery();
        }

        public List<CharacterInfo> FindAll()
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            
            command.CommandText = "SELECT * FROM CharacterStatus";

            List<CharacterInfo> result = new List<CharacterInfo>();
            DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                bool status = Convert.ToBoolean(reader["character_status"]);
                result.Add(new CharacterInfo(
                reader["character_name"].ToString(),
                status,
                Convert.ToInt32(reader["savegame_id"])));
            }
            return result;
        }

        public void Update(CharacterInfo myObject)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText =
                "UPDATE CharacterStatus SET character_status = ? WHERE savegame_id = ? AND character_name = ?";

            var characterStatus = command.CreateParameter();
            characterStatus.Value = myObject.CharacterStatus;
            command.Parameters.Add(characterStatus);

            var id = command.CreateParameter();
            id.Value = myObject.SaveId;
            command.Parameters.Add(id);

            var characterName = command.CreateParameter();
            characterName.Value = myObject.CharacterName;
            command.Parameters.Add(characterName);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM CharacterStatus WHERE savegame_id = ?";

            var saveGameId = command.CreateParameter();
            saveGameId.Value = id;
            command.Parameters.Add(saveGameId);

            command.ExecuteNonQuery();
        }
    }
}