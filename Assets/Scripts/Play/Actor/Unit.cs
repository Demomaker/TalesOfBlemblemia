﻿using System.Collections;
 using UnityEngine;

 namespace Game
 {
     //Authors: Jérémie Bertrand & Mike Bédard
     public abstract class Unit : MonoBehaviour
     {
         [SerializeField] private Vector2Int initialPosition;
         private GridController gridController;
         private Tile currentTile = null;
         private readonly int movementRange;
         private readonly int attackRange;
         private int healthPoints = Constants.DEFAULT_CHARACTER_HEALTH_POINTS;
         private int movesLeft = Constants.NUMBER_OF_MOVES_PER_CHARACTER_PER_TURN;
         private bool canPlay = true;

         public int MovesLeft
         {
             get => movesLeft;
             set => movesLeft = value;
         }

         public bool CanPlay
         {
             get => canPlay;
             set => canPlay = value;
         }

         public bool IsCurrentlySelected => gridController.SelectedUnit == this;
         public bool CanMove => MovesLeft > 0;
         public bool IsDead => healthPoints <= 0;
         public int MovementRange => movementRange;
         public int AttackRange => attackRange;

         protected Unit(int movementRange, int attackRange)
         {
             this.movementRange = movementRange;
             this.attackRange = attackRange;
         }

         private void Awake()
         {
             gridController = Finder.GridController;
         }

         protected void Start()
         {
             StartCoroutine(InitPosition());
         }

         public void ResetNumberOfMovesLeft()
         {
             MovesLeft = Constants.NUMBER_OF_MOVES_PER_CHARACTER_PER_TURN;
         }

         private void MoveTo(Vector3 position)
         {
             transform.position = position;
         }

         public void MoveTo(Tile tile)
         {
             MovesLeft -= 1;
             if (currentTile != null) currentTile.UnlinkUnit();
             currentTile = tile;
             if (currentTile != null && currentTile.LinkUnit(this)) MoveTo(currentTile.WorldPosition);
         }


         private IEnumerator InitPosition()
         {
             yield return new WaitForEndOfFrame();
             MovesLeft += 1;
             MoveTo(Finder.GridController.GetTile(initialPosition.x, initialPosition.y));
         }

         public void Attack(Unit unit)
         {
             MovesLeft -= 1;
             unit.healthPoints -= 2;
         }

         public void Die()
         {
             Destroy(gameObject);
         }
     }
 }

