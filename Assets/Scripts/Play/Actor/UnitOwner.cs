using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// A virtual player, be it human or artificial
    /// Authors: Mike Bédard, Zacharie Lavigne
    /// </summary>
    public class UnitOwner
    {
        protected readonly List<Unit> ownedUnits = new List<Unit>();
        protected readonly List<Unit> enemyUnits = new List<Unit>();
        protected int numberOfStartingOwnedUnits;
        private string name = "";
        public string Name => name;
        public List<Unit> OwnedUnits => ownedUnits;

        
        
        public bool HasNoMorePlayableUnits
        {
            get
            {
                for (int i = 0; i < ownedUnits.Count; i++)
                {
                    if (!ownedUnits[i].HasActed)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool HasLost { get; set; }

        public int RemoveDeadUnits()
        {
            int unitsRemoved = 0;
            for (int i = 0; i < ownedUnits.Count; i++)
            {
                if (ownedUnits[i].NoHealthLeft)
                {
                    RemoveOwnedUnit(ownedUnits[i]);
                    unitsRemoved++;
                }
            }
            return unitsRemoved;
        }

        public void Lose()
        {
            MakeOwnedUnitsUnplayable();
            HasLost = true;
        }

        public virtual void Win()
        {
            //TODO: Remove or to be completed
        }

        public void MakeOwnedUnitsUnplayable()
        {
            for (int i = 0; i < ownedUnits.Count; i++)
            {
                ownedUnits[i].HasActed = true;
            }
        }

        private void MakeOwnedUnitsPlayable()
        {
            foreach (var unit in ownedUnits)
            {
                unit.ResetTurnStats();
            }
        }

        public bool HaveAllUnitsDied()
        {
            return ownedUnits.Count <= 0;
        }

        public void OnTurnGiven()
        {
            MakeOwnedUnitsPlayable();
        }

        public void AddOwnedUnit(Unit unit)
        {
            ownedUnits.Add(unit);
        }

        public void UpdateNumberOfStartingOwnedUnits()
        {
            numberOfStartingOwnedUnits = ownedUnits.Count;
        }

        public virtual void RemoveOwnedUnit(Unit unit)
        {
            unit.HasActed = true;
            if (ownedUnits.Contains(unit))
            {
                ownedUnits.Remove(unit);
            }
        }
        
        public void AddEnemyUnit(Unit enemy)
        {
            enemyUnits.Add(enemy);
        }

        public virtual void OnNewLevel()
        {
            //Nothing to do on purpose
        }
    }
}