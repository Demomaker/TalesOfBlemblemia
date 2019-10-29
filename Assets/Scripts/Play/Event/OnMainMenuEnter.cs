using Harmony;
using JetBrains.Annotations;

namespace Game
{
    public class OnMainMenuEnter : EventChannel<MainMenuController>
    {
        public static event EventHandler<MainMenuController> Notify;
        public override void Publish(MainMenuController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}