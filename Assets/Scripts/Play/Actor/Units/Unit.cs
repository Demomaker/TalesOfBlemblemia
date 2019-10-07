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
         
         private bool isMoving = false;
         private bool isAttacking = false;

         private void Awake()
         {
             gridController = Finder.GridController;
             
             classStats = UnitStats.SoldierUnitStats;
             weapon = Sword.BasicWeapon;
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
             tile.LinkUnit(this);
             currentTile = tile;
             movementCosts = PathFinder.PrepareComputeCost(tile.LogicalPosition);
         }


         private IEnumerator InitPosition()
         {
             yield return new WaitForEndOfFrame();
             MoveTo(gridController.GetTile(initialPosition.x, initialPosition.y));
         }

         //TODO changements pour la mécanique d'attaque
         //La chance de hit, le coup critique, la riposte ensuite
         public bool Attack(Unit target, bool isCountering = true)
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

             HasActed = true;
             target.currentHealthPoints -= 2;
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
             if (isCountering && !target.IsDead)
                 target.Attack(this, false);
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
             Destroy(gameObject);
         }
         
         public void MoveByPath(List<Tile> path)
         {
             StartCoroutine(MoveByPath(path,Constants.MOVEMENT_DURATION));
         }

         private IEnumerator MoveByPath(List<Tile> path, float duration)
         {
             if (isMoving) yield break;
             isMoving = true;

             foreach (var tile in path)
             {
                 float counter = 0;

                 if(path.IndexOf(tile) != path.Count - 1) movesLeft -= tile.CostToMove;
                 Vector3 startPos = transform.position;
                 LookAt(tile.WorldPosition);

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
             HasActed = true;
             currentHealthPoints += HpGainedByResting;
         }
     } 
 }