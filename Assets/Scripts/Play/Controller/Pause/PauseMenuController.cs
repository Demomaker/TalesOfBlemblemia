using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button returnToOverworldButton;

        private PauseController pauseController;

        private void Start()
        {
            pauseController = Finder.PauseController;
        }

        [UsedImplicitly]
        private void ResumeGame()
        {
            pauseController.Resume();
        }

        private void GoToOptionsMenu()
        {
            pauseController.GoToOptions();
        }
    }
}