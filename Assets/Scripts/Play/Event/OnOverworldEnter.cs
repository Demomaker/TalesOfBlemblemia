using Harmony;

namespace Game
{
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnOverworldEnter : EventChannel<OverworldController>
    {
        public event EventHandler<OverworldController> Notify;
        public override void Publish(OverworldController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}