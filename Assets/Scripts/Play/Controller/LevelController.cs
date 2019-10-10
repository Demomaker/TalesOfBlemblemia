using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Linq;
using Game;
using Harmony;
using UnityEngine;
using UnityEngine.UIElements;
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
        [SerializeField] private GameObject dialogueUi = null;
        [SerializeField] private DialogueTrigger dialogueTriggerStartFranklem = null;
        [SerializeField] private bool completeIfAllEnemiesDefeated = false;
        [SerializeField] private bool completeIfPointAchieved = false;
        [SerializeField] private bool completeIfSurvivedCertainNumberOfTurns = false;
        [SerializeField] private bool completeIfCertainEnemyDefeated = false;
        [SerializeField] private bool defeatIfNotCompleteLevelInCertainAmountOfTurns = false;
        [SerializeField] private bool defeatIfProtectedIsKilled = false;
        [SerializeField] private bool defeatIfAllPlayerUnitsDied = false;
        [SerializeField] private Vector2Int pointToAchieve = new Vector2Int();
        [SerializeField] private Unit enemyToDefeat = null;
        [SerializeField] private Unit unitToProtect = null;
        [SerializeField] private int numberOfTurnsBeforeDefeat = 0;
        [SerializeField] private int numberOfTurnsBeforeCompletion = 0;
        [SerializeField] private bool revertWeaponTriangle = false;

        private bool levelCompleted = false;
        private bool levelFailed = false;
        private bool levelEnded = false;
        private bool isComputerPlaying;
        
        private Unit[] units = null;
        private UnitOwner currentPlayer;
        private readonly List<UnitOwner> players = new List<UnitOwner>();
        private int numberOfPlayerTurns = 0;
        public bool RevertWeaponTriangle => revertWeaponTriangle;

        private void Start()
        {
            players.Clear();
            InitializePlayersAndUnits();
            currentPlayer = players[0];
            OnTurnGiven();

            if (dialogueUi != null) 
            dialogueUi.SetActive(true);
            dialogueTriggerStartFranklem.TriggerDialogue();
        }

        protected void Update()
        {
            CheckIfLevelEnded();
            
            if (Input.GetKeyDown(KeyCode.O))
            {
                levelCompleted = true;
                levelEnded = true;
            }

            if (levelEnded)
            {
                if (levelCompleted)
                    Finder.GameController.LevelsCompleted.Add(levelName);
                Finder.GameController.LoadLevel(Constants.OVERWORLD_SCENE_NAME);
            }

            if (currentPlayer == null) throw new NullReferenceException("Current player is null!");
            
            //TODO enlever ca avant la release
            CheckForComputerTurnSkip();
            CheckForPlayerTurnSkip();

            CheckForCurrentPlayerWin();
            CheckForCurrentPlayerLoss();
            CheckForCurrentPlayerEndOfTurn();
            Play(currentPlayer);
        }

        private void OnTurnGiven()
        {
            if(currentPlayer is HumanPlayer) numberOfPlayerTurns++;
            currentPlayer.OnTurnGiven();
        }

        private void CheckIfLevelEnded()
        {
            CheckIfLevelCompleted();
            CheckIfLevelFailed();
            if (levelFailed || levelCompleted) levelEnded = true;
        }

        private void CheckIfLevelCompleted()
        {
            bool firstConditionAchieved = true;
            bool secondConditionAchieved = true;
            bool thirdConditionAchieved = true;
            bool fourthConditionAchieved = true;
            if (completeIfAllEnemiesDefeated)
            {
                //TODO: Uncomment below when Turns are available
                if(!ComputerPlayer.Instance.HaveAllUnitsDied()) firstConditionAchieved = false;
            }
            if (completeIfPointAchieved)
            {
                if ((GameObject.Find("Franklem") == null) || (GameObject.Find("Franklem").GetComponent<Unit>() == null) || (GameObject.Find("Franklem").GetComponent<Unit>().CurrentTile == null) || !(GameObject.Find("Franklem").GetComponent<Unit>().CurrentTile.LogicalPosition == pointToAchieve)) secondConditionAchieved = false;
            }
            if (completeIfCertainEnemyDefeated)
            {
                if (!(enemyToDefeat == null || enemyToDefeat.NoHealthLeft)) thirdConditionAchieved = false;
            }
            if (completeIfSurvivedCertainNumberOfTurns)
            {
                if (numberOfPlayerTurns < numberOfTurnsBeforeCompletion) fourthConditionAchieved = false;
            }

            levelCompleted = firstConditionAchieved && secondConditionAchieved && thirdConditionAchieved && fourthConditionAchieved;
        }

        private void CheckIfLevelFailed()
        {
            //TODO: Uncomment below when Turns are available
            levelFailed =
                defeatIfNotCompleteLevelInCertainAmountOfTurns && (numberOfPlayerTurns >= numberOfTurnsBeforeDefeat) ||
                (defeatIfProtectedIsKilled && unitToProtect.NoHealthLeft) ||
                (defeatIfAllPlayerUnitsDied &&
                 HumanPlayer.Instance.HaveAllUnitsDied()
                ) || (GameObject.Find("Franklem") == null || GameObject.Find("Franklem").GetComponent<Unit>().NoHealthLeft);
        }
        
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
                OnTurnGiven();
            }
        }

        public void CheckForComputerTurnSkip()
        {
            if (Input.GetKeyDown(Constants.SKIP_COMPUTER_TURN_KEY) && currentPlayer is ComputerPlayer)
            {
                isComputerPlaying = false;
                currentPlayer = players.Find(player => player is HumanPlayer);
                OnTurnGiven();
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
                OnTurnGiven();
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
