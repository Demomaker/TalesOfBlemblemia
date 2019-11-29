using Harmony;

namespace Game
{
    /// <summary>
    /// OnLevelVictory event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnLevelVictory : EventChannel
    {
        public event EventHandler Notify; 
        public override void Publish()
        {
            Notify?.Invoke();
        }
    }
}