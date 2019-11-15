using Harmony;

namespace Game
{
    /// <summary>
    /// OnUnitDeath event channel
    /// Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnUnitDeath : EventChannel<Unit>
    {
        public event EventHandler<Unit> Notify; 
        public override void Publish(Unit eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}