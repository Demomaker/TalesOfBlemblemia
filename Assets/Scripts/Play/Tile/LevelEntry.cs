using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEntry : MonoBehaviour
{
    [SerializeField] private string representedLevelName;

    public void OnLevelEntry()
    {
        Finder.GameController.LoadLevel(representedLevelName);
    }
}
