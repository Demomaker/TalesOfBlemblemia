namespace Game
{
    public class CharacterInfo
    {
        public string CharacterName { get; set; }
        public bool CharacterStatus { get; set; }
        public int SaveId { get; set; }

        public CharacterInfo(string characterName, bool characterStatus, int saveId)
        {
            CharacterStatus = characterStatus;
            CharacterName = characterName;
            SaveId = saveId;
        }
    }
}