﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace Game
{
    public class SaveSettingsRepo : Repository<PlayerSettings>
    {
        private readonly DbConnection connection;

        public SaveSettingsRepo(DbConnection connection)
        {
            this.connection = connection;
        }

        public void Insert(PlayerSettings myObject)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO playersettings(playerId, musicToggle, sfxToggle, mainVolume, musicVolume, sfxVolume) VALUES(?,?,?,?,?,?)";
            
            #region SettingUpParameters
            var playerIdParameter = command.CreateParameter();
            playerIdParameter.Value = myObject.PlayerId;
            command.Parameters.Add(playerIdParameter);

            var musicToggleParameter = command.CreateParameter();
            musicToggleParameter.Value = Convert.ToInt32(myObject.MusicToggle);
            command.Parameters.Add(musicToggleParameter);

            var sfxToggleParameter = command.CreateParameter();
            sfxToggleParameter.Value = Convert.ToInt32(myObject.SfxToggle);
            command.Parameters.Add(sfxToggleParameter);

            var mainVolumeParameter = command.CreateParameter();
            mainVolumeParameter.Value = myObject.MainVolume;
            command.Parameters.Add(mainVolumeParameter);

            var musicVolumeParameter = command.CreateParameter();
            musicVolumeParameter.Value = myObject.MusicVolume;
            command.Parameters.Add(musicVolumeParameter);

            var sfxVolumeParameter = command.CreateParameter();
            sfxVolumeParameter.Value = myObject.SfxVolume;
            command.Parameters.Add(sfxVolumeParameter);
            #endregion
            
            command.ExecuteNonQuery();
        }

        public PlayerSettings Find(long id)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM playersettings WHERE playerid = ?";

            var playerIdParameter = command.CreateParameter();
            playerIdParameter.Value = id;
            command.Parameters.Add(playerIdParameter);

            DbDataReader reader = command.ExecuteReader();
            PlayerSettings playerSettings = new PlayerSettings(0,0,true,true,0,0,0);
            while (reader.Read())
            {
                playerSettings = PlayerSettingsBuilder(reader);
            }

            return playerSettings;
        }

        public List<PlayerSettings> FindAll()
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM playersettings";
            
            List<PlayerSettings> result = new List<PlayerSettings>();
            DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(PlayerSettingsBuilder(reader));
            }

            return result;
        }

        public void Update(PlayerSettings myObject)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                "UPDATE playersettings SET musicToggle = ?, sfxToggle = ?, mainVolume = ?, musicVolume = ?, sfxVolume = ? WHERE id = ?";

            #region SettingUpParameters
            var musicToggleParameter = command.CreateParameter();
            musicToggleParameter.Value = myObject.MusicToggle;
            command.Parameters.Add(musicToggleParameter);
            
            var sfxToggleParameter = command.CreateParameter();
            sfxToggleParameter.Value = myObject.SfxToggle;
            command.Parameters.Add(sfxToggleParameter);
            
            var mainVolumeParameter = command.CreateParameter();
            mainVolumeParameter.Value = myObject.MainVolume;
            command.Parameters.Add(mainVolumeParameter);
            
            var musicVolumeParameter = command.CreateParameter();
            musicVolumeParameter.Value = myObject.MusicVolume;
            command.Parameters.Add(musicVolumeParameter);
            
            var sfxVolumeParameter = command.CreateParameter();
            sfxVolumeParameter.Value = myObject.SfxVolume;
            command.Parameters.Add(sfxVolumeParameter);
            
            var id = command.CreateParameter();
            id.Value = myObject.Id;
            command.Parameters.Add(id);
            #endregion
            
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM playersettings WHERE playerId = ?";

            var playerSettingsToDelete = command.CreateParameter();
            playerSettingsToDelete.Value = id;
            command.Parameters.Add(playerSettingsToDelete);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// This method creates an instance of PlayerSettings and places the information read in the database in it.
        /// </summary>
        /// <param name="reader">The reader to get the information from the database</param>
        /// <returns>A new instance of PlayerSetting with the settings in the database for the player</returns>
        private static PlayerSettings PlayerSettingsBuilder(DbDataReader reader)
        {
            PlayerSettings playerSettings = new PlayerSettings(0,0,true,true,0,0,0);
            try
            {
                playerSettings = new PlayerSettings(Convert.ToInt32(reader["id"]),
                    Convert.ToInt32(reader["playerId"]),
                    Convert.ToBoolean(reader["musicToggle"]),
                    Convert.ToBoolean(reader["sfxToggle"]),
                    Convert.ToInt32(reader["mainVolume"]),
                    Convert.ToInt32(reader["musicVolume"]),
                    Convert.ToInt32(reader["sfxVolume"]));
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            return playerSettings;
        }
    }
}
