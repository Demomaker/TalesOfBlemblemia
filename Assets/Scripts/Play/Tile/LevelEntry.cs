using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEntry : MonoBehaviour
{
    public string RepresentedLevelName;
    public string PreviousLevelName;
    public bool PreviousLevelCompleted =>  RepresentedLevelName == PreviousLevelName || Finder.GameController.LevelsCompleted.Contains(PreviousLevelName);
    private bool CanBeClicked => PreviousLevelCompleted;

    public void OnLevelEntry()
    {
        if(CanBeClicked && !string.IsNullOrEmpty(RepresentedLevelName))
        Finder.GameController.LoadLevel(RepresentedLevelName);
    }
}
