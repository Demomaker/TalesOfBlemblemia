using Harmony;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Game
{
    public class LoadGameMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button[] saveSlots;

        private Navigator navigator;
        private SaveController saveController;
        private Canvas loadGameScreen;

        private void Awake()
        {
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            loadGameScreen = GetComponent<Canvas>();
        }
        
        private void Start()
        {
            int saveCounter = 0;

            SaveInfos[] saves = saveController.GetSaves();
            
            foreach (var saveSlot in saveSlots)
            {
                saveSlot.transform.Find(Constants.NAME_STRING).GetComponent<TMP_Text>().text = saves[saveCounter].username;
                saveSlot.transform.Find(Constants.STAGE_STRING).GetComponent<TMP_Text>().text = saves[saveCounter].levelName;
                saveSlot.transform.Find(Constants.DIFFICULTY_STRING).GetComponent<TMP_Text>().text =
                    saves[saveCounter].difficultyLevel;
                
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
            SceneManager.LoadScene(sceneName);
            if (!SceneManager.GetSceneByName(Constants.GAME_UI_SCENE_NAME).isLoaded)
            {
                SceneManager.LoadScene(Constants.GAME_UI_SCENE_NAME, LoadSceneMode.Additive);
            }
        }
    }
}