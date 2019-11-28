using System.Collections.Generic;
using System.Linq;

namespace Game
{
    /// <summary>
    /// A virtual player, be it human or artificial
    /// Authors: Mike Bédard, Zacharie Lavigne, Jérémie Bertrand
    /// </summary>
    public class UnitOwner
    {
        protected readonly List<Unit> ownedUnits = new List<Unit>();
        protected readonly List<Unit> enemyUnits = new List<Unit>();
        private bool hasLostAUnitInCurrentLevel;

        public int NumberOfUnits => ownedUnits.Count;
        public List<Unit> DefeatedUnits { get; } = new List<Unit>();
        public string Name { get; }

        public bool HaveAllUnitsDied => ownedUnits.Count <= 0;
        public List<Unit> OwnedUnits => ownedUnits;
        public bool HasNoMorePlayableUnits => ownedUnits.All(t => t.HasActed);

        protected UnitOwner(string name)
        {
            Name = name;
        }

        public void OnTurnGiven()
        {
            ownedUnits.ForEach(unit => unit.ResetTurnStats());
        }

        public void AddOwnedUnit(Unit unit)
        {
            ownedUnits.Add(unit);
        }
        
        public void AddEnemyUnit(Unit enemy)
        {
            enemyUnits.Add(enemy);
        }
        
        private void RemoveOwnedUnit(Unit unit)
        {
            if (ownedUnits.Contains(unit))
            {
                unit.HasActed = true;
                hasLostAUnitInCurrentLevel = true;
                ownedUnits.Remove(unit);
            }
        }

        public void RemoveEnemyUnit(Unit enemy)
        {
            if (enemyUnits.Contains(enemy))
            {
                DefeatedUnits.Add(enemy);
                enemyUnits.Remove(enemy);
            }
        }
        
        public void RemoveDeadUnits()
        {
            for (int i = ownedUnits.Count - 1; i >= 0; i--)
            {
                var unit = ownedUnits[i];
                if(unit.NoHealthLeft) RemoveOwnedUnit(unit);
            }
            for (int i = enemyUnits.Count - 1; i >= 0; i--)
            {
                var unit = enemyUnits[i];
                if(unit.NoHealthLeft) RemoveEnemyUnit(unit);
            }
        }

        public void ResetOwnedUnitsAlpha()
        {
            ownedUnits.ForEach(unit => unit.ResetAlpha());
        }
    }
}