﻿using System;
 using System.Collections;
 using System.Collections.Generic;
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
         public int MovesLeft
         {
             get => movesLeft;
         }
         public bool CanStillMove
         {
             get => movesLeft > 0;
         }

         public bool HasActed { get; set; } = false;

         public bool IsCurrentlySelected => gridController.SelectedUnit == this;
         public bool IsDead => currentHealthPoints <= 0;
         public int MovementRange => Stats.MoveSpeed;
         public int AttackRange => 1;

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
             movesLeft = Stats.MoveSpeed;
         }

         private void MoveTo(Vector3 position)
         {
             transform.position = position;
         }

         public void MoveTo(Tile tile)
         {
             movesLeft -= 1;
             if (currentTile != null) currentTile.UnlinkUnit();
             currentTile = tile;
             if (currentTile != null && currentTile.LinkUnit(this)) MoveTo(currentTile.WorldPosition);
         }


         private IEnumerator InitPosition()
         {
             yield return new WaitForEndOfFrame();
             movesLeft += 1;
             MoveTo(Finder.GridController.GetTile(initialPosition.x, initialPosition.y));
         }

         //TODO changements pour la mécanique d'attaque
         //La chance de hit, le coup critique, la riposte ensuite
         public bool Attack(Unit target, bool canCrit)
         {
             if (TargetIsInRange(target))
             {
                 HasActed = true;
                 target.currentHealthPoints -= 2;
                 //A unit cannot make a critical hit on a counter
                 target.Attack(this, false);
                 return true;
             }
             return false;
         }

         private bool TargetIsInRange(Unit target)
         {
             return currentTile.IsWithinRange(target.currentTile, AttackRange);
         }

         public void Die()
         {
             Destroy(gameObject);
         }


         public void MoveByPath(List<Tile> path)
         {
             //TODO changer ca, peut etre dans une coroutine
             for (int i = 0; i < path.Count; i++)
             {
                 if (movesLeft <= 0)
                     i = path.Count;
                 else
                 {
                     movesLeft--;
                     MoveTo(path[i]);
                 }
             }
         }

         public void Rest()
         {
             //TODO changer pour mettre fin au tour
             currentHealthPoints += HpGainedByResting;
         }
     } 
 }
