using Harmony;

namespace Game
{
    /// <summary>
    /// OnOverWorldEnter event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnOverWorldEnter : EventChannel<OverWorldController>
    {
        public event EventHandler<OverWorldController> Notify;
        public override void Publish(OverWorldController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}