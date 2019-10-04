using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public static class UIExtensions
    {
        public static Button SelectedButton => EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();

        public static void Click(this Button button)
        {
            button.OnSubmit(new BaseEventData(EventSystem.current));
        }
    }
}