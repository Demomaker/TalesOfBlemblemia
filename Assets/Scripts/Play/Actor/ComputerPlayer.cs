using System.Collections;
using System.Collections.Generic;

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
        
        private int dynamicUnitCounter;
        private int dynamicUnitCount;
        private Unit currentUnit ;
        
        public ComputerPlayer(int nbOfChoice) : base(COMPUTER_PLAYER_NAME)
        {
            aiController = new AiController(nbOfChoice);
            targetsToDestroy = new List<Targetable>();
        }

        public void OnUnitDeath(Unit unit)
        {
            if (unit != currentUnit) return;
            dynamicUnitCount--;
            dynamicUnitCounter--;
        }

        public void AddTarget(Targetable target)
        {
            targetsToDestroy.Add(target);
        }
        
        public IEnumerator PlayUnits(GridController grid, LevelController levelController, UIController uiController)
        {
            dynamicUnitCount = ownedUnits.Count;
            for (dynamicUnitCounter = 0; dynamicUnitCounter < dynamicUnitCount; dynamicUnitCounter++)
            {
                while (uiController.IsBattleReportActive) yield return null;
                currentUnit = dynamicUnitCount > 0 ? ownedUnits[dynamicUnitCounter] : currentUnit = ownedUnits[0];

                if (!currentUnit.HasActed && !levelController.LevelEnded)
                {
                    var action = aiController.DetermineAction(currentUnit, enemyUnits, targetsToDestroy, grid, levelController);
                    
                    yield return currentUnit.MoveByAction(action);
                }
            }
        }
    }
}