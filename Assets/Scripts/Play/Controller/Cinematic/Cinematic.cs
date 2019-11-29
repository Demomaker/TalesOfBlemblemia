﻿using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    //Author: Jérémie Bertrand, Mike Bédard
    public class Cinematic : MonoBehaviour
    {
        [SerializeField] private CinematicTriggerType trigger = CinematicTriggerType.Manual;
        [SerializeField] private List<CinematicAction> actions;

        private OnLevelVictory onLevelVictory;
        private OnLevelFailed onLevelFailed;
        private OnCampaignFailed onCampaignFailed;

        public IEnumerable<CinematicAction> Actions => actions;

        private void Awake()
        {
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelFailed = Harmony.Finder.OnLevelFailed;
            onCampaignFailed = Harmony.Finder.OnCampaignFailed;
        }
        
        private void Start()
        {
            if(trigger == CinematicTriggerType.OnStart) TriggerCinematic(Harmony.Finder.LevelController);
        }

        private void OnEnable()
        {
            EnableEvents();
        }

        private void OnDisable()
        {
            DisableEvents();
        }

        private void EnableEvents()
        {
            switch (trigger)
            {
                case CinematicTriggerType.OnLevelVictory:
                    onLevelVictory.Notify += TriggerCinematic;
                    break;
                case CinematicTriggerType.OnLevelFailed:
                    onLevelFailed.Notify += TriggerCinematic;
                    break;
                case CinematicTriggerType.OnCampaignFailed:
                    onCampaignFailed.Notify += TriggerCinematic;
                    break;
            }
        }

        private void DisableEvents()
        {
            switch (trigger)
            {
                case CinematicTriggerType.OnLevelVictory:
                    onLevelVictory.Notify -= TriggerCinematic;
                    break;
                case CinematicTriggerType.OnLevelFailed:
                    onLevelFailed.Notify -= TriggerCinematic;
                    break;
                case CinematicTriggerType.OnCampaignFailed:
                    onCampaignFailed.Notify -= TriggerCinematic;
                    break;
            }
        }

        public void TriggerCinematic(LevelController levelController)
        {
            levelController.CinematicController.LaunchCinematic(this);
        }
        
        public void TriggerCinematic()
        {
            TriggerCinematic(Harmony.Finder.LevelController);
        }
    }
}