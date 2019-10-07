using System.Collections;
using UnityEngine;

namespace Game
{
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

        public override void CheckUnitDeaths()
        {
            Debug.Log("Beginning of ai turn");
            base.CheckUnitDeaths();
        }

        public IEnumerator PlayUnits()
        {
            for (int i = 0; i < ownedUnits.Count; i++)
            {
                var currentUnit = ownedUnits[i];
                if (!currentUnit.HasActed)
                {
                    currentUnit.ComputeTilesCosts();
                    var action = AiController.DetermineAction(currentUnit, enemyUnits);
                    while (!currentUnit.HasActed)
                    {
                        currentUnit.ExecuteAction(action);
                        yield return null;
                    }
                    base.CheckUnitDeaths();
                }
            } 
        }
    }
}