using System;
using System.Collections;
using System.Collections.Generic;
using Game;
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

        #region Unity Event Functions
        private void Awake()
        {
            InitializeEvents();
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
        #endregion
        #region Event Channel Handling
        
        private void InitializeEvents()
        {
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelFailed = Harmony.Finder.OnLevelFailed;
            onCampaignFailed = Harmony.Finder.OnCampaignFailed;
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
        #endregion
        #region Cinematic Methods
        public void TriggerCinematic(LevelController levelController = null)
        {
            Harmony.Finder.LevelController.CinematicController.LaunchCinematic(this);
        }
        #endregion
    }
}