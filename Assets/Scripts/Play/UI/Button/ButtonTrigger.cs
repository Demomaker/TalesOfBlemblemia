using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Trigger for button presses
    /// Author : Mike Bédard
    /// </summary>
    public class ButtonTrigger : MonoBehaviour
    {
        private static OnButtonClick onButtonClick = null;

        private void Awake()
        {
            if (onButtonClick == null)
                onButtonClick = Harmony.Finder.OnButtonClick;
        }

        public void OnClick()
        {
            onButtonClick.Publish(GetComponent<Button>());
        }
    }
}