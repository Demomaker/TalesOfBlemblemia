using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    //Author: Jérémie Bertrand
    public class Cinematic : MonoBehaviour
    {
        [SerializeField] private CinematicTriggerType trigger = CinematicTriggerType.Manual;
        [SerializeField] private List<CinematicAction> actions;
        
        private OnLevelVictory onLevelVictory;
        public IEnumerable<CinematicAction> Actions => actions;

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

        public enum CinematicTriggerType
        {
            Manual,
            OnStart,
            OnLevelVictory,
            OnLevelFailed
        }
    }
}