using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Finder = Harmony.Finder;

[RequireComponent(typeof(Button))]
public class LevelEntry : MonoBehaviour
{
    [SerializeField] private string representedLevelName;
    private Button levelEntryButton = null;
    private bool isFirstLevel => representedLevelName == Finder.GameController.StartingLevelName;
    private bool canBeClicked => (isFirstLevel && Finder.GameController.NameOfLevelCompleted == null || PreviousLevelWasCompleted()) && !string.IsNullOrEmpty(representedLevelName);
    
    private void Awake()
    {
        levelEntryButton = GetComponent<Button>();
        if(levelEntryButton == null) UnityEngine.Debug.LogError(name + ": Button is null!");
    }
    
    private void Start()
    {
        var colors = levelEntryButton.colors;
        colors.highlightedColor = !canBeClicked ? Color.red : Color.green;
        levelEntryButton.colors = colors;
    }

    private List<string> GetPreviousLevelNames()
    {
        var currentLevels = Finder.GameController.Levels.FindAll(level => level.LevelName == representedLevelName);
        if (currentLevels == null) return null;
        var returnValue = new List<string>();
        currentLevels.ForEach(currentLevel => returnValue.Add(currentLevel.PreviousLevel));
        return returnValue;
    }
    
    public void OnLevelEntry()
    {
        if (canBeClicked)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Finder.GameController.LoadLevel(representedLevelName);
        }
    }
    
    private bool PreviousLevelWasCompleted()
    {
        var previousLevelNames = GetPreviousLevelNames();
        return previousLevelNames != null && previousLevelNames.Any(previousLevelName => previousLevelName == Finder.GameController.NameOfLevelCompleted);
    }
}
