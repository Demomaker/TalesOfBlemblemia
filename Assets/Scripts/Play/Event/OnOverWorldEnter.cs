using Harmony;

namespace Game
{
    [Findable(Tags.GAME_CONTROLLER_TAG)]
    public class OnOverWorldEnter : EventChannel<OverWorldController>
    {
        public event EventHandler<OverWorldController> Notify;
        public override void Publish(OverWorldController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}