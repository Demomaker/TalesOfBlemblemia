﻿using Harmony;

namespace Game
{
    /// <summary>
    /// OnHurt event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnMovementChange : EventChannel
    {
        public event EventHandler Notify;

        public override void Publish()
        {
            Notify?.Invoke();
        }
    }
}