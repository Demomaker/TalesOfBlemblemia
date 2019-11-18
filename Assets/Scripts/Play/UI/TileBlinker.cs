using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class TileBlinker : MonoBehaviour
{
    [SerializeField] private Tile tileToBlink;
    [SerializeField] private Sprite blinkSprite;
    private Coroutine blinkCoroutine;
    // Update is called once per frame
    private void Start()
    {
        blinkCoroutine = StartCoroutine(tileToBlink.Blink(blinkSprite));
    }

    private void OnDisable()
    {
        StopCoroutine(blinkCoroutine);
        if(tileToBlink != null)
        tileToBlink.ResetTileImage();
    }
}
