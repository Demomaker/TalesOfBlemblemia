using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private string overworldName;
    [SerializeField] private string levelName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Finder.GameController.LevelsCompleted.Add(levelName);
            Finder.GameController.LoadLevel(overworldName);
        }
    }

    //To be used in levels, needs to load overworld on deload
}
