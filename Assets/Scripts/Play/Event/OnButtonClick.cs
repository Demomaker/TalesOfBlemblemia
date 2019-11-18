using Harmony;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// OnButtonClick event channel
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class OnButtonClick : EventChannel<Button>
    {
        public event EventHandler<Button> Notify; 
        public override void Publish(Button eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}