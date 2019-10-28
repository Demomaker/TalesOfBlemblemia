using Harmony;

namespace Game
{
    public class OnDodge : EventChannel<Unit>
    {
        public static event EventHandler<Unit> Notify;
        public override void Publish(Unit eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}