using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// The human player that controls its units
    /// Authors: Zacharie Lavigne, Mike Bédard, Antoine Lessard
    /// </summary>
    public class HumanPlayer : UnitOwner
    {
        #region Fields
        private const string HUMAN_PLAYER_NAME = "Player";
        private bool hasEverLostAUnit = false;
        private bool hasLostAUnitInCurrentLevel = false;
        private int numberOfRecruitedUnitsFromAlternativePath = 0;
        #endregion Fields
        #region Accessors
        public bool HasEverLostAUnit => hasEverLostAUnit;
        public bool HasLostAUnitInCurrentLevel => hasLostAUnitInCurrentLevel;
        public int NumberOfUnits => ownedUnits.Count;
        public int NumberOfRecruitedUnitsFromAlternatePath => numberOfRecruitedUnitsFromAlternativePath;
        private static HumanPlayer instance = null;
        private List<Unit> defeatedUnits = new List<Unit>();
        public List<Unit> DefeatedUnits => defeatedUnits;

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
        #endregion Accessors
        #region Constructors
        private HumanPlayer()
        {
            name = HUMAN_PLAYER_NAME;
        }
        #endregion Constructors
        #region HumanPlayer-related Functions
        public void OnNewLevel()
        {
            hasLostAUnitInCurrentLevel = false;
        }
        
        public override void RemoveOwnedUnit(Unit unit)
        {
            base.RemoveOwnedUnit(unit);
            hasLostAUnitInCurrentLevel = true;
            hasEverLostAUnit = true;
            
            defeatedUnits.Add(unit);
        }
        #endregion HumanPlayer-related Functions
    }
}