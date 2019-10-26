using Harmony;

namespace Game
{
    public class OnOverworldEnter : EventChannel<OverworldController>
    {
        public static event EventHandler<OverworldController> Notify;
        public override void Publish(OverworldController eventParam)
        {
            Notify(eventParam);
        }
    }
}