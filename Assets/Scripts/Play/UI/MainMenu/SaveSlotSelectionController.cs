using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Author: Antoine Lessard
namespace Game
{
    public class SaveSlotSelectionController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button[] saveSlots;
        [Header("Canvas")] 
        [SerializeField] private NewGameMenuController newGameScreen;

        private Navigator navigator;
        private SaveController saveController;
        private GameSettings gameSettings;

        private Canvas saveSelectionScreen;

        private void Awake()
        {
            navigator = Harmony.Finder.Navigator;
            saveController = Harmony.Finder.SaveController;
            saveSelectionScreen = GetComponent<Canvas>();
            gameSettings = Harmony.Finder.GameSettings;
        }

        public void Enter()
        {
            navigator.Enter(saveSelectionScreen);
        }

        private void Start()
        {
            SetupSaveValues();
        }

        private void SetupSaveValues()
        {
            var saveCounter = 0;

            var saves = saveController.GetSaves();

            foreach (var saveSlot in saveSlots)
            {
                saveSlot.transform.Find(gameSettings.NameString).GetComponent<TMP_Text>().text = saves[saveCounter].Username;
                saveSlot.transform.Find(gameSettings.StageString).GetComponent<TMP_Text>().text = saves[saveCounter].LevelName;
                saveSlot.transform.Find(gameSettings.DifficultyString).GetComponent<TMP_Text>().text =
                    saves[saveCounter].DifficultyLevel;

                ++saveCounter;
            }
        }

        [UsedImplicitly]
        public void SaveSlotSelected(int saveSlotId)
        {
            newGameScreen.Enter(saveSlotId);
        }

        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            navigator.Leave();
        }
    }
}