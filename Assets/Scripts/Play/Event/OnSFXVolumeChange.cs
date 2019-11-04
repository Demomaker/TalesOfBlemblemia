﻿using Harmony;

namespace Game
{
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnSFXVolumeChange : EventChannel<float>
    {
        public event EventHandler<float> Notify; 
        public override void Publish(float eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}