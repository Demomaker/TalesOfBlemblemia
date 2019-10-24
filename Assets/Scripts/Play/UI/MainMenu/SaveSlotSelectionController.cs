using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class SaveSlotSelectionController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button saveSlot1;
        [SerializeField] private Button saveSlot2;
        [SerializeField] private Button saveSlot3;

        [Header("Canvas")] 
        [SerializeField] private NewGameMenuController newGameScreen;

        private Navigator navigator;
        private SaveController saveController;

        private Canvas saveSelectionScreen;

        private void Awake()
        {
            navigator = Finder.Navigator;
            saveController = Finder.SaveController;
            saveSelectionScreen = GetComponent<Canvas>();
        }

        public void Enter()
        {
            navigator.Enter(saveSelectionScreen);
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
            newGameScreen.Enter(Constants.SAVE_SLOT_ONE);
        }

        [UsedImplicitly]
        public void SaveSlot2Selected()
        {
            newGameScreen.Enter(Constants.SAVE_SLOT_TWO);
        }

        [UsedImplicitly]
        public void SaveSlot3Selected()
        {
            newGameScreen.Enter(Constants.SAVE_SLOT_THREE);
        }

    }
}