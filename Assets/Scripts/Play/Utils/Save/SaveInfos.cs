using System.Collections.Generic;

namespace Game
{
    public struct SaveInfos
    {
        public int id;

        public string username;

        public string difficultyLevel;

        public string levelName;

        public List<CharacterInfo> characterInfos;

        public SaveInfos(int id, string username, string difficultyLevel, string levelName,
            Dictionary<string, bool> characterStatus)
        {
            this.id = id;
            this.username = username;
            this.difficultyLevel = difficultyLevel;
            this.levelName = levelName;
            characterInfos = new List<CharacterInfo>();
            if (characterStatus != null)
            {
                foreach (var characterStatusDictionary in characterStatus)
                {
                    characterInfos.Add(new CharacterInfo(characterStatusDictionary.Key, characterStatusDictionary.Value,
                        id));
                }
            }
        }
    }
}