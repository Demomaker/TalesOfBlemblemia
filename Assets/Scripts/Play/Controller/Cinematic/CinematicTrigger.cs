﻿using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // Authors: Jérémie Bertrand
    public class CinematicTrigger : MonoBehaviour
    {
        [SerializeField] private CinematicTriggerType trigger = CinematicTriggerType.Manual;
        [SerializeField] private Cinematic cinematic;

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
            Harmony.Finder.LevelController.CinematicController.LaunchCinematic(cinematic);
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