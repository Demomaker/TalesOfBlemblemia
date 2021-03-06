﻿using System.Collections.Generic;
using System.Linq;

namespace Game
{
    /// <summary>
    /// A virtual player, be it human or artificial
    /// Authors: Zacharie Lavigne, Jérémie Bertrand, Mike Bédard
    /// </summary>
    public class UnitOwner
    {
        protected readonly List<Unit> ownedUnits = new List<Unit>();
        protected readonly List<Unit> enemyUnits = new List<Unit>();

        public int NumberOfUnits => ownedUnits.Count;
        public List<Unit> DefeatedUnits { get; } = new List<Unit>();
        public string Name { get; }
        public bool HaveAllUnitsDied => ownedUnits.Count <= 0 || ownedUnits.All(unit => unit.NoHealthLeft);
        public List<Unit> OwnedUnits => ownedUnits;
        public bool HasNoMorePlayableUnits => ownedUnits.All(t => t.HasActed);
        public bool HasLostAUnitInCurrentLevel { get; private set; }

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
                HasLostAUnitInCurrentLevel = true;
                DefeatedUnits.Add(unit);
                ownedUnits.Remove(unit);
            }
        }

        public void RemoveEnemyUnit(Unit enemy)
        {
            if (enemyUnits.Contains(enemy))
            {
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