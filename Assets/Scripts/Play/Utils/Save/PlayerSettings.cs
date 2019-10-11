namespace Game
{
    public struct PlayerSettings
    {
        public int Id { get; }
        public bool MusicToggle { get; set; }
        public bool SfxToggle { get; set; }
        public int MainVolume { get; set; }
        public int MusicVolume { get; set; }
        public int SfxVolume { get; set; }

        public PlayerSettings(int id, bool musicToggle, bool sfxToggle, int mainVolume, int musicVolume, int sfxVolume)
        {
            Id = id;
            MusicToggle = musicToggle;
            SfxToggle = sfxToggle;
            MainVolume = mainVolume;
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
        }
    }
}