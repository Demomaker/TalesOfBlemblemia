﻿using Harmony;

namespace Game
{
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnMainMenuEnter : EventChannel<MainMenuController>
    {
        public event EventHandler<MainMenuController> Notify;
        public override void Publish(MainMenuController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}