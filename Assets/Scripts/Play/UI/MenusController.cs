using UnityEngine;

namespace Game
{
    public class MenusController : MonoBehaviour
    {
        [Header("Controllers")] 
        [SerializeField] private MainMenuController mainScreen;

        [Header("Canvas")] 
        [SerializeField] private Canvas newGameScreen;
        [SerializeField] private Canvas loadGameScreen;
        [SerializeField] private Canvas optionsScreen;
        [SerializeField] private Canvas creditsScreen;
        [SerializeField] private Canvas saveSlotSelectionScreen;

        private Navigator navigator;

        private void Awake()
        {
            navigator = Finder.Navigator;
        }

        private void Start()
        {
            newGameScreen.enabled = false;
            loadGameScreen.enabled = false;
            optionsScreen.enabled = false;
            creditsScreen.enabled = false;
            saveSlotSelectionScreen.enabled = false;
            mainScreen.Enter();
        }
    }
}