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
        }

        public IEnumerator PlayUnits()
        {
            for (int i = 0; i < ownedUnits.Count; i++)
            {
                var currentUnit = ownedUnits[i];
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