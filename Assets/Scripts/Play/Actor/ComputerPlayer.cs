using System.Collections;
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
        }

        public IEnumerator PlayUnits()
        {
            foreach (var unit in ownedUnits)
            {
                while (uiController.IsBattleReportActive)
                {
                    yield return null;
                } 
                var currentUnit = unit;
                     
                if (!currentUnit.HasActed)
                {
                    var action = AiController.DetermineAction(currentUnit, enemyUnits);
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