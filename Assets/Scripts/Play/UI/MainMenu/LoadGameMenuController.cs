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
        [SerializeField] private Button saveSlot1;
        [SerializeField] private Button saveSlot2;
        [SerializeField] private Button saveSlot3;

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
            //BC : Constantes manquantes.
            //     Vous avez même pas d'excuse en plus, car je les génère automatiquement pour vous!!!
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

        public void Enter()
        {
            navigator.Enter(loadGameScreen);
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
            navigator.Leave();
        }

        private void LoadScenes(int saveSlotNumber, string sceneName)
        {
            saveController.SaveSelected = saveSlotNumber;
            SceneManager.LoadScene(sceneName);
            //BC : ???
            if (!SceneManager.GetSceneByName(Constants.GAME_UI_SCENE_NAME).isLoaded)
            {
                SceneManager.LoadScene(Constants.GAME_UI_SCENE_NAME, LoadSceneMode.Additive);
            }
        }
    }
}