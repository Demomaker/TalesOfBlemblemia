using Harmony;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            loadGameScreen = GetComponent<Canvas>();
            levelLoader = Harmony.Finder.LevelLoader;

        }
        
        private void Start()
        {
            int saveCounter = 0;

            SaveInfos[] saves = saveController.GetSaves();
            
            foreach (var saveSlot in saveSlots)
            {
                saveSlot.transform.Find(gameSettings.NameString).GetComponent<TMP_Text>().text = saves[saveCounter].Username;
                saveSlot.transform.Find(gameSettings.StageString).GetComponent<TMP_Text>().text = saves[saveCounter].LevelName;
                saveSlot.transform.Find(gameSettings.DifficultyString).GetComponent<TMP_Text>().text =
                    saves[saveCounter].DifficultyLevel;
                
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
            saveSlotId = Mathf.Clamp(saveSlotId, 0, 2);
            LoadScenes(saveSlotId + 1, saveSlots[saveSlotId].name);
        }

        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            navigator.Leave();
        }

        private void LoadScenes(int saveSlotNumber, string sceneName)
        {
            saveController.SaveSelected = saveSlotNumber;
            levelLoader.FadeToLevel(sceneName, LoadSceneMode.Additive);
        }
    }
}