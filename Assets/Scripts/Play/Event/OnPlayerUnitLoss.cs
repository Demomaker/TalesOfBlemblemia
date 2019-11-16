using Harmony;

namespace Game
{
    /// <summary>
    /// OnPlayerUnitLoss event channel
    /// Author : Mike Bédard
    /// </summary>
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