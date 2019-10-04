using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Finder = Harmony.Finder;

public class LevelController : MonoBehaviour
{
    [SerializeField] private string levelName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Finder.GameController.LevelsCompleted.Add(levelName);
            Finder.GameController.LoadLevel(Constants.OVERWORLD_SCENE_NAME);
        }
    }

    //To be used in levels, needs to load overworld on deload
}
