using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The computer player that controls its units
    /// Authors: Zacharie Lavigne, Pierre-Luc Maltais, Jérémie Bertrand
    /// </summary>
    public class ComputerPlayer : UnitOwner
    {
        private const string COMPUTER_PLAYER_NAME = "Enemy";
        private readonly AiController aiController;
        private readonly List<Targetable> targetsToDestroy;
        
        private Unit currentUnit ;
        
        public ComputerPlayer(int nbOfChoice) : base(COMPUTER_PLAYER_NAME)
        {
            aiController = new AiController(nbOfChoice);
            targetsToDestroy = new List<Targetable>();
        }

        public void AddTarget(Targetable target)
        {
            targetsToDestroy.Add(target);
        }
        
        public IEnumerator PlayUnits(GridController grid, LevelController levelController, UIController uiController)
        {
            for (int i = ownedUnits.Count - 1; i >= 0; i--)
            {
                while (uiController.IsBattleReportActive) yield return null;
                currentUnit = ownedUnits[i];
                if (!currentUnit.HasActed && !levelController.LevelEnded)
                {
                    var action = aiController.DetermineAction(currentUnit, enemyUnits, targetsToDestroy, grid, levelController);
                    yield return currentUnit.MoveByAction(action);
                }
            }
        }
    }
}