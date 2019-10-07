using System;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Une action qu'une unité peut effectuer dans son tour, soit un mouvement et soit attaquer ou se reposer
    /// Auteur: Zacharie Lavigne
    /// </summary>
    public class Action
    {
        private List<Tile> path;
        /// <summary>
        /// Le mouvement à effectuer dans le tour
        /// </summary>
        public List<Tile> Path => path;
        
        private ActionType actionType;
        /// <summary>
        /// Le type d'action à effectuer après le mouvement
        /// </summary>
        public ActionType ActionType => actionType;
        
        /// <summary>
        /// Le score d'une action. C'est avec au score que l'unité artificielle détermine l'action qu'elle fera à son tour
        /// </summary>
        public float Score { get; set; }
        
        private Unit target;
        /// <summary>
        /// L'unité à attaquer si l'action à comme but d'attaquer
        /// </summary>
        public Unit Target => target;

        public Action(List<Tile> path, ActionType actionType, float score, Unit target = null)
        {
            this.path = path;
            this.actionType = actionType;
            this.Score = score;
            if (actionType == ActionType.Rest && target != null)
                throw new ArgumentException("A rest action cannot have a target unit");
            this.target = target;
        }
    }
}