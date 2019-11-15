﻿using Harmony;

namespace Game
{
    /// <summary>
    /// OnUnitMove event channel
    /// </summary>
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnUnitMove : EventChannel<Unit>
    {
        public event EventHandler<Unit> Notify;
        public override void Publish(Unit eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}