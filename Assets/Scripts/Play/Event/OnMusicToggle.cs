using Harmony;

namespace Game
{
    public class OnMusicToggle : EventChannel<bool>
    {
        public static event EventHandler<bool> Notify; 
        public override void Publish(bool eventParam)
        {
            Notify(eventParam);
        }
    }
}