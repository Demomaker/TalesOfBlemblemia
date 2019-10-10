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
         
         public bool IsEnemy => playerType == PlayerType.Enemy;
         public bool IsPlayer => playerType == PlayerType.Ally;
         public bool IsRecruitable => playerType == PlayerType.None;

         [SerializeField] private PlayerType playerType;

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
         public UnitStats Stats => classStats + weapon.WeaponStats;

         /// <summary>
         /// The unit's weapon type
         /// </summary>
         public WeaponType WeaponType => weapon.WeaponType;

         /// <summary>
         /// The weapon type this unit has advantage on 
         /// </summary>
         public WeaponType WeaponAdvantage => weapon.Advantage;

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
         public int MovesLeft => movesLeft;
         public bool CanStillMove => movesLeft > 0;
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

         public void MoveTo(Tile tile)
         {
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
                 List<Tile> path = PathFinder.PrepareFindPath(gridController, movementCosts,
                     currentTile.LogicalPosition.x,
                     currentTile.LogicalPosition.y, tile.LogicalPosition.x, tile.LogicalPosition.y, IsEnemy);
                 path.Reverse();
                 path.RemoveAt(0);
                 path.Add(tile);
                 movesLeft -= currentTile.CostToMove;
                 MoveByPath(path);
             }
             movementCosts = PathFinder.PrepareComputeCost(tile.LogicalPosition);
         }

         private void LinkUnitToTile(Tile tile)
         {
             currentTile = tile;
             tile.LinkUnit(this);
         }


         private IEnumerator InitPosition()
         {
             yield return new WaitForEndOfFrame();
             MoveTo(gridController.GetTile(initialPosition.x, initialPosition.y));
         }

         //TODO changements pour la mécanique d'attaque
         //La chance de hit, le coup critique, la riposte ensuite
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

         private void MoveByPath(List<Tile> path)
         {
             StartCoroutine(MoveByPath(path,Constants.MOVEMENT_DURATION));
         }

         private IEnumerator MoveByPath(List<Tile> path, float duration)
         {
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
         }

         public void Rest()
         {
             HasActed = true;
             CurrentHealthPoints += HpGainedByResting;
             Debug.Log("Unit rested!");
         }
         
         /// <summary>
         /// Executes an action
         /// Author: Jérémie Bertrand, Zacharie Lavigne
         /// </summary>
         /// <param name="actionToDo">The action to execute on this turn</param>
         public void ExecuteAction(Action actionToDo)
         {
             if (!isMoving && movesLeft > 0 && actionToDo.Path.Count > 1 && actionToDo.Path.Last() != currentTile) 
                 MoveTo(actionToDo.Path.Last());
             else if (!isMoving && !isAttacking)
             {
                 if (actionToDo.ActionType == ActionType.Attack && actionToDo.Target != null)
                 {
                     if(!Attack(actionToDo.Target))
                         Rest();
                 }
                 else
                 {
                     Rest();
                 }
             }
         }

         public bool RecruitUnit()
         {
             if (IsRecruitable)
             {
                 playerType = PlayerType.Ally;
                 HumanPlayer.Instance.AddOwnedUnit(this);
                 Debug.Log(name + " has been recruited!");
             }
             return IsRecruitable;
         }

         public void ComputeTilesCosts()
         {
             if (currentTile != null) movementCosts = PathFinder.PrepareComputeCost(currentTile.LogicalPosition);
         }
         
     } 
 }
