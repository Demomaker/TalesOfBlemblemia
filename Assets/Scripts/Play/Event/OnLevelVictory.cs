using Harmony;
using JetBrains.Annotations;

namespace Game
{
    /// <summary>
    /// OnLevelVictory event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnLevelVictory : EventChannel<LevelController>
    {
        public event EventHandler<LevelController> Notify;
        public override void Publish(LevelController levelController)
        {
            Notify?.Invoke(levelController);
        }
    }
}