﻿using JetBrains.Annotations;
 using UnityEngine;
 
 namespace Game
{
    //Authors: Jérémie Bertrand & Mike Bédard
    public static class Constants
    {
        public const int PLAYER_MOVEMENT_RANGE = 3;
        public const int ENEMY_MOVEMENT_RANGE = 3;
        public const int PLAYER_ATTACK_RANGE = 1;
        public const int ENEMY_ATTACK_RANGE = 1;
        public const string LEVEL_1_SCENE_NAME = "Level1";
        public const string LEVEL_2_SCENE_NAME = "Level2";
        public const string LEVEL_3_SCENE_NAME = "ParabeneForest";
        public const string LEVEL_4_SCENE_NAME = "TestLevel";
        public const string LEVEL_5_SCENE_NAME = "Level5";
        public const string LEVEL_6_SCENE_NAME = "Level6";
        public const string LEVEL_7_SCENE_NAME = "Level7";
        public const string LEVEL_8_SCENE_NAME = "Level8";
        public const string OVERWORLD_SCENE_NAME = "Overworld";
        public const string GAME_CONTROLLER_TAG = "GameController";
        public const string GRID_CONTROLLER_TAG = "GridController";
        public const float ATTACK_DURATION = 0.3f;
        public const float MOVEMENT_DURATION = 0.3f;
        public const string PLAYER_NAME = "Leader of Allies";
        public const string AI_NAME = "Leader of Enemies";
        public const int DEFAULT_CHARACTER_HEALTH_POINTS = 6;
        public const int NUMBER_OF_MOVES_PER_CHARACTER_PER_TURN = 3;
        public const int NUMBER_OF_RECRUITABLES_ON_ALTERNATE_PATH = 8;
        public const string ACHIEVEMENT_GET_STRING = "Achievement Get!";
        public const KeyCode SKIP_COMPUTER_TURN_KEY = KeyCode.Space;

        public const bool DEFAULT_TOGGLE_VALUE = true;
        public const int DEFAULT_SLIDER_VALUE = 100;
        public const string DEFAULT_USERNAME = "Franklem";
        
        //Playable characters
        public const string FRANKLEM_NAME = "Franklem";
        public const string MYRIAM_NAME = "Myriam";
        public const string BRAM_NAME = "Bram";
        public const string RASS_NAME = "Rass";
        public const string ULRIC_NAME = "Ulric";
        public const string JEBEDIAH_NAME = "Jebediah";
        public const string THOMAS_NAME = "Thomas";
        public const string ABRAHAM_NAME = "Abraham";
        
        public static class AchievementName
        {
            public const string COMPLETE_CAMPAIGN_ON_EASY = "Baby Steps";
            public const string COMPLETE_CAMPAIGN_ON_MEDIUM = "Now you're getting it!";
            public const string COMPLETE_CAMPAIGN_ON_HARD = "You mad man!";
            public const string DEFEAT_BLACK_KNIGHT = "In my own castle...";
            public const string REACH_FINAL_LEVEL_WITH_8_PLAYERS = "The Octet";
            public const string FINISH_A_LEVEL_WITHOUT_UNIT_LOSS = "No one left behind...";
            public const string FINISH_CAMPAIGN_WITHOUT_UNIT_LOSS = "Great leader";
            public const string SAVE_ALL_RECRUITABLES_FROM_ALTERNATE_PATH = "Champion of the people!";
        }
    }
}