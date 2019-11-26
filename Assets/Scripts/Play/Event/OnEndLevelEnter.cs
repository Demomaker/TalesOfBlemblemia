using Harmony;

namespace Game
{
    /// <summary>
    /// OnOverWorldEnter event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnEndLevelEnter : EventChannel<EndLevelController>
    {
        public event EventHandler<EndLevelController> Notify;
        public override void Publish(EndLevelController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}