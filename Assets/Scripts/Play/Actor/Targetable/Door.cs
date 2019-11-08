﻿using System;
using UnityEngine;

namespace Game
{
    public class Door : Targetable
    {
        [SerializeField] private int baseHealth = 1;

        public int BaseHealth => baseHealth;

        protected override void Start()
        {
            if (baseHealth < 1)
                throw new Exception("Door base health should be at least 1");
            CurrentHealthPoints = baseHealth;
            base.Start();
        }

        public override void Die()
        {
            currentTile.UnlinkDoor();
            Harmony.Finder.LevelController.IncrementTileUpdate();
            base.Die();
        }
    }
}