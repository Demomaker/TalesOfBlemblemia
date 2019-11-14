using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // Authors: Jérémie Bertrand
    public class Cinematic : MonoBehaviour
    {
        [SerializeField] private CinematicTriggerType trigger = CinematicTriggerType.Manual;
        //BR : Aurait pu être un ScriptableObject.
        //     Il existe aussi des trucs dans Unity pour "officiellement" faire des cinématiques.
        //     Voir : https://unity3d.com/fr/how-to/cutscenes-cinematics-with-timeline-and-cinemachine
        [SerializeField] private List<CameraAction> cameraActions;
        public IEnumerable<CameraAction> CameraActions => cameraActions;

        private void Start()
        {
            if(trigger == CinematicTriggerType.OnStart) TriggerCinematic();
        }

        private void OnEnable()
        {
            switch (trigger)
            {
                case CinematicTriggerType.OnLevelVictory:
                    OnLevelVictory.Notify += TriggerCinematic;
                    break;
            }
        }

        private void OnDisable()
        {
            switch (trigger)
            {
                case CinematicTriggerType.OnLevelVictory:
                    OnLevelVictory.Notify -= TriggerCinematic;
                    break;
            }
        }

        public void TriggerCinematic(LevelController levelController = null)
        {
            Harmony.Finder.LevelController.CinematicController.LaunchCinematic(this);
        }

        enum CinematicTriggerType
        {
            Manual,
            OnStart,
            OnLevelVictory,
            OnLevelFailed
        }
    }
}