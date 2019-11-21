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
        private static ComputerPlayer instance = null;
        private List<Targetable> targetsToDestroy;
        private OnUnitDeath onUnitDeath;

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

        private ComputerPlayer()
        {
            targetsToDestroy = new List<Targetable>();
            onUnitDeath = Harmony.Finder.OnUnitDeath;
            onUnitDeath.Notify += OnUnitDeath;
        }

        
        public void AddTarget(Targetable target)
        {
            targetsToDestroy.Add(target);
        }
        
        private void OnUnitDeath(Unit unit)
        {
            if (unit == currentUnit)
            {
                dynamicUnitCount--;
                dynamicUnitCounter--;
            }
        }

        private int dynamicUnitCounter;
        private int dynamicUnitCount;
        private Unit currentUnit = null;

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

                try
                {
                    currentUnit = ownedUnits[dynamicUnitCounter];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                if (!currentUnit.HasActed)
                {
                    var action = AiController.DetermineAction(currentUnit, enemyUnits, targetsToDestroy);
                    
                    yield return currentUnit.MoveByAction(action);
                    
                }
            }
        }
    }
}