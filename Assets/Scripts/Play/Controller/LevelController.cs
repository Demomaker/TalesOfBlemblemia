using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// Controller for each individual level. Manages the turn functionalities as well as the players' turns
    /// Authors: Mike Bédard, Jérémie Bertrand, Zacharie Lavigne, Antoine Lessard
    /// </summary>
    [Findable("LevelController")]
    public class LevelController : MonoBehaviour
    {
        private const string PROTAGONIST_NAME = "Franklem";
        private const int CREDITS_DURATION = 20;

        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private string customObjectiveMessage = "";
        [SerializeField] private bool completeIfPointAchieved = false;
        [SerializeField] private bool completeIfSurvivedCertainNumberOfTurns = false;
        [SerializeField] private bool completeIfCertainTargetsDefeated = false;
        [SerializeField] private Vector2Int pointToAchieve = new Vector2Int();
        [SerializeField] private Targetable[] targetsToDefeat = null;
        [SerializeField] private bool allTargetsNeedToBeDefeated;
        [SerializeField] private Targetable[] targetsToProtect = null;
        [SerializeField] private int numberOfTurnsBeforeCompletion;
        [SerializeField] private bool revertWeaponTriangle = false;
        [SerializeField] private UnityEngine.Object pointingArrowPrefab = null;

        private CinematicController cinematicController;
        private int levelTileUpdateKeeper;
        private string levelName;
        private bool levelIsEnding;
        private bool isComputerPlaying;
        private OnLevelVictory onLevelVictory;
        private OnLevelFailed onLevelFailed;
        private OnLevelChange onLevelChange;
        private OnCampaignFailed onCampaignFailed;
        private UIController uiController;
        private LevelLoader levelLoader;
        private Unit[] units;
        private UnitOwner currentPlayer;
        private int numberOfPlayerTurns;
        private GameSettings gameSettings;
        private GameController gameController;
        private SaveController saveController;
        private EndGameCreditsController endGameCredits;
        
        private readonly HumanPlayer humanPlayer = new HumanPlayer();
        private readonly ComputerPlayer computerPlayer = new ComputerPlayer();

        private bool AllEnemiesDied => computerPlayer.HaveAllUnitsDied;
        private bool PointAchieved => completeIfPointAchieved && GameObject.Find(PROTAGONIST_NAME)?.GetComponent<Unit>()?.CurrentTile.LogicalPosition == pointToAchieve;
        private bool AllTargetsDefeated => completeIfCertainTargetsDefeated && AllTargetsToDefeatHaveBeenDefeated();
        private bool Survived => completeIfSurvivedCertainNumberOfTurns && numberOfPlayerTurns >= numberOfTurnsBeforeCompletion;
        private bool ProtagonistDied => GameObject.Find(PROTAGONIST_NAME) == null || GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().NoHealthLeft;
        private bool LevelCompleted => AllEnemiesDied || PointAchieved || AllTargetsDefeated || Survived;
        public bool PlayerUnitIsMovingOrAttacking => humanPlayer.OwnedUnits.All(unit => unit.IsMoving || unit.IsAttacking);
        private bool LevelFailed => ProtagonistDied;
        private bool LevelEnded => LevelCompleted || LevelFailed;
        public bool RevertWeaponTriangle => revertWeaponTriangle;
        public int LevelTileUpdateKeeper => levelTileUpdateKeeper;
        public AudioClip BackgroundMusic => backgroundMusic;
        public CinematicController CinematicController => cinematicController;
        public UnitOwner CurrentPlayer => currentPlayer;
        public HumanPlayer HumanPlayer => humanPlayer;
        public ComputerPlayer ComputerPlayer => computerPlayer;

        private void Awake()
        {
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelFailed = Harmony.Finder.OnLevelFailed;
            onLevelChange = Harmony.Finder.OnLevelChange;
            onCampaignFailed = Harmony.Finder.OnCampaignFailed;
            levelLoader = Harmony.Finder.LevelLoader;
            saveController = Finder.SaveController;
            gameController = Finder.GameController;
            gameSettings = Harmony.Finder.GameSettings;
            cinematicController = GetComponent<CinematicController>();
            levelName = gameObject.scene.name;
            endGameCredits = GetComponentInChildren<EndGameCreditsController>();
            uiController = Harmony.Finder.UIController;
            if (endGameCredits != null) endGameCredits.gameObject.SetActive(false);
        }
        
        private void Start()
        {
            onLevelChange.Publish(this);
            units = FindObjectsOfType<Unit>();
            GiveUnits(units);
            currentPlayer = humanPlayer;
            OnTurnGiven();
            ActivatePlayerUnits();
            CheckForDarkKnight();
            uiController.ModifyVictoryCondition(customObjectiveMessage);
            if (completeIfPointAchieved)
            {
                CreatePointToAchievePointingArrow();
            }
        }
        
        protected void Update()
        {
            if (LevelEnded) StartCoroutine(EndLevel());
            CheckForCurrentPlayerLoss();
            CheckForCurrentPlayerEndOfTurn();
            Play();
        }
        
        private IEnumerator EndLevel()
        {
            if (levelIsEnding) yield break;
            levelIsEnding = true;
            if(LevelCompleted) onLevelVictory.Publish(this);
            if(LevelFailed) PublishFailDependingOnDifficultyLevel(gameController.DifficultyLevel);
            while (cinematicController.IsPlayingACinematic) yield return null;
            if (endGameCredits != null)
            {
                endGameCredits.RollCredits();
                yield return new WaitForSeconds(CREDITS_DURATION);
            }
            CheckForPermadeath();
            CheckIfUnitWasRecruited();
            CheckIfUpperPathWasTaken();
            UpdatePlayerSave();
            levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
        }

        private void PublishFailDependingOnDifficultyLevel(DifficultyLevel difficultyLevel)
        {
            if (difficultyLevel == DifficultyLevel.Easy)
            {
                onLevelFailed.Publish(this);
            }
            else
            {
                onCampaignFailed.Publish(this);
            }
        }

        private void CreatePointToAchievePointingArrow()
        {
            var pointingArrowTransformOffset = new Vector2(0.5f, 0.5f);
            var pointingArrow = (GameObject)Instantiate(pointingArrowPrefab, Vector3.zero, Quaternion.identity, transform);
            pointingArrow.transform.SetParent(null);
            pointingArrow.GetComponent<PointingArrow>().SetTransformToPointPosition(new Vector3(pointToAchieve.x + pointingArrowTransformOffset.x, pointToAchieve.y + pointingArrowTransformOffset.y, 0));
        }

        private void CheckIfUpperPathWasTaken()
        {
            if (levelName == gameSettings.DarkTowerSceneName)
            {
                var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;
                foreach (var character in characterInfos.Where(character =>
                    character.CharacterName == gameSettings.AbrahamName || character.CharacterName == gameSettings.ThomasName))
                {
                    character.CharacterStatus = false;
                }
            }
        }

        private void CheckIfUnitWasRecruited()
        {
            foreach (var unit in units)
            {
                if (unit != null && unit.IsRecruitable)
                {
                    var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;
                    characterInfos.Find(info => info.CharacterName == unit.name).CharacterStatus = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Check for the player units defeated during the level and mark them as defeated in the player save if the difficulty
        /// is medium or hard. Resets the save of the player if Franklem was defeated.
        /// </summary>
        private void CheckForPermadeath()
        {
            var defeatedPlayerUnits = humanPlayer.DefeatedUnits;

            if (!defeatedPlayerUnits.Any()) return;
            var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;

            foreach (var unit in defeatedPlayerUnits)
            {
                if (gameController.PermaDeath)
                {
                    foreach (var character in characterInfos.Where(character => character.CharacterName == unit.name))
                    {
                        if (character.CharacterName == gameSettings.FranklemName)
                        {
                            saveController.ResetSave();
                            break;
                        }
                        character.CharacterStatus = false;
                    }
                }
            }
        }

        private bool AllTargetsToDefeatHaveBeenDefeated()
        {
            return allTargetsNeedToBeDefeated
                ? targetsToDefeat.Count(target => target == null || target.NoHealthLeft) == targetsToDefeat.Length
                : targetsToDefeat.Count(target => target == null || target.NoHealthLeft) > 0;
        }

        private void Play()
        {
            currentPlayer.RemoveDeadUnits();
            if (isComputerPlaying || currentPlayer != computerPlayer) return;
            isComputerPlaying = true;
            StartCoroutine(computerPlayer.PlayUnits());
        }

        private void GiveUnits(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                if (unit.IsPlayer)
                {
                    humanPlayer.AddOwnedUnit(unit);
                    computerPlayer.AddEnemyUnit(unit);
                }
                else if (unit.IsEnemy)
                {
                    computerPlayer.AddOwnedUnit(unit);
                    humanPlayer.AddEnemyUnit(unit);
                }
            }

            foreach (var target in targetsToProtect)
            {
                computerPlayer.AddTarget(target);
            }
        }

        private void CheckForCurrentPlayerEndOfTurn()
        {
            if (!currentPlayer.HasNoMorePlayableUnits) return;
            currentPlayer.ResetOwnedUnitsAlpha();
            GiveTurnToNextPlayer();
            OnTurnGiven();
        }
        
        private void OnTurnGiven()
        {
            if (currentPlayer == humanPlayer)
            {
                numberOfPlayerTurns++;
                uiController.ModifyTurnCounter(numberOfPlayerTurns);
                EnemyRangeController.OnPlayerTurn(computerPlayer.OwnedUnits);
            }
            else
            {
                EnemyRangeController.OnComputerTurn();
            }
            uiController.ModifyTurnInfo(currentPlayer);
            currentPlayer.OnTurnGiven();
        }


        private void CheckForCurrentPlayerLoss()
        {
            if (currentPlayer != null && !currentPlayer.HaveAllUnitsDied) return;
            GiveTurnToNextPlayer();
        }

        private void GiveTurnToNextPlayer()
        {
            isComputerPlaying = false;
            currentPlayer = (currentPlayer != humanPlayer) ? (UnitOwner) humanPlayer : computerPlayer;
        }
        
        /// <summary>
        /// Activating only the units that are not marked as defeated in the player's save file
        /// </summary>
        private void ActivatePlayerUnits()
        {
            var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;

            var unitsToRemove = (from gameUnit in currentPlayer.OwnedUnits
                from saveUnit in characterInfos
                where gameUnit.name == saveUnit.CharacterName && !saveUnit.CharacterStatus
                select gameUnit).ToList();

            foreach (var unit in unitsToRemove)
            {
                unit.gameObject.SetActive(false);
                currentPlayer.OwnedUnits.Remove(unit);
                computerPlayer.RemoveEnemyUnit(unit);
            }
        }
        /// <summary>
        /// Saves if the level was successfully completed
        /// </summary>
        private void UpdatePlayerSave()
        {
            //If the level was successfully completed, mark it as completed
            if (!LevelCompleted) return;
            gameController.OnLevelCompleted(levelName);
            saveController.GetCurrentSaveSelectedInfos().LevelName = gameController.PreviousLevelName;
            saveController.UpdateSave(saveController.SaveSelected);
        }

        public void IncrementTileUpdate()
        {
            levelTileUpdateKeeper++;
        }
        
        private void CheckForDarkKnight()
        {
            if (levelName == gameSettings.MorktressSceneName && gameController.PreviousLevelName == gameSettings.DarkTowerSceneName)
            {
                if (computerPlayer.OwnedUnits.Find(info => info.name == gameSettings.DarkKnightName) != null)
                    computerPlayer.OwnedUnits.Find(info => info.name == gameSettings.DarkKnightName).gameObject.SetActive(false);
            }
        }
    }
}