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
    #region Serialized Fields
    [SerializeField] private LevelID representedLevelId;
    #endregion Serialized Fields
    #region Other Fields
    private GameSettings gameSettings;
    private Button levelEntryButton;
    private List<string> previousLevelName;
    private OverWorldController overWorldController;
    private GameController gameController;
    private string representedLevelName;
    #endregion Other Fields
    #region Accessors
    private bool IsFirstLevel => gameSettings.StartingLevelSceneName == representedLevelName;
    private bool CanBeClicked => overWorldController.IsDebugging || IsFirstLevel && previousLevelName.Count <= 0 || PreviousLevelNameListContainsElement(gameController.PreviousLevelName);
    #endregion Accessors
    #region Unity Event Functions
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
    #endregion Unity Event Functions
    #region Level-entering-related Functions
    public void OnLevelEntry()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (CanBeClicked && overWorldController.CanLoadANewLevel && !overWorldController.CharacterIsMoving)
        {
            StartCoroutine(overWorldController.LoadLevel(representedLevelName, transform.position));
        }
    }
    
    private bool PreviousLevelNameListContainsElement(string previousLevel)
    {
        return previousLevelName.Any(levelName => levelName == previousLevel);
    }
    #endregion Level-entering-related Functions
}