using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Finder = Harmony.Finder;

[RequireComponent(typeof(Button))]
public class LevelEntry : MonoBehaviour
{
    [SerializeField] private LevelID.LevelIDValue representedLevelId;
    private GameSettings gameSettings;
    private Button levelEntryButton;
    private string previousLevelName;
    private OverWorldController overWorldController;
    private GameController gameController;
    private string representedLevelName;
    private bool IsFirstLevel => string.IsNullOrEmpty(gameController.PreviousLevelName);
    private bool CanBeClicked => overWorldController.IsDebugging || IsFirstLevel && string.IsNullOrEmpty(previousLevelName) || gameController.PreviousLevelName == previousLevelName;

    private void Awake()
    {
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
                previousLevelName = level.PreviousLevel;
                break;
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
        if (CanBeClicked && overWorldController.CanLoadANewLevel)
        {
            StartCoroutine(overWorldController.LoadLevel(representedLevelName, transform.position));
        }
    }
}