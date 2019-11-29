using UnityEngine;

namespace Game
{
    //Author : Zacharie Lavigne
    public class EndGameCreditsController : MonoBehaviour
    {
        public bool CreditsAreRolling => isActiveAndEnabled;
        
        public void RollCredits()
        {
            Harmony.Finder.LevelController.CinematicController.IsPlayingACinematic = true;
            gameObject.SetActive(true);
        }

        public void StopCredits()
        {
            Harmony.Finder.LevelController.CinematicController.IsPlayingACinematic = false;
        }
    }
}
