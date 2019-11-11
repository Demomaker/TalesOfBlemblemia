﻿using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Finder = Harmony.Finder;

public class LevelEntry : MonoBehaviour
{
    private List<string> previousLevelNames;
    private bool IsFirstLevel => representedLevelName == Harmony.Finder.GameSettings.StartingLevelName;
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
        if (CanBeClicked && !string.IsNullOrEmpty(representedLevelName))
        {
            EventSystem.current.SetSelectedGameObject(null);
            Finder.GameController.LoadLevel(representedLevelName);
        }
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
        if (PreviousLevelNames == null) return false;
        foreach (var t in PreviousLevelNames)
        {
            if (t == Finder.GameController.NameOfLevelCompleted)
            {
                return true;
            }
        }

        return false;
    }
}
