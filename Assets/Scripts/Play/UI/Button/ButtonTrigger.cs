using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public void OnClick()
    {
        Finder.SoundManager.PlaySingle(Finder.SoundClips.ButtonClickSound);
    }
}
