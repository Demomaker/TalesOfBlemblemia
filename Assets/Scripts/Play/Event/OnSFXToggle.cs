﻿using Harmony;

namespace Game
{
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnSFXToggle : EventChannel<bool>
    {
        public event EventHandler<bool> Notify; 
        public override void Publish(bool eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}