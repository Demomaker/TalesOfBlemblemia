using Harmony;
using UnityEngine.UI;

namespace Game
{
    public class OnButtonClick : EventChannel<Button>
    {
        public static event EventHandler<Button> Notify; 
        public override void Publish(Button eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}