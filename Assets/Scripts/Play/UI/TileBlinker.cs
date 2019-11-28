using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

/// <summary>
/// Manages tile blinking (made especially for usage with a Pointing Arrow)
/// Author : Mike Bédard
/// </summary>
public class TileBlinker : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Tile tileToBlink;
    [SerializeField] private Sprite blinkSprite;
    #endregion
    #region Other Fields
    private Coroutine blinkCoroutine;
    #endregion Other Fields
    #region Unity Event Functions
    private void Start()
    {
        if(tileToBlink != null)
        blinkCoroutine = Harmony.Finder.CoroutineStarter.StartCoroutine(tileToBlink.Blink(blinkSprite));
    }

    private void OnDisable()
    {
        if(blinkCoroutine != null)
        StopCoroutine(blinkCoroutine);
        if(tileToBlink != null)
        tileToBlink.ResetTileImage();
    }
    #endregion Unity Event Functions
}
