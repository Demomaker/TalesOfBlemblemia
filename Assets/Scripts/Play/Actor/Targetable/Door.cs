using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    //Author: Zacharie Lavigne
    public class Door : Targetable
    {
        [SerializeField] private int baseHealth = 1;
        private LevelController levelController;

        protected override void Awake()
        {
            levelController = Harmony.Finder.LevelController;
            base.Awake();
        }
        protected override void Start()
        {
            if (baseHealth < 1) throw new Exception("Door base health should be at least 1");
            CurrentHealthPoints = baseHealth;
            base.Start();
        }

        protected override IEnumerator Die()
        {
            currentTile.UnlinkDoor();
            levelController.IncrementTileUpdate();
            yield return base.Die();
        }
    }
}