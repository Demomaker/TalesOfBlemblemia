using System;
using System.Collections;
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
        private OnLevelDefeat onLevelDefeat;
        public IEnumerable<CinematicAction> Actions => actions;

        private void Awake()
        {
            InitializeEvents();
        }
        

        private void EnableEvents()
        {
            switch (trigger)
            {
                case CinematicTriggerType.OnLevelVictory:
                    onLevelVictory.Notify += TriggerCinematic;
                    break;
                case CinematicTriggerType.OnLevelFailed:
                    onLevelDefeat.Notify += TriggerCinematic;
                    break;
            }
        }

        private void InitializeEvents()
        {
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelDefeat = Harmony.Finder.OnLevelDefeat;
        }

        private void Start()
        {
            if(trigger == CinematicTriggerType.OnStart) TriggerCinematic();
        }

        private void OnEnable()
        {
            EnableEvents();
        }

        private void OnDisable()
        {
            DisableEvents();
        }

        private void DisableEvents()
        {
            switch (trigger)
            {
                case CinematicTriggerType.OnLevelVictory:
                    onLevelVictory.Notify -= TriggerCinematic;
                    break;
                case CinematicTriggerType.OnLevelFailed:
                    onLevelDefeat.Notify -= TriggerCinematic;
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