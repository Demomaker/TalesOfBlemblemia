using Game;
using Harmony;

namespace Game
{
    public class OnLevelChange : EventChannel<LevelController>
    {
        public static event EventHandler<LevelController> Notify;
        public override void Publish(LevelController eventParam)
        {
            Notify(eventParam);
        }
    }
}