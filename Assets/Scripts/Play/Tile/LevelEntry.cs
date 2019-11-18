using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Finder = Harmony.Finder;

[RequireComponent(typeof(Button))]
public class LevelEntry : MonoBehaviour
{
    [SerializeField] private LevelName.LevelNameEnum representedLevelId;
    private GameSettings gameSettings;
    private Button levelEntryButton;
    private List<string> previousLevelName;
    private OverWorldController overWorldController;
    private GameController gameController;
    private string representedLevelName;
    private bool IsFirstLevel => gameSettings.StartingLevelSceneName == representedLevelName;
    private bool CanBeClicked => overWorldController.IsDebugging || IsFirstLevel && previousLevelName.Count <= 0 || PreviousLevelNameListContainsElement(gameController.PreviousLevelName);

    private bool PreviousLevelNameListContainsElement(string previousLevel)
    {
        return previousLevelName.Any(levelName => levelName == previousLevel);
    }


    private void Awake()
    {
        previousLevelName = new List<string>();
        gameSettings = Finder.GameSettings;
        representedLevelName = representedLevelId.GetLevelNameFromLevelID();
        overWorldController = Finder.OverWorldController;
        gameController = Finder.GameController;
        levelEntryButton = GetComponent<Button>();
        if(levelEntryButton == null) Debug.LogError(name + ": Button is null!");
        foreach (var level in gameController.Levels)
        {
            if (level.LevelName == representedLevelName)
            {
                previousLevelName.Add(level.PreviousLevel);
            }
        }
    }
    
    private void Start()
    {
        var colors = levelEntryButton.colors;
        colors.highlightedColor = !CanBeClicked ? Color.red : Color.green;
        levelEntryButton.colors = colors;
        if (representedLevelName == gameController.PreviousLevelName)
        {
            var target = transform.position;
            overWorldController.CharacterTransform.position = new Vector3(target.x + 1, target.y, target.z);
        }
    }

    public void OnLevelEntry()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (CanBeClicked && overWorldController.CanLoadANewLevel && !overWorldController.CharacterIsMoving)
        {
            StartCoroutine(overWorldController.LoadLevel(representedLevelName, transform.position));
        }
    }
}