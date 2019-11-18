using Harmony;

namespace Game
{
    /// <summary>
    /// OnMainVolumeChange event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnMainVolumeChange : EventChannel<float>
    {
        public event EventHandler<float> Notify; 
        public override void Publish(float eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}