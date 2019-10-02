using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class UnitOwner
    {
        protected readonly List<Unit> ownedUnits = new List<Unit>();
        protected readonly List<Unit> playableUnits = new List<Unit>();
        protected readonly List<Unit> ennemyUnits = new List<Unit>();

        private bool hasLost = false;
        public string Name = "";

        public bool HasLost
        {
            get => hasLost;
            set => hasLost = value;
        }

        public virtual void Play()
        {
            for(int i = 0; i < playableUnits.Count; i++)
            {
                if (!playableUnits[i].HasDoneAction)
                {
                    RemoveUnitFromPlayableUnits(playableUnits[i]);
                }
            }

            for(int i = 0; i < ownedUnits.Count; i++)
            {
                if (ownedUnits[i].IsDead)
                {
                    RemoveOwnedUnit(ownedUnits[i]);
                }
            }
        }

        public virtual void Lose()
        {
            MakeOwnedUnitsUnplayable();
            hasLost = true;
        }

        public virtual void Win()
        {
            
        }

        public void MakeOwnedUnitsUnplayable()
        {
            foreach (Unit unit in playableUnits)
            {
                unit.CanPlay = false;
            }
        }

        public void MakeOwnedUnitsPlayable()
        {
            foreach (Unit unit in playableUnits)
            {
                unit.CanPlay = true;
                unit.ResetNumberOfMovesLeft();
            }
        }

        public bool HasNoMoreMovableUnits()
        {
            return playableUnits.Count <= 0;
        }

        public bool HaveAllUnitsDied()
        {
            return ownedUnits.Count <= 0;
        }

        public void OnTurnGiven()
        {
            foreach(Unit unit in ownedUnits)
            playableUnits.Add(unit);
            MakeOwnedUnitsPlayable();
        }

        public void AddOwnedUnit(Unit unit)
        {
            ownedUnits.Add(unit);
        }

        public void RemoveOwnedUnit(Unit unit)
        {
            unit.CanPlay = false;
            if (playableUnits.Contains(unit))
                playableUnits.Remove(unit);
            if (ownedUnits.Contains(unit))
                ownedUnits.Remove(unit);
        }

        public void RemoveUnitFromPlayableUnits(Unit unit)
        {
            unit.CanPlay = false;
            playableUnits.Remove(unit);
        }
    }
}