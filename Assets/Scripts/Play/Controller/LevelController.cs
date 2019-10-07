using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UIElements;
using Finder = Harmony.Finder;

public class LevelController : MonoBehaviour
{
    //The turnSystem branches' changes are needed to continue
    [SerializeField] private string levelName;

    [Header("Level Completion Conditions")]
    [SerializeField] private bool completeIfAllEnemiesDefeated;
    [SerializeField] private bool completeIfPointAchieved;
    [SerializeField] private bool completeIfSurvivedCertainNumberOfTurns;
    [SerializeField] private bool completeIfCertainEnemyDefeated;

    [SerializeField] private Vector3 pointToAchieve;
    [SerializeField] private Unit enemyToDefeat;
    [SerializeField] private int numberOfTurnsBeforeCompletion;

    [Header("Level Defeat Conditions")] 
    [SerializeField] private bool defeatIfNotCompleteLevelInCertainAmountOfTurns;
    [SerializeField] private bool defeatIfProtectedIsKilled;
    [SerializeField] private bool defeatIfAllPlayerUnitsDied;

    [SerializeField] private Unit unitToProtect;
    [SerializeField] private int numberOfTurnsBeforeDefeat;

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
            if (!(GameObject.Find("Franklem").transform.position == pointToAchieve)) secondConditionAchieved = false;
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
            (defeatIfProtectedIsKilled && unitToProtect.IsDead) || (defeatIfAllPlayerUnitsDied /*&& HumanPlayer.Instance.HaveAllUnitsDied()*/);
    }
}
