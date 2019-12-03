using System;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Author: Antoine Lessard
namespace Game
{
    public class LoadGameMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button[] saveSlots;
        
        private Navigator navigator;
        private SaveController saveController;
        private GameSettings gameSettings;
        private Canvas loadGameScreen;
        private LevelLoader levelLoader;
        private GameController gameController;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            navigator = Harmony.Finder.Navigator;
            saveController = Harmony.Finder.SaveController;
            loadGameScreen = GetComponent<Canvas>();
            levelLoader = Harmony.Finder.LevelLoader;
            gameController = Harmony.Finder.GameController;
        }
        
        private void Start()
        {
            var saveCounter = 0;

            var saves = saveController.GetSaves();

            foreach (var saveSlot in saveSlots)
            {
                saveSlot.transform.Find(gameSettings.NameString).GetComponent<TMP_Text>().text = saves[saveCounter].Username;
                saveSlot.transform.Find(gameSettings.DifficultyString).GetComponent<TMP_Text>().text =
                    saves[saveCounter].DifficultyLevel;
                saveSlot.transform.Find(gameSettings.StageString).GetComponent<TMP_Text>().text = saveSlot.name = saves[saveCounter].LevelName;
                if (SaveSlotSelectionController.SaveSlotIsEmpty(saveSlots, Array.IndexOf(saveSlots, saveSlot)))
                {
                    saveSlot.transform.Find(gameSettings.StageString).GetComponent<TMP_Text>().text = saveSlot.name =
                        gameSettings.EmptyLevelString;
                    saveSlot.enabled = false;
                    var tempColor = saveSlot.GetComponent<Image>().color;
                    tempColor = gameSettings.PaleAlpha;
                    saveSlot.GetComponent<Image>().color = tempColor;
                }

                ++saveCounter;
            }
        }

        public void Enter()
        {
            navigator.Enter(loadGameScreen);
        }

        [UsedImplicitly]
        public void SaveSlotSelected(int saveSlotId)
        {
            LoadGame(saveSlotId);

            levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
        }

        private void LoadGame(int saveSlotId)
        {
            saveSlotId = Mathf.Clamp(saveSlotId, 0, 2);
            saveController.SaveSelected = saveSlotId + 1;
            var difficultyLevel = saveSlots[saveSlotId].transform.Find(gameSettings.DifficultyString)
                .GetComponent<TMP_Text>().text;
            switch (difficultyLevel)
            {
                case "Easy":
                    gameController.DifficultyLevel = DifficultyLevel.Easy;
                    break;
                case "Medium":
                    gameController.DifficultyLevel = DifficultyLevel.Medium;
                    break;
                case "Hard":
                    gameController.DifficultyLevel = DifficultyLevel.Hard;
                    break;
                default:
                    gameController.DifficultyLevel = DifficultyLevel.Medium;
                    break;
            }

            gameController.PreviousLevelName = saveSlots[saveSlotId].transform.Find(gameSettings.StageString)
                .GetComponent<TMP_Text>().text;
        }
        


        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            navigator.Leave();
        }
    }
}