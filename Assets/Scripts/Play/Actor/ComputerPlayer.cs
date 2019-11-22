using System.Collections;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// The computer player that controls its units
    /// Authors: Zacharie Lavigne, Pierre-Luc Maltais
    /// </summary>
    public class ComputerPlayer : UnitOwner
    {
        #region Fields
        private static ComputerPlayer instance = null;
        private List<Targetable> targetsToDestroy;
        #endregion Fields
        #region Accessors
        public static ComputerPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComputerPlayer();
                }
                return instance;
            }
        }
        #endregion Accessors
        #region Constructors
        private ComputerPlayer()
        {
            targetsToDestroy = new List<Targetable>();  
        }
        #endregion Constructors
        #region ComputerPlayer-related Functions
        public void AddTarget(Targetable target)
        {
            targetsToDestroy.Add(target);
        }
        
        public IEnumerator PlayUnits()
        {
            for (int i = 0; i < ownedUnits.Count; i++)
            {
                var uiController = Harmony.Finder.UIController;
                while (uiController.IsBattleReportActive)
                {
                    yield return null;
                } 
                var currentUnit = ownedUnits[i];
                     
                if (!currentUnit.HasActed)
                {
                    var action = AiController.DetermineAction(currentUnit, enemyUnits, targetsToDestroy);
                    
                    while (!currentUnit.HasActed)
                    {
                        yield return currentUnit.MoveByAction(action);
                    }

                    base.RemoveDeadUnits();
                }
            }
        }
        #endregion ComputerPlayer-related Functions
    }
}