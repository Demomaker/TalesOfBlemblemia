using Harmony;

namespace Game
{
    /// <summary>
    /// OnOverWorldEnter event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnEndLevelEnter : EventChannel<EndSceneController>
    {
        public event EventHandler<EndSceneController> Notify;
        public override void Publish(EndSceneController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}