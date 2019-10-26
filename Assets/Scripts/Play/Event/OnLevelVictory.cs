using Harmony;

namespace Game
{
    public class OnLevelVictory : EventChannel<LevelController>
    {
        public static event EventHandler<LevelController> Notify; 
        public override void Publish(LevelController eventParam)
        {
            Notify(eventParam);
        }
    }
}