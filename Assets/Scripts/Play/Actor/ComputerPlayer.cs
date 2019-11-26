using System;
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
        private int dynamicUnitCounter;
        private int dynamicUnitCount;
        private Unit currentUnit = null;
        private static ComputerPlayer instance = null;
        private const string COMPUTER_PLAYER_NAME = "Enemy";
        private List<Targetable> targetsToDestroy;
        private OnUnitDeath onUnitDeath;
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
            name = COMPUTER_PLAYER_NAME;
            onUnitDeath = Harmony.Finder.OnUnitDeath;
            onUnitDeath.Notify += OnUnitDeath;
        }
        #endregion Constructors
        #region ComputerPlayer-related Functions
        
        public void AddTarget(Targetable target)
        {
            targetsToDestroy.Add(target);
        }
        
        private void OnUnitDeath(Unit unit)
        {
            if (unit != currentUnit) return;
            dynamicUnitCount--;
            dynamicUnitCounter--;
        }

        public IEnumerator PlayUnits()
        {
            dynamicUnitCount = ownedUnits.Count;
            for (dynamicUnitCounter = 0; dynamicUnitCounter < dynamicUnitCount; dynamicUnitCounter++)
            {
                var uiController = Harmony.Finder.UIController;
                while (uiController.IsBattleReportActive)
                {
                    yield return null;
                }
                
                currentUnit = ownedUnits[dynamicUnitCounter];
                
                if (!currentUnit.HasActed)
                {
                    var action = AiController.DetermineAction(currentUnit, enemyUnits, targetsToDestroy);
                    
                    yield return currentUnit.MoveByAction(action);
                }
            }
        }
        #endregion ComputerPlayer-related Functions
    }
}