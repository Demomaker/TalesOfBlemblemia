using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Harmony;
using UnityEngine;
using Finder = Harmony.Finder;

namespace Game
{
    /// <summary>
    /// Controller for each individual level. Manages the turn functionalities as well as the players' turns
    /// Authors: Mike Bédard, Jérémie Bertrand, Zacharie Lavigne
    /// </summary>
    [Findable("LevelController")]
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private string levelName;
        [SerializeField] private GameObject dialogueUi;

        private Unit[] units = null;
        
        private UnitOwner currentPlayer;
        private readonly List<UnitOwner> players = new List<UnitOwner>();
        [SerializeField] private bool revertWeaponTriangle = false;
        public bool RevertWeaponTriangle => revertWeaponTriangle;

        private void Start()
        {
            players.Clear();
            InitializePlayersAndUnits();
            currentPlayer = players[0];
            players[0].OnTurnGiven();
            dialogueUi.SetActive(true);
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Finder.GameController.LevelsCompleted.Add(levelName);
                Finder.GameController.LoadLevel(Constants.OVERWORLD_SCENE_NAME);
            }

            if (currentPlayer == null) throw new NullReferenceException("Current player is null!");
            
            //TODO enlever ca quand le joueur pourra se reposer
            CheckForComputerTurnSkip();
            CheckForPlayerTurnSkip();

            CheckForCurrentPlayerWin();
            CheckForCurrentPlayerLoss();
            CheckForCurrentPlayerEndOfTurn();
            Play(currentPlayer);
        }

        private bool isComputerPlaying;

        private void Play(UnitOwner unitOwner)
        {
            unitOwner.CheckUnitDeaths();
            if (!isComputerPlaying && unitOwner is ComputerPlayer)
            {
                isComputerPlaying = true;
                var currentComputerPlayer = unitOwner as ComputerPlayer;
                StartCoroutine(currentComputerPlayer.PlayUnits());
            }
        }

        private void CheckForPlayerTurnSkip()
        {
            if (Input.GetKeyDown(Constants.SKIP_COMPUTER_TURN_KEY) && currentPlayer is HumanPlayer)
            {
                isComputerPlaying = false;
                currentPlayer = players.Find(player => player is ComputerPlayer);
                currentPlayer.OnTurnGiven();
            }
        }

        public void CheckForComputerTurnSkip()
        {
            if (Input.GetKeyDown(Constants.SKIP_COMPUTER_TURN_KEY) && currentPlayer is ComputerPlayer)
            {
                isComputerPlaying = false;
                currentPlayer = players.Find(player => player is HumanPlayer);
                currentPlayer.OnTurnGiven();
            }
        }
        private void InitializePlayersAndUnits()
        {
            UnitOwner player1 = HumanPlayer.Instance;
            UnitOwner player2 = ComputerPlayer.Instance;

            units = FindObjectsOfType<Unit>();

            GiveUnits(units, false, player1);
            GiveUnits(units, true, player2);

            player1.OnNewLevel();
            player2.OnNewLevel();
            
            players.Add(player1);
            players.Add(player2);
        }

        private void GiveUnits(Unit[] units, bool isEnemy, UnitOwner unitOwner)
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].IsEnemy == isEnemy)
                    unitOwner.AddOwnedUnit(units[i]);
                else
                    unitOwner.AddEnemyUnit(units[i]);
            }
        }

        public void CheckForCurrentPlayerEndOfTurn()
        {
            if (currentPlayer.HasNoMorePlayableUnits)
            {
                GiveTurnToNextPlayer();
                currentPlayer.OnTurnGiven();
            }
        }

        public void CheckForCurrentPlayerWin()
        {
            if (HasWon(currentPlayer))
            {
                currentPlayer.Win();
            }
        }

        public void CheckForCurrentPlayerLoss()
        {
            if (currentPlayer.HaveAllUnitsDied())
            {
                currentPlayer.Lose();
                UnitOwner playerWhoLost = currentPlayer;
                GiveTurnToNextPlayer();
            }
        }

        public bool HasWon(UnitOwner unitOwner)
        {
            return players.Contains(unitOwner) && players.Count <= 1;
        }

        public void GiveTurnToNextPlayer()
        {
            Harmony.Finder.LevelController.ReevaluateAllMovementCosts();
            isComputerPlaying = false;
            currentPlayer.MakeOwnedUnitsUnplayable();
            int nextPlayerIndex = (players.IndexOf(currentPlayer) + 1) % 2;
        
            if (players.ElementAt(nextPlayerIndex) != null)
                currentPlayer = players.ElementAt(nextPlayerIndex);
        }

        public void ReevaluateAllMovementCosts()
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].ComputeTilesCosts();
            }
        }
        
    }
}
