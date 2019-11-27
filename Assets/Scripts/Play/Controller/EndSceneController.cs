using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// Controls the end scene of the game
/// Author : Mike Bédard
/// </summary>
public class EndSceneController : MonoBehaviour
{
    [SerializeField] private AudioClip endMusic;
    [SerializeField] private Vector3 cameraEndPosition;
    [SerializeField] private float tilemapChangeTimeInSeconds;
    [SerializeField] private int numberOfTilesOnScreen;
    [SerializeField] private Camera camera;
    
    private LevelLoader levelLoader;
    private OnEndLevelEnter onEndLevelEnter;
    private GameSettings gameSettings;
    private Vector3 cameraStartPosition;
    private bool sceneQuit = false;

    public AudioClip EndMusic => endMusic;

    private void Awake()
    {
        levelLoader = Harmony.Finder.LevelLoader;
        onEndLevelEnter = Harmony.Finder.OnEndLevelEnter;
        gameSettings = Harmony.Finder.GameSettings;
        onEndLevelEnter.Publish(this);
        cameraStartPosition = camera.transform.position;
        StartCoroutine(ChangeTilemaps());
    }
    
    private IEnumerator ChangeTilemaps()
    {
        while (!sceneQuit)
        {
            yield return new WaitForSeconds(tilemapChangeTimeInSeconds);
            yield return StartCoroutine(ChangeToNextTilemap());
        }
    }

    private IEnumerator ChangeToNextTilemap()
    {
        if (camera.transform.position.x < cameraEndPosition.x)
            camera.transform.position = cameraStartPosition;
        else
        {
            var cameraPositionXBeforeChange = camera.transform.position.x;
            while (camera.transform.position.x > cameraPositionXBeforeChange - numberOfTilesOnScreen)
            {
                yield return null;
                camera.transform.position = new Vector3(camera.transform.position.x - 0.1f, camera.transform.position.y, camera.transform.position.z);
            }
        }
    }
    
    [UsedImplicitly]
    public void ReturnToMainMenu()
    {
        sceneQuit = true;
        levelLoader.FadeToLevel(gameSettings.MainmenuSceneName, LoadSceneMode.Additive);
    }

}
