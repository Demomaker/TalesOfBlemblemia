using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The computer player that controls its units
    /// Authors: Jérémie Bertrand, Zacharie Lavigne
    /// </summary>
    public class ComputerPlayer : UnitOwner
    {
        //BR : no....nO....NO....NOOOO!!!!!!!!!! PAS UN SINGLETON!!!!!!!!!!!!!!
        //     Ça risque de vous causer de nombreux problèmes, sérieusement!
        private static ComputerPlayer instance = null;
        private List<Targetable> targetsToDestroy;
        
        private UIController uiController;

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
            uiController = Harmony.Finder.UIController;
            targetsToDestroy = new List<Targetable>();  
        }

        public void AddTarget(Targetable target)
        {
            targetsToDestroy.Add(target);
        }
        
        public IEnumerator PlayUnits()
        {
            foreach (var unit in ownedUnits)
            {
                //BC : Le UI ne devrait pas déborder ici. Aucun rapport.
                while (uiController.IsBattleReportActive)
                {
                    yield return null;
                } 
                var currentUnit = unit;
                     
                if (!currentUnit.HasActed)
                {
                    var action = AiController.DetermineAction(currentUnit, enemyUnits, targetsToDestroy);
                    while (!currentUnit.HasActed)
                    {
                        yield return currentUnit.MoveByAction(action);
                    }
                    base.CheckUnitDeaths();
                }
            }
        }
    }
}