using Game;
using Harmony;

namespace Game
{
    /// <summary>
    /// OnLevelChange event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnLevelChange : EventChannel<LevelController>
    {
        public event EventHandler<LevelController> Notify;
        public override void Publish(LevelController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}