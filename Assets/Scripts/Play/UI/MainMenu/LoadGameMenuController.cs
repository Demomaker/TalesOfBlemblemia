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
        [SerializeField] private Button saveSlot1;
        [SerializeField] private Button saveSlot2;
        [SerializeField] private Button saveSlot3;

        private MenusController menusController;
        private SaveController saveController;

        private void Awake()
        {
            menusController = Finder.MenusController;
            saveController = Finder.SaveController;
        }
        
        private void Start()
        {
            saveSlot1.transform.Find("Name").GetComponent<TMP_Text>().text = saveController.saveSlot1.username;
            saveSlot1.transform.Find("Stage").GetComponent<TMP_Text>().text = saveController.saveSlot1.levelName;
            saveSlot1.transform.Find("Difficulty").GetComponent<TMP_Text>().text =
                saveController.saveSlot1.difficultyLevel;
            
            saveSlot2.transform.Find("Name").GetComponent<TMP_Text>().text = saveController.saveSlot2.username;
            saveSlot2.transform.Find("Stage").GetComponent<TMP_Text>().text = saveController.saveSlot2.levelName;
            saveSlot2.transform.Find("Difficulty").GetComponent<TMP_Text>().text =
                saveController.saveSlot2.difficultyLevel;
            
            saveSlot3.transform.Find("Name").GetComponent<TMP_Text>().text = saveController.saveSlot3.username;
            saveSlot3.transform.Find("Stage").GetComponent<TMP_Text>().text = saveController.saveSlot3.levelName;
            saveSlot3.transform.Find("Difficulty").GetComponent<TMP_Text>().text =
                saveController.saveSlot3.difficultyLevel;
        }

        [UsedImplicitly]
        public void SaveSlot1Selected()
        {
            LoadScenes(1, saveSlot1.name);
        }

        [UsedImplicitly]
        public void SaveSlot2Selected()
        {
            LoadScenes(2, saveSlot2.name);
        }

        [UsedImplicitly]
        public void SaveSlot3Selected()
        {
            LoadScenes(3, saveSlot3.name);
        }

        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            menusController.ReturnFromLoadGameMenu();
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