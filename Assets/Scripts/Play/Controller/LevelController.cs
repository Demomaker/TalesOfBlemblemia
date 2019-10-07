using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Game;
using UnityEngine;
using UnityEngine.UIElements;
using Finder = Harmony.Finder;

public class LevelController : MonoBehaviour
{
    //The turnSystem branches' changes are needed to continue
    [SerializeField] private string levelName;
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

    private bool levelCompleted = false;
    private bool levelFailed = false;
    private bool levelEnded = false;
    
    //Augment each turn
    private int numberOfPlayerTurns = 0;
    private void Update()
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
            /*if(!ComputerPlayer.Instance.HaveAllUnitsDied())*/ firstConditionAchieved = false;
        }
        if (completeIfPointAchieved)
        {
            if (!(GameObject.Find("Franklem").GetComponent<Unit>().CurrentTile.LogicalPosition == pointToAchieve)) secondConditionAchieved = false;
        }
        if (completeIfCertainEnemyDefeated)
        {
            if (!enemyToDefeat.IsDead) thirdConditionAchieved = false;
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
            (defeatIfProtectedIsKilled && unitToProtect.IsDead) || (defeatIfAllPlayerUnitsDied /*&& HumanPlayer.Instance.HaveAllUnitsDied()*/) || GameObject.Find("Franklem").GetComponent<Unit>().IsDead;
    }
}
