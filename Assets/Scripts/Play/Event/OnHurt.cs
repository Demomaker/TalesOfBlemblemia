using Harmony;

namespace Game
{
    /// <summary>
    /// OnHurt event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnHurt : EventChannel<Unit>
    {
        public event EventHandler<Unit> Notify;
        public override void Publish(Unit eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}