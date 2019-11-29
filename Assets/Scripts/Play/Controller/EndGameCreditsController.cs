using UnityEngine;

namespace Game
{
    //Author : Zacharie Lavigne
    public class EndGameCreditsController : MonoBehaviour
    {
        private LevelController levelController;
        public bool CreditsAreRolling => isActiveAndEnabled;
        
        private void Awake()
        {
            levelController = Harmony.Finder.LevelController;
        }

        public void RollCredits()
        {
            levelController.CinematicController.IsPlayingACinematic = true;
            gameObject.SetActive(true);
        }

        public void StopCredits()
        {
            levelController.CinematicController.IsPlayingACinematic = false;
        }
    }
}
