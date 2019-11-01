using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTrigger : MonoBehaviour
{
    public static OnButtonClick onButtonClick = null;

    private void Awake()
    {
        if (onButtonClick == null)
            onButtonClick = new OnButtonClick();
    }

    public void OnClick()
    {
        onButtonClick.Publish(GetComponent<Button>());
    }
}
