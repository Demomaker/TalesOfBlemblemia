using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// A virtual player, be it human or artificial
    /// Authors: Mike Bédard, Zacharie Lavigne
    /// </summary>
    public class UnitOwner
    {
        protected readonly List<Unit> ownedUnits = new List<Unit>();
        //BR : Ne devriez vous pas plutôt avoir une référence vers un autre "UnitOwner" ?
        //     Actuellement, il n'y a pas qu'une seule source de vérité.
        protected readonly List<Unit> enemyUnits = new List<Unit>();
        //BC : Variable assignée, mais jamais utilisée.
        protected int numberOfStartingOwnedUnits;
        private bool hasLost = false;
        //BC : Code mort. Attribut inutilisé.
        private string name = "";
        public string Name => name;

        //BR : Évitez les noms de méthodes qui utilisent une forme négatives.
        //     Mettez ça au positif, genre "HasPlayableUnits" et lorsque vous utilisez
        //     cette propriété, inversez la.
        public bool HasNoMorePlayableUnits
        {
            get
            {
                //BR : N'avais-je pas dit de utiliser des boucles "for each".
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

        //BR : Code mort...même si je me doute que ça va être utilisé plus tard.
        //     Évitez de prévoir trop à l'avance.
        public bool HasLost
        {
            get => hasLost;
            set => hasLost = value;
        }

        //BC : Ça devrait pas être nécessaire. Actuellement, la classe "UnitOwner" est invalide pendant un
        //     certains temps tandis que le "ComputerPlayer" joue. Une classe doit toujours conserver
        //     un état valide.
        //
        //     Ça devrait être automatique.
        public virtual void CheckUnitDeaths()
        {
            for (int i = 0; i < ownedUnits.Count; i++)
            {
                if (ownedUnits[i].NoHealthLeft)
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
            //BC : Vide, sans commentaire.
        }

        public void MakeOwnedUnitsUnplayable()
        {
            //BR : For each
            for (int i = 0; i < ownedUnits.Count; i++)
            {
                ownedUnits[i].HasActed = true;
            }
        }

        private void MakeOwnedUnitsPlayable()
        {
            //BR : Et pourtant, ce fut fait ici...
            foreach (var unit in ownedUnits)
            {
                unit.ResetTurnStats();
            }
        }

        //BC : Défaut de cohérence. Pourquoi "HasNoMorePlayableUnits" est une propriété,
        //     mais que ceci est une méthode ?
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

        //BC : Défaut d'orienté objet. Cela devrait être automatique.
        //     J'ai pas le temps de tout expliquer, venez me voir au pire pour que l'on arrange ça.
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
            //BR : Et...comment enlève-t-on une unité ennemie ?
            enemyUnits.Add(enemy);
        }
        //BC : Méthode vide, sans commentaire.
        public virtual void OnNewLevel()
        {
            
        }
    }
}