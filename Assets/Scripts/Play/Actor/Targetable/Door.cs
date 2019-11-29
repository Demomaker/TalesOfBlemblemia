using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    //Author: Zacharie Lavigne
    public class Door : Targetable
    {
        [SerializeField] private int baseHealth = 1;

        protected override void Start()
        {
            if (baseHealth < 1) throw new Exception("Door base health should be at least 1");
            CurrentHealthPoints = baseHealth;
            base.Start();
        }

        protected override IEnumerator Die()
        {
            currentTile.UnlinkDoor();
            Harmony.Finder.LevelController.IncrementTileUpdate();
            yield return base.Die();
        }
    }
}