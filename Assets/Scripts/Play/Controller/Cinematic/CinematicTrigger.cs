using Harmony;
using UnityEngine;

namespace Play
{
    public class CinematicTrigger : MonoBehaviour
    {
        [SerializeField] private CameraAction[] cameraActions;

        public void TriggerDialogue()
        {
            Finder.LevelController.CinematicController.LaunchCinematic(cameraActions);
        }
    }
}