using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Game
{
    //Author: Jérémie Bertrand, Mike Bédard
    [RequireComponent(typeof(Button))]
    public class LevelEntry : MonoBehaviour
    {
        [SerializeField] private LevelID representedLevelId;
        
        private readonly List<string> previousLevelName = new List<string>();
        
        private GameSettings gameSettings;
        private Button levelEntryButton;
        private OverWorldController overWorldController;
        private GameController gameController;

        private bool IsFirstLevel => gameSettings.StartingLevelSceneName == RepresentedLevelName;
        private bool CanBeClicked => overWorldController.IsDebugging || IsFirstLevel && previousLevelName.Count <= 0 || PreviousLevelNameListContainsElement(gameController.PreviousLevelName);
        private string RepresentedLevelName => representedLevelId.GetLevelNameFromLevelID();

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            overWorldController = Harmony.Finder.OverWorldController;
            gameController = Harmony.Finder.GameController;
            levelEntryButton = GetComponent<Button>();

            foreach (var level in gameController.Levels)
            {
                if (level.LevelName == RepresentedLevelName)
                {
                    previousLevelName.Add(level.PreviousLevel);
                }
            }
        }
        
        private void Start()
        {
            var colors = levelEntryButton.colors;
            colors.highlightedColor = !CanBeClicked ? gameSettings.Red : gameSettings.Green;
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
            if (CanBeClicked && overWorldController.CanLoadANewLevel && !overWorldController.CharacterIsMoving)
            {
                Harmony.Finder.CoroutineStarter.StartCoroutine(overWorldController.LoadLevel(RepresentedLevelName, transform.position));
            }
        }
        
        private bool PreviousLevelNameListContainsElement(string previousLevel)
        {
            return previousLevelName.Any(levelName => levelName == previousLevel);
        }
    }
}