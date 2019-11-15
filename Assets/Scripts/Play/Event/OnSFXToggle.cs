using Harmony;

namespace Game
{
    /// <summary>
    /// OnSFXToggle event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnSFXToggle : EventChannel<bool>
    {
        public event EventHandler<bool> Notify; 
        public override void Publish(bool eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}