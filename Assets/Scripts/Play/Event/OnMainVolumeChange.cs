﻿using Harmony;

namespace Game
{
    public class OnMainVolumeChange : EventChannel<float>
    {
        public static event EventHandler<float> Notify; 
        public override void Publish(float eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}