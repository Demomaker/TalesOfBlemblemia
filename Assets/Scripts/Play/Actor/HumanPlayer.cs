using UnityEngine;

namespace Game
{
    public class HumanPlayer : UnitOwner
    {

        private bool hasEverLostAUnit = false;
        private bool hasLostAUnitInCurrentLevel = false;
        private int numberOfRecruitedUnitsFromAlternativePath = 0;
        public bool HasEverLostAUnit => hasEverLostAUnit;
        public bool HasLostAUnitInCurrentLevel => hasLostAUnitInCurrentLevel;
        public int NumberOfUnits => ownedUnits.Count;
        public int NumberOfRecruitedUnitsFromAlternatePath => numberOfRecruitedUnitsFromAlternativePath;
        private static HumanPlayer instance = null;
        public static HumanPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HumanPlayer();
                }
                return instance;
            }
        }

        public override void CheckUnitDeaths()
        {
            base.CheckUnitDeaths();
        }

        public override void RemoveOwnedUnit(Unit unit)
        {
            base.RemoveOwnedUnit(unit);
            if (!hasLostAUnitInCurrentLevel)
            {
                Finder.SoundManager.StopCurrentMusic();
                Finder.SoundManager.PlayMusic(Finder.SoundClips.SadMusic);
            }
            hasLostAUnitInCurrentLevel = true;
            hasEverLostAUnit = true;
        }

        public override void OnNewLevel()
        {
            base.OnNewLevel();
            hasLostAUnitInCurrentLevel = false;
        }

        private HumanPlayer()
        {
        }
        
        
    }
}