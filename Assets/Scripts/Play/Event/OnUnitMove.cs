using Harmony;

namespace Game
{
    public class OnUnitMove : EventChannel<Unit>
    {
        public static event EventHandler<Unit> Notify;
        public override void Publish(Unit eventParam)
        {
            Notify(eventParam);
        }
    }
}