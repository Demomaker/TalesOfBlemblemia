﻿using Harmony;

namespace Game
{
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnMusicVolumeChange : EventChannel<float>
    {
        public event EventHandler<float> Notify; 
        public override void Publish(float eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}