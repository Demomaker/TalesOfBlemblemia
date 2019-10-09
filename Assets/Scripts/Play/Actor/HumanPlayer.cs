using UnityEngine;

namespace Game
{
    public class HumanPlayer : UnitOwner
    {

        public bool HasEverLostAUnit = false;
        public bool HasLostAUnitInCurrentLevel = false;
        private int numberOfRecruitedUnitsFromAlternativePath = 0;
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
            HasLostAUnitInCurrentLevel = true;
            HasEverLostAUnit = true;
        }

        public override void OnNewLevel()
        {
            base.OnNewLevel();
            HasLostAUnitInCurrentLevel = false;
        }

        private HumanPlayer()
        {
        }
        
        
    }
}