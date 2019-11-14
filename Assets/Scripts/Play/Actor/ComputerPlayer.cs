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
            targetsToDestroy = new List<Targetable>();  
        }

        public void FetchUiController()
        {
            uiController = Harmony.Finder.UIController;
        }

        public void AddTarget(Targetable target)
        {
            targetsToDestroy.Add(target);
        }
        
        public IEnumerator PlayUnits()
        {
            //TODO les unités qui meurent altèrent la liste du foreach et font crasher si une unité meurt pendant sont tour
            foreach (var unit in ownedUnits)
            {
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