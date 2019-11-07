﻿using JetBrains.Annotations;
 using UnityEngine;
 
 namespace Game
{
    //Authors: Jérémie Bertrand & Mike Bédard
    public static class Constants
    {
        
        public const int PERCENT = 100;
        
        public const int PLAYER_MOVEMENT_RANGE = 3;
        public const int ENEMY_MOVEMENT_RANGE = 3;
        public const int PLAYER_ATTACK_RANGE = 1;
        public const int ENEMY_ATTACK_RANGE = 1;
        public const float ATTACK_DURATION = 0.3f;
        public const float MOVEMENT_DURATION = 0.3f;
        public const int NUMBER_OF_RECRUITABLES_ON_ALTERNATE_PATH = 8;
        
        public const KeyCode SKIP_COMPUTER_TURN_KEY = KeyCode.Space;
        
        public const string LEVEL_1_SCENE_NAME = "Tutorial";
        public const string LEVEL_2_SCENE_NAME = "Level2";
        public const string LEVEL_3_SCENE_NAME = "ParabeneForest";
        public const string LEVEL_4_SCENE_NAME = "TestLevel";
        public const string LEVEL_5_SCENE_NAME = "Level5";
        public const string LEVEL_6_SCENE_NAME = "Level6";
        public const string LEVEL_7_SCENE_NAME = "Level7";
        public const string LEVEL_8_SCENE_NAME = "Level8";
        public const string OVERWORLD_SCENE_NAME = "Overworld";
        public const string MAINMENU_SCENE_NAME = "MainMenu";
        public const string GAME_UI_SCENE_NAME = "GameUI";

        public const bool DEFAULT_TOGGLE_VALUE = true;
        public const int DEFAULT_SLIDER_VALUE = 100;
        public const string DEFAULT_USERNAME = "Franklem";
        public const int SAVE_SLOT_ONE = 1;
        public const int SAVE_SLOT_TWO = 2;
        public const int SAVE_SLOT_THREE = 3;
        
        //Playable characters
        public const string FRANKLEM_NAME = "Franklem";
        public const string MYRIAM_NAME = "Myriam";
        public const string BRAM_NAME = "Bram";
        public const string RASS_NAME = "Rass";
        public const string ULRIC_NAME = "Ulric";
        public const string JEBEDIAH_NAME = "Jebediah";
        public const string THOMAS_NAME = "Thomas";
        public const string ABRAHAM_NAME = "Abraham";
        
        public const string ACHIEVEMENT_GET_STRING = "Achievement Get!";
        // Camera values
        public const float MIN_CAM_SCROLL_AREA = 5f;
        public const float MAX_CAM_SCROLL_AREA = 20f;
        public const float DEFAULT_CAM_ORTHOGRAPHIC_SIZE = 7.5f;
        public const float MIN_CAM_ORTHOGRAPHIC_SIZE = 3f;
        public const float MAX_CAM_ORTHOGRAPHIC_SIZE = 15f;
        public const float MAX_CAM_X = 100f;
        public const float MIN_CAM_X = -100f;
        public const float MAX_CAM_Y = 100f;
        public const float MIN_CAM_Y = -100f;
        public const float MAX_CAM_MOVE_SPEED = 30f;
        public const float MIN_CAM_MOVE_SPEED = 10f;
        public const float MAX_CAM_ZOOM_SPEED = 10f;
        public const float MIN_CAM_ZOOM_SPEED = 1f;
        
        public const float MAX_CINEMATIC_TIME = 20f;
        public const float MIN_CINEMATIC_TIME = 0f;
        public const float DEFAULT_CINEMATIC_TIME = 3f;
        
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

        public static class AnimationProperties
        {
            public const string IS_MOVING = "IsMoving";
            public const string IS_ATTACKING = "IsAttacking";
            public const string IS_GOING_TO_DIE = "IsGoingToDie";
            public const string IS_BEING_HURT = "IsBeingHurt";
            public const string IS_DODGING = "IsDodging";
            public const string IS_RESTING = "IsResting";
        }
    }
}