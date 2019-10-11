 using System;
 using System.Collections;
 using System.Collections.Generic;
 using System.Linq;
 using DG.Tweening;
 using UnityEngine;
 using Random = UnityEngine.Random;

 namespace Game
 {
     //Authors: Jérémie Bertrand, Zacharie Lavigne
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

         public int CurrentHealthPoints
         {
             get { return currentHealthPoints; }
             private set
             {
                 currentHealthPoints = value;
                 if (NoHealthLeft)
                 {
                     Die();
                 }
             }
         }

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
                 if (CurrentHealthPoints + maxGain > Stats.MaxHealthPoints)
                     return Stats.MaxHealthPoints - CurrentHealthPoints;
                 return maxGain;
             }
         }

         private int movesLeft;
         private bool canPlay = true;
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
         public bool NoHealthLeft => CurrentHealthPoints <= 0;
         public int MovementRange => Stats.MoveSpeed;
         public int AttackRange => 1;
         
         private bool isMoving = false;
         public bool IsMoving => isMoving;

         private bool isAttacking = false;
         public bool IsAttacking => isAttacking;

         private void Awake()
         {
             gridController = Finder.GridController;
             
             classStats = UnitStats.SoldierUnitStats;
             weapon = Sword.BasicWeapon;
             CurrentHealthPoints = Stats.MaxHealthPoints;
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

         private List<Tile> PrepareMove(Tile tile)
         {
             currentTile.UnlinkUnit();
             isMoving = true;
             List<Tile> path = PathFinder.PrepareFindPath(gridController, movementCosts,
                 currentTile.LogicalPosition.x,
                 currentTile.LogicalPosition.y, tile.LogicalPosition.x, tile.LogicalPosition.y, this);
             path.Reverse();
             path.RemoveAt(0);
             path.Add(tile);
             movesLeft -= currentTile.CostToMove;
             return path;
         }
         
         public void MoveToAction(Action action)
         {
             var tile = action.Path.Last();
             if (tile == null || tile == currentTile || isMoving) return;
             if (currentTile == null)
             {
                 transform.position = tile.WorldPosition;
                 LinkUnitToTile(tile);
             }
             else
             {
                 currentTile.UnlinkUnit();
                 isMoving = true;
                 action.Path.RemoveAt(0);
                 movesLeft -= currentTile.CostToMove;
                 MoveByAction(action);
             }
             movementCosts = PathFinder.PrepareComputeCost(tile.LogicalPosition, IsEnemy);
         }

         private void LinkUnitToTile(Tile tile)
         {
             currentTile = tile;
             tile.LinkUnit(this);
         }
         
         private IEnumerator InitPosition()
         {
             yield return new WaitForEndOfFrame();
             MoveToTileAndAct(gridController.GetTile(initialPosition.x, initialPosition.y));
         }
         
         public bool Attack(Unit target, bool isCountering = false)
         {
             if (TargetIsInRange(target))
             {
                 StartCoroutine(Attack(target, isCountering, Constants.ATTACK_DURATION));
                 return true;
             }
             return false;
         }

         private IEnumerator Attack(Unit target, bool isCountering, float duration)
         {
             if (isAttacking) yield break;
             isAttacking = true;
             
             float counter = 0;
             Vector3 startPos = transform.position;
             Vector3 targetPos = (target.CurrentTile.WorldPosition + startPos) / 2f;
             LookAt(targetPos);
             duration /= 2;

             while (counter < duration)
             {
                 counter += Time.deltaTime;
                 transform.position = Vector3.Lerp(startPos, targetPos, counter / duration);
                 yield return null;
             }

             if (!isCountering)
             {
                 HasActed = true;
             }

             Debug.Log(WeaponAdvantage.ToString());
             float hitRate = Stats.HitRate - target.currentTile.DefenseRate;
             int damage = Random.value <= hitRate ? Stats.AttackStrength : 0;
             if (!isCountering && target.WeaponType == WeaponAdvantage)
             {
                 damage *= Random.value <= Stats.CritRate ? 2 : 1;
             }
             
             target.CurrentHealthPoints -= damage;
             counter = 0;
             
             while (counter < duration)
             {
                 counter += Time.deltaTime;
                 transform.position = Vector3.Lerp(targetPos, startPos, counter / duration);
                 yield return null;
             }
             
             transform.position = startPos;
             isAttacking = false;
             
             //A unit cannot make a critical hit on a counter
             //A unit cannot counter on a counter
             if (!isCountering && !target.NoHealthLeft)
                 target.Attack(this, true);

         }

         private void LookAt(Vector3 target)
         {
             transform.localRotation = Quaternion.Euler(0, target.x < transform.position.x ? 180 : 0, 0);
         }

         public bool TargetIsInRange(Unit target)
         {
             return currentTile.IsWithinRange(target.currentTile, AttackRange);
         }

         public void Die()
         {
             currentTile.UnlinkUnit();
             Harmony.Finder.LevelController.ReevaluateAllMovementCosts();
             Destroy(gameObject);
         }

         /// <summary>
         /// Starts to move following an action path and then executes the action
         /// Author: Jérémie Bertrand
         /// </summary>
         /// <param name="action">The action to execute</param>
         public void MoveByAction(Action action)
         {
             StartCoroutine(MoveByAction(action, Constants.MOVEMENT_DURATION));
         }

         /// <summary>
         /// Moves following an action path and then executes the action
         /// Author: Jérémie Bertrand
         /// Contributor: Zacharie Lavigne
         /// </summary>
         /// <param name="action">The action to execute</param>
         /// <param name="duration">The duration in seconds to walk to each tile</param>
         /// <returns>Is a coroutine</returns>
         private IEnumerator MoveByAction(Action action, float duration)
         {
             var path = action.Path;
             
             Tile finalTile = null;
             for (int i = 0; i < path.Count; i++)
             {
                 finalTile = path[i];
                 float counter = 0;

                 if(path.IndexOf(finalTile) != path.Count - 1) 
                     movesLeft -= finalTile.CostToMove;
                 Vector3 startPos = transform.position;
                 LookAt(finalTile.WorldPosition);

                 while (counter < duration)
                 {
                     counter += Time.deltaTime;
                     transform.position = Vector3.Lerp(startPos, finalTile.WorldPosition, counter / duration);
                     yield return null;
                 }

                 if (movesLeft < 0 && path.IndexOf(finalTile) != path.Count - 1)
                 {
                     i = path.Count;
                 }
             }
             LinkUnitToTile(finalTile);
             transform.position = currentTile.WorldPosition;
             isMoving = false;
             if (action.ActionType != ActionType.Nothing)
             {
                 if (action.ActionType == ActionType.Attack && action.Target != null)
                 {
                     if (!Attack(action.Target))
                         Rest();
                 }
                 else
                 {
                     Rest();
                 }
             }
         }

         public void Rest()
         {
             HasActed = true;
             CurrentHealthPoints += HpGainedByResting;
             Debug.Log("Unit rested!");
         }
         
         /// <summary>
         /// Executes an action for the AI
         /// Author: Jérémie Bertrand, Zacharie Lavigne
         /// </summary>
         /// <param name="actionToDo">The action to execute on this turn</param>
         public void ExecuteAction(Action actionToDo)
         {
             if (!isMoving && movesLeft > 0 && actionToDo.Path.Count > 1 && actionToDo.Path.Last() != currentTile)
                 MoveToAction(actionToDo);
             else if (!isMoving && !isAttacking)
             {
                 if (actionToDo.ActionType == ActionType.Attack && actionToDo.Target != null)
                 {
                     if (!Attack(actionToDo.Target))
                         Rest();
                 }
                 else
                 {
                     Rest();
                 }
             }
         }

         public void ComputeTilesCosts()
         {
             if (currentTile != null)
                 movementCosts = PathFinder.PrepareComputeCost(currentTile.LogicalPosition, IsEnemy);
         }

         /// <summary>
         /// Starts a series of action to move to a specific tile and do an action afterwards
         /// Author: Zacharie Lavigne
         /// </summary>
         /// <param name="tile">The destination</param>
         /// <param name="actionType">The type of action to execute</param>
         /// <param name="target">The target if the action is to attack</param>
         public void MoveToTileAndAct(Tile tile, ActionType actionType = ActionType.Nothing, Unit target = null)
         {
             if (tile == null || tile == currentTile || isMoving) return;
             if (currentTile == null)
             {
                 transform.position = tile.WorldPosition;
                 LinkUnitToTile(tile);
             }
             else
             {
                 MoveByAction(new Action(PrepareMove(tile), actionType, target));
             }
             movementCosts = PathFinder.PrepareComputeCost(tile.LogicalPosition, IsEnemy);
         }

         /// <summary>
         /// Starts a series of action to move next to an enemy unit and attack it
         /// Author: Zacharie Lavigne
         /// </summary>
         /// <param name="target">The unit to attack</param>
         public void AttackDistantUnit(Unit target)
         {
             var adjacentTile = gridController.FindAvailableAdjacentTile(target.CurrentTile	, this);
             if (adjacentTile != null)
                MoveToTileAndAct(adjacentTile, ActionType.Attack, target);
         }
     } 
 }
