using UnityEngine;

namespace Game
{
    public class EndGameCreditsController : MonoBehaviour
    {
        public void RollCredits()
        {
            
            Harmony.Finder.LevelController.CinematicController.IsPlayingACinematic = true;
            gameObject.SetActive(true);
        }

        public bool CreditsAreRolling => isActiveAndEnabled;
    }
}
