using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
public class EndSceneController : LevelController
{
    [SerializeField] private Vector3 cameraEndPosition;
    [SerializeField] private float tilemapChangeTimeInSeconds;
    [SerializeField] private int numberOfTilesOnScreen;
    [SerializeField] private Camera camera;
    private Vector3 cameraStartPosition;
    private bool sceneQuit;
    private Cinematic cinematic;

    protected override void Awake()
    {
        levelLoader = Harmony.Finder.LevelLoader;
        onLevelChange = Harmony.Finder.OnLevelChange;
        gameSettings = Harmony.Finder.GameSettings;
        cameraStartPosition = camera.transform.position;
        cinematic = GetComponent<Cinematic>();
        cinematicController = GetComponent<CinematicController>();
        onLevelChange.Publish(this);
    }

    protected override void Start()
    {
        cinematic.TriggerCinematic();
        StartCoroutine(ChangeTilemaps());
    }

    protected override void Update()
    {
        //Do Nothing On Purpose 
    }

    protected override void OnEnable()
    {
        //Do Nothing On Purpose 
    }

    protected override void OnDisable()
    {
        //Do Nothing On Purpose   
    }
    
    private IEnumerator ChangeTilemaps()
    {
        while (CinematicController.IsPlayingACinematic) yield return null;
        while (!sceneQuit)
        {
            yield return new WaitForSeconds(tilemapChangeTimeInSeconds);
            yield return StartCoroutine(ChangeToNextTilemap());
        }
    }

    private IEnumerator ChangeToNextTilemap()
    {
        if (camera.transform.position.x > cameraEndPosition.x)
            camera.transform.position = cameraStartPosition;
        else
        {
            var cameraPositionXBeforeChange = camera.transform.position.x;
            while (camera.transform.position.x < cameraPositionXBeforeChange + numberOfTilesOnScreen)
            {
                yield return null;
                camera.transform.position = new Vector3(camera.transform.position.x + 0.1f, camera.transform.position.y, camera.transform.position.z);
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
