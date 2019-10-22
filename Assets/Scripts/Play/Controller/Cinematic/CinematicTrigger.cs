using UnityEngine;

namespace Game
{
    public class CinematicTrigger : MonoBehaviour
    {
        [SerializeField] private CameraAction[] cameraActions;

        public void TriggerDialogue()
        {
            Harmony.Finder.LevelController.CinematicController.LaunchCinematic(cameraActions);
        }
    }
}