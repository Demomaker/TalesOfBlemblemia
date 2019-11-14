using Harmony;
using UnityEngine.UI;

namespace Game
{
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class OnButtonClick : EventChannel<Button>
    {
        public event EventHandler<Button> Notify; 
        public override void Publish(Button eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}