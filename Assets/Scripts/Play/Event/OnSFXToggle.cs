﻿using Harmony;

namespace Game
{
    public class OnSFXToggle : EventChannel<bool>
    {
        public static event EventHandler<bool> Notify; 
        public override void Publish(bool eventParam)
        {
            Notify(eventParam);
        }
    }
}