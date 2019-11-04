using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;
using Finder = Harmony.Finder;

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
