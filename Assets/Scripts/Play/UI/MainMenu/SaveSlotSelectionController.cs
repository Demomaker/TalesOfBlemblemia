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

        [Header("Controls")] 
        [SerializeField] private KeyCode confirmKey = KeyCode.Mouse0;

        private MenusController menusController;
        private SaveController saveController;
        private int saveNumberSelected;

        private void Awake()
        {
            menusController = Finder.MenusController;
            saveController = Finder.SaveController;
            saveNumberSelected = 0;
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
            saveNumberSelected = 1;
            menusController.GoToNewGameMenu(saveNumberSelected);
        }

        [UsedImplicitly]
        public void SaveSlot2Selected()
        {
            saveNumberSelected = 2;
            menusController.GoToNewGameMenu(saveNumberSelected);
        }

        [UsedImplicitly]
        public void SaveSlot3Selected()
        {
            saveNumberSelected = 3;
            menusController.GoToNewGameMenu(saveNumberSelected);
        }

    }
}