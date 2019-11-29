using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

//Author: Antoine Lessard
namespace Game
{
    public class CreditsMenuController : MonoBehaviour
    {
        private Navigator navigator;
        private Canvas creditsScreen;

        private void Awake()
        {
            navigator = Harmony.Finder.Navigator;
            creditsScreen = GetComponent<Canvas>();
        }

        public void Enter()
        {
            navigator.Enter(creditsScreen);
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            navigator.Leave();
        }
    }
}