﻿using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Finder = Harmony.Finder;

public class LevelEntry : MonoBehaviour
{
    private List<string> previousLevelNames;
    private bool IsFirstLevel => representedLevelName == Finder.GameController.StartingLevelName;
    private bool CanBeClicked => IsFirstLevel && Finder.GameController.NameOfLevelCompleted == null || PreviousLevelWasCompleted();
    [SerializeField] string representedLevelName;
    public List<string> PreviousLevelNames
    {
        get
        {
            List<string> retval = new List<string>();
            List<Level> currentLevels = Finder.GameController.Levels.FindAll(level => level.LevelName == representedLevelName);
            if (currentLevels != null)
            {
                for (int i = 0; i < currentLevels.Count; i++)
                {
                    if (currentLevels[i] != null)
                        retval.Add(currentLevels[i].PreviousLevel);
                }
                return retval;
            }

            return null;
        }
    }
    public void OnLevelEntry()
    {
        if(CanBeClicked && !string.IsNullOrEmpty(representedLevelName))
        Finder.GameController.LoadLevel(representedLevelName);
    }

    private void Update()
    {
        var colors = GetComponent<Button>().colors;
        if (!CanBeClicked || string.IsNullOrEmpty(representedLevelName))
        {
            colors.pressedColor = Color.red;
        }
        else
        {
            colors.pressedColor = Color.green;
        }
        GetComponent<Button>().colors = colors;
    }

    private bool PreviousLevelWasCompleted()
    {
        if(PreviousLevelNames != null)
        for (int i = 0; i < PreviousLevelNames.Count; i++)
        {
            if (PreviousLevelNames[i] == Finder.GameController.NameOfLevelCompleted)
            {
                return true;
            }
        }

        return false;
    }
}
