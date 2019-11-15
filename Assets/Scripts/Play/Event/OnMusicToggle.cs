using Harmony;

namespace Game
{
    /// <summary>
    /// OnMusicToggle event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnMusicToggle : EventChannel<bool>
    {
        public event EventHandler<bool> Notify; 
        public override void Publish(bool eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}