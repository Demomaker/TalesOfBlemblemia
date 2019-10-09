namespace Game
{
    public struct CharacterInfo
    {
        public string characterName;
        public bool characterStatus;
        public int saveId;

        public CharacterInfo(string characterName, bool characterStatus, int saveId)
        {
            this.characterStatus = characterStatus;
            this.characterName = characterName;
            this.saveId = saveId;
        }
    }
}