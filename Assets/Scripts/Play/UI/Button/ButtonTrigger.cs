using Game;
using UnityEngine;
using UnityEngine.UI;
using Finder = Harmony.Finder;

/// <summary>
/// Trigger for button presses
/// Author : Mike Bédard
/// </summary>
public class ButtonTrigger : MonoBehaviour
{
    public static OnButtonClick onButtonClick = null;

    private void Awake()
    {
        if (onButtonClick == null)
            onButtonClick = Finder.OnButtonClick;
    }

    public void OnClick()
    {
        onButtonClick.Publish(GetComponent<Button>());
    }
}
