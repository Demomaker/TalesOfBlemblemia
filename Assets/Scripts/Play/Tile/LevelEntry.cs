using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Finder = Harmony.Finder;

[RequireComponent(typeof(Button))]
public class LevelEntry : MonoBehaviour
{
    [SerializeField] private string representedLevelName;
    private Button levelEntryButton;
    private string previousLevelName;
    private OverWorldController overWorldController;
    private GameController gameController;
    private bool IsFirstLevel => string.IsNullOrEmpty(Finder.GameController.PreviousLevelName);
    private bool CanBeClicked => Finder.OverWorldController.IsDebugging || IsFirstLevel && string.IsNullOrEmpty(previousLevelName) || Finder.GameController.PreviousLevelName == previousLevelName;
    public string RepresentedLevelName => representedLevelName;
    private void Awake()
    {
        overWorldController = Finder.OverWorldController;
        gameController = Finder.GameController;
        levelEntryButton = GetComponent<Button>();
        if(levelEntryButton == null) Debug.LogError(name + ": Button is null!");
        for (int i = 0; i < gameController.Levels.Length; i++)
        {
            if (Finder.GameController.Levels[i].LevelName == representedLevelName)
            {
                previousLevelName = gameController.Levels[i].PreviousLevel;
                break;
            }
        }
    }
    
    private void Start()
    {
        var colors = levelEntryButton.colors;
        colors.highlightedColor = !CanBeClicked ? Color.red : Color.green;
        levelEntryButton.colors = colors;
        if (RepresentedLevelName == gameController.PreviousLevelName)
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