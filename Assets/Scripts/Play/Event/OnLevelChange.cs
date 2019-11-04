using Game;
using Harmony;

namespace Game
{
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnLevelChange : EventChannel<LevelController>
    {
        public event EventHandler<LevelController> Notify;
        public override void Publish(LevelController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}