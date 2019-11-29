using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Game
{
    //Author : Antoine Lessard
    public class AchievementsRepo : IRepository<AchievementInfo>
    {
        private readonly DbConnection connection;

        public AchievementsRepo(DbConnection connection)
        {
            this.connection = connection;
        }
        
        public void Insert(AchievementInfo myObject)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "INSERT INTO Achievements(id, achievement_name, achievement_description, achieved) VALUES (Null,?,?,?)";
                
                var achievementName = command.CreateParameter();
                achievementName.Value = myObject.AchievementName;
                command.Parameters.Add(achievementName);

                var achievementDescription = command.CreateParameter();
                achievementDescription.Value = myObject.AchievementDescription;
                command.Parameters.Add(achievementDescription);

                var achieved = command.CreateParameter();
                achieved.Value = myObject.Achieved;
                command.Parameters.Add(achieved);

                command.ExecuteNonQuery();
            }
        }

        public List<AchievementInfo> FindAll()
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "SELECT * FROM Achievements";

                var result = new List<AchievementInfo>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var achieved = Convert.ToBoolean(reader["achieved"]);
                        result.Add(new AchievementInfo(
                            reader["achievement_name"].ToString(),
                            reader["achievement_description"].ToString(),
                            achieved));
                    }
                }
                return result;
            }
        }

        public void Update(AchievementInfo myObject)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                
                command.CommandText = "UPDATE Achievements SET achieved = ? WHERE achievement_name = ?";

                var achieved = command.CreateParameter();
                achieved.Value = myObject.Achieved;
                command.Parameters.Add(achieved);

                var achievementName = command.CreateParameter();
                achievementName.Value = myObject.AchievementName;
                command.Parameters.Add(achievementName);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "DELETE * FROM Achievements";

                command.ExecuteNonQuery();
            }
        }
    }
}