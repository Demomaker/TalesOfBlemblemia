using Harmony;

namespace Game
{
    public class OnUnitDeath : EventChannel<Unit>
    {
        public static event EventHandler<Unit> Notify; 
        public override void Publish(Unit eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}