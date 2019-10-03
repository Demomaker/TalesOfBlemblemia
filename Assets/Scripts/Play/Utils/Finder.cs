﻿﻿using UnityEngine;
using Random = System.Random;

namespace Game
{
    //Authors: Jérémie Bertrand & Mike Bédard
    public static class Finder
    {
        private static GameController gameController;
        private static GridController gridController;
        private static Random random;
        
        public static GameController GameController
        {
            get
            {
                if (gameController == null)
                    gameController = GameObject.FindWithTag(Constants.GAME_CONTROLLER_TAG).GetComponent<GameController>();
                return gameController;
            }
            set => gameController = value;
        }

        public static GridController GridController
        {
            get
            {
                if (gridController == null)
                    gridController = GameObject.FindWithTag(Constants.GRID_CONTROLLER_TAG).GetComponent<GridController>();
                return gridController;
            }
        }
        
        public static Random Random
        {
            get
            {
                if (random == null)
                    random = new Random();
                return random;
            }
        } 
    }
}