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
        [SerializeField] private Button[] saveSlots;

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
            int saveCounter = 0;

            SaveInfos[] saves = saveController.GetSaves();
            
            foreach (var saveSlot in saveSlots)
            {
                saveSlot.transform.Find(Constants.NAME_STRING).GetComponent<TMP_Text>().text = saves[saveCounter].Username;
                saveSlot.transform.Find(Constants.STAGE_STRING).GetComponent<TMP_Text>().text = saves[saveCounter].LevelName;
                saveSlot.transform.Find(Constants.DIFFICULTY_STRING).GetComponent<TMP_Text>().text =
                    saves[saveCounter].DifficultyLevel;
                
                ++saveCounter;
            }
        }

        [UsedImplicitly]
        public void SaveSlotSelected(int saveSlotId)
        {
            newGameScreen.Enter(saveSlotId);
        }
    }
}