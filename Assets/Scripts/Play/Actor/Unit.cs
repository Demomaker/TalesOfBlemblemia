﻿using System;
 using System.Collections;
 using System.Collections.Generic;
 using System.Linq;
 using DG.Tweening;
 using UnityEngine;

 namespace Game
 {
     //Authors: Jérémie Bertrand & Mike Bédard
     public class Unit : MonoBehaviour
     {
         [SerializeField] private Vector2Int initialPosition;
         private GridController gridController;
         /// <summary>
         /// The tile the unit is on
         /// </summary>
         private Tile currentTile = null;
         public Tile CurrentTile => currentTile;
         
         /// <summary>
         /// Value of if a unit is an enemy 
         /// </summary>
         [SerializeField] private bool isEnemy;
         public bool IsEnemy => isEnemy;

         /// <summary>
         /// The unit's class stats
         /// </summary>
         [SerializeField] private UnitStats classStats;

         /// <summary>
         /// The unit's weapon
         /// </summary>
         [SerializeField] private Weapon weapon;

         /// <summary>
         /// Array representing the movement cost needed to move to every tile on the grid
         /// </summary>
         private int[,] movementCosts;
         public int[,] MovementCosts => movementCosts;

         /// <summary>
         /// The unit's current health
         /// </summary>
         private int currentHealthPoints;

         public int CurrentHealthPoints => currentHealthPoints;

         /// <summary>
         /// The unit's stats
         /// </summary>
         public UnitStats Stats
         {
             get { return classStats + weapon.WeaponStats; }
         }

         /// <summary>
         /// The unit's weapon type
         /// </summary>
         public WeaponType WeaponType
         {
             get { return weapon.WeaponType; }
         }

         /// <summary>
         /// The weapon type this unit has advantage on 
         /// </summary>
         public WeaponType WeaponAdvantage
         {
             get { return weapon.Advantage; }
         }

         /// <summary>
         /// The health points a unit would gain by resting
         /// Resting replenishes half your health points without exceeding the unit's max health
         /// </summary>
         public int HpGainedByResting
         {
             get
             {
                 int maxGain = Stats.MaxHealthPoints / 2;
                 if (currentHealthPoints + maxGain > Stats.MaxHealthPoints)
                     return Stats.MaxHealthPoints - currentHealthPoints;
                 return maxGain;
             }
         }

         private int movesLeft;
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
         //TODO changer pour que ca soit quand l'unité a fait son action
         public bool HasDoneAction => MovesLeft > 0;
         public bool IsDead => currentHealthPoints <= 0;
         public int MovementRange => Stats.MoveSpeed;
         public int AttackRange => 1;
         
         bool isMoving = false;

         private void Awake()
         {
             gridController = Finder.GridController;
             
             classStats = UnitStats.SoldierUnitStats;
             weapon = Axe.BasicWeapon;
             currentHealthPoints = Stats.MaxHealthPoints;
             movesLeft = Stats.MoveSpeed;
         }

         protected void Start()
         {
             StartCoroutine(InitPosition());
         }

         public void ResetNumberOfMovesLeft()
         {
             MovesLeft = Constants.NUMBER_OF_MOVES_PER_CHARACTER_PER_TURN;
         }

         public void MoveTo(Tile tile)
         {
             if (tile == null) return;
             if (currentTile == null)
             {
                 transform.position = tile.WorldPosition;
             }
             else
             {
                 currentTile.UnlinkUnit();
                 List<Tile> path = PathFinder.PrepareFindPath(gridController, movementCosts,
                     currentTile.LogicalPosition.x,
                     currentTile.LogicalPosition.y, tile.LogicalPosition.x, tile.LogicalPosition.y);
                 path.Reverse();
                 path.RemoveAt(0);
                 path.Add(tile);
                 movesLeft -= currentTile.CostToMove;
                 MoveByPath(path);
             }
             currentTile = tile;
             currentTile.LinkUnit(this);
             movementCosts = PathFinder.PrepareComputeCost(tile.LogicalPosition);
         }


         private IEnumerator InitPosition()
         {
             yield return new WaitForEndOfFrame();
             MoveTo(gridController.GetTile(initialPosition.x, initialPosition.y));
         }

         //TODO changements pour la mécanique d'attaque
         public void Attack(Unit target)
         {
             if (TargetIsInRange(target))
             {
                 //TODO changer pour mettre fin au tour
                 MovesLeft -= 1;
                 target.currentHealthPoints -= 2;
             }
         }

         private bool TargetIsInRange(Unit target)
         {
             return (Math.Abs(target.CurrentTile.LogicalPosition.x - this.currentTile.LogicalPosition.x) <= AttackRange
                 && Math.Abs(target.CurrentTile.LogicalPosition.y - this.currentTile.LogicalPosition.y) <= AttackRange);
         }

         public void Die()
         {
             Destroy(gameObject);
         }

         public void MoveByPath(List<Tile> path)
         {
             StartCoroutine(MoveByPath(path,0.2f));
         }

         private IEnumerator MoveByPath(List<Tile> path, float duration = 0.2f)
         {
             if (isMoving)
             {
                 yield break;
             }
             
             isMoving = true;

             foreach (var tile in path)
             {
                 float counter = 0;

                 if(path.IndexOf(tile) != path.Count - 1) movesLeft -= tile.CostToMove;
                 
                 Vector3 startPos = transform.position;

                 while (counter < duration)
                 {
                     counter += Time.deltaTime;
                     transform.position = Vector3.Lerp(startPos, tile.WorldPosition, counter / duration);
                     yield return null;
                 }
             }

             transform.position = currentTile.WorldPosition;
             
             isMoving = false;
         }

         public void Rest()
         {
             //TODO changer pour mettre fin au tour
             currentHealthPoints += HpGainedByResting;
         }
     } 
 }
