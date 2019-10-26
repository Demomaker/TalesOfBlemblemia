using Harmony;

namespace Game
{
    public class OnMusicVolumeChange : EventChannel<float>
    {
        public static event EventHandler<float> Notify; 
        public override void Publish(float eventParam)
        {
            Notify(eventParam);
        }
    }
}