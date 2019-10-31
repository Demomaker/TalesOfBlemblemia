using System;
using System.Collections.Generic;

namespace Game
{
    public class Action
    {
        private List<Tile> path;
        public List<Tile> Path => path;
        
        private ActionType actionType;
        public ActionType ActionType => actionType;
        
        public float Score { get; set; }
        
        private Targetable target;
        public Targetable Target => target;

        public Action(List<Tile> path, ActionType actionType = ActionType.Nothing, Targetable target = null, float score = 0f)
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