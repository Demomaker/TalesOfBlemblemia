using System;
using System.Collections.Generic;

namespace Game
{
    //Author: Zacharie Lavigne
    public class Action
    {
        private List<Tile> path;
        private Targetable target;
        private ActionType actionType;

        public List<Tile> Path => path;
        public Targetable Target => target;
        public ActionType ActionType => actionType;
        public float Score { get; set; }

        public Action(List<Tile> path, ActionType actionType = ActionType.Nothing, Targetable target = null, float score = 0f)
        {
            this.path = path;
            this.actionType = actionType;
            Score = score;
            if (actionType == ActionType.Rest && target != null)
                throw new ArgumentException("A rest action cannot have a target unit");
            this.target = target;
        }
    }
}