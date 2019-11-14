using Harmony;

namespace Game
{
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnPlayerUnitLoss : EventChannel<Unit>
    {
        public event EventHandler<Unit> Notify; 
        public override void Publish(Unit eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}