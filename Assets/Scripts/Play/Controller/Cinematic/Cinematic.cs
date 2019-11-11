using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Cinematic : MonoBehaviour
    {
        [SerializeField] private CinematicTriggerType trigger = CinematicTriggerType.Manual;
        private OnLevelVictory onLevelVictory;
        
        [SerializeField] private List<CameraAction> cameraActions;
        public IEnumerable<CameraAction> CameraActions => cameraActions;

        private void Awake()
        {
            onLevelVictory = Harmony.Finder.OnLevelVictory;
        }

        private void Start()
        {
            if(trigger == CinematicTriggerType.OnStart) TriggerCinematic();
        }

        private void OnEnable()
        {
            switch (trigger)
            {
                case CinematicTriggerType.OnLevelVictory:
                    onLevelVictory.Notify += TriggerCinematic;
                    break;
            }
        }

        private void OnDisable()
        {
            switch (trigger)
            {
                case CinematicTriggerType.OnLevelVictory:
                    onLevelVictory.Notify -= TriggerCinematic;
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