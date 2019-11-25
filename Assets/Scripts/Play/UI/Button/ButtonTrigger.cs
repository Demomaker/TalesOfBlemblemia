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
    #region Fields
    public static OnButtonClick onButtonClick = null;
    #endregion Fields
    #region Unity Event Functions
    private void Awake()
    {
        if (onButtonClick == null)
            onButtonClick = Finder.OnButtonClick;
    }

    public void OnClick()
    {
        onButtonClick.Publish(GetComponent<Button>());
    }
    #endregion Unity Event Functions
}
