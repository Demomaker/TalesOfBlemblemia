﻿using JetBrains.Annotations;
 using UnityEngine;
 
 namespace Game
{
    //Authors: Jérémie Bertrand & Mike Bédard
    public static class CameraConstants
    {
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
        public const float MAX_CAM_MOVE_SPEED = 25f;
        public const float MIN_CAM_MOVE_SPEED = 10f;
        public const float MAX_CAM_ZOOM_SPEED = 10f;
        public const float MIN_CAM_ZOOM_SPEED = 1f;
        
        //Cinematic properties
        public const float MAX_CINEMATIC_TIME = 20f;
        public const float MIN_CINEMATIC_TIME = 0f;
        public const float DEFAULT_CINEMATIC_TIME = 3f;
        
    }
}