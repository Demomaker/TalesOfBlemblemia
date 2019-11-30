using Harmony;

namespace Game
{
    /// <summary>
    /// OnLevelFailed event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnLevelFailed : EventChannel
    {
        public event EventHandler Notify; 
        public override void Publish()
        {
            Notify?.Invoke();
        }
    }
}