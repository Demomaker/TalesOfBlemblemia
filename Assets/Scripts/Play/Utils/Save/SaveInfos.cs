using System.Collections.Generic;

namespace Game
{
    public class SaveInfos
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string DifficultyLevel { get; set; }

        public string LevelName { get; set; }

        public List<CharacterInfo> CharacterInfos { get; set; }

        public SaveInfos(int id, string username, string difficultyLevel, string levelName,
            Dictionary<string, bool> characterStatus)
        {
            Id = id;
            Username = username;
            DifficultyLevel = difficultyLevel;
            LevelName = levelName;
            CharacterInfos = new List<CharacterInfo>();
            if (characterStatus != null)
            {
                foreach (var characterStatusDictionary in characterStatus)
                {
                    CharacterInfos.Add(new CharacterInfo(characterStatusDictionary.Key, characterStatusDictionary.Value,
                        id));
                }
            }
        }
        
        public SaveInfos()
        {
            Id = 0;
            Username = "";
            DifficultyLevel = "";
            LevelName = "";
            CharacterInfos = null;
        }
    }
}