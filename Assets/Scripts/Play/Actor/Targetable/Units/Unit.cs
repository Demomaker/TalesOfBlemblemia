using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    //Authors: Jérémie Bertrand, Zacharie Lavigne
    public class Unit : Targetable
    {
        public Vector2Int Position => currentTile.LogicalPosition;
        
        #region Serialized fields
        [SerializeField] private PlayerType playerType;
        [SerializeField] private UnitStats classStats;
        #endregion
        
        #region Fields
        private GridController gridController;
        private Weapon weapon;
        private int[,] movementCosts;
        private int movesLeft;
        private int tileUpdateKeeper;
        #endregion
        
        #region Properties
        public int HpGainedByResting
        {
            get
            {
                int maxGain = Stats.MaxHealthPoints / 4;
                if (CurrentHealthPoints + maxGain > Stats.MaxHealthPoints)
                    return Stats.MaxHealthPoints - CurrentHealthPoints;
                return maxGain;
            }
        }
        public int HpGainedByHealing
        {
            get
            {
                int maxGain = Stats.MaxHealthPoints / 2;
                if (CurrentHealthPoints + maxGain > Stats.MaxHealthPoints)
                    return Stats.MaxHealthPoints - CurrentHealthPoints;
                return maxGain;
            }
        }

        public int[,] MovementCosts
        {
            get
            {
                if (tileUpdateKeeper != Harmony.Finder.LevelController.LevelTileUpdateKeeper)
                {
                    if (currentTile != null)
                        MovementCosts = PathFinder.PrepareComputeCost(currentTile.LogicalPosition, IsEnemy);
                }
                return movementCosts;
            }
            set
            {
                movementCosts = value;
                tileUpdateKeeper = Harmony.Finder.LevelController.LevelTileUpdateKeeper;
            }
        }
        public bool IsEnemy => playerType == PlayerType.Enemy;
        public bool IsPlayer => playerType == PlayerType.Ally;
        public bool IsRecruitable => playerType == PlayerType.None;
        public UnitStats Stats => classStats + weapon.WeaponStats;
        public WeaponType WeaponType => weapon.WeaponType;
        public WeaponType WeaponAdvantage => weapon.Advantage;
        public int MovesLeft => movesLeft;
        public bool HasActed { get; set; }
        public int AttackRange => 1;
        private bool isMoving;
        private bool isAttacking;
        #endregion
        
        private void Awake()
        {
            weapon = GetComponentInParent<Weapon>();
            if (weapon == null)
                throw new Exception("A unit gameObject should have a weapon script");
            gridController = Finder.GridController;
            CurrentHealthPoints = Stats.MaxHealthPoints;
            movesLeft = Stats.MoveSpeed;
        }
        
        public void ResetTurnStats()
        {
            HasActed = false;
            movesLeft = Stats.MoveSpeed;
        }
        
        private void LookAt(Vector3 target)
        {
            transform.localRotation = Quaternion.Euler(0, target.x < transform.position.x ? 180 : 0, 0);
        }

        #region Movements
        public List<Tile> PrepareMove(Tile tile)
        {
            isMoving = true;
            if (tile != currentTile)
            {
                currentTile.UnlinkUnit();
                List<Tile> path = PathFinder.PrepareFindPath(gridController, movementCosts,
                    new Vector2Int(currentTile.LogicalPosition.x, currentTile.LogicalPosition.y), 
                    new Vector2Int(tile.LogicalPosition.x, tile.LogicalPosition.y), this);
                path.RemoveAt(0);
                path.Add(tile);
                movesLeft -= currentTile.CostToMove;
                return path;
            }
            return null;
        }
        public Coroutine MoveByAction(Action action)
        {
            return StartCoroutine(MoveByAction(action, Constants.MOVEMENT_DURATION));
        }
        private IEnumerator MoveByAction(Action action, float duration)
        {
            var path = action.Path;
            if (path != null /*&& ((IsEnemy && path.Count > 1) || !IsEnemy)*/)
            {
                Tile finalTile = null;
                for (int i = 0; i < path.Count; i++)
                {
                    if (path[i] != null)
                        finalTile = path[i];
                    float counter = 0;

                    if (path.IndexOf(finalTile) != path.Count - 1)
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

                CurrentTile = finalTile;
                if (currentTile != null) transform.position = currentTile.WorldPosition;
                isMoving = false;
            }

            if (action.ActionType != ActionType.Nothing)
            {
                if (action.ActionType == ActionType.Attack && action.Target != null)
                {
                    if (!Attack(action.Target))
                        Rest();
                }
                if (action.ActionType == ActionType.Recruit && action.Target != null)
                {
                    if (action.Target.GetType() == typeof(Unit) && !RecruitUnit((Unit)action.Target))
                        Rest();
                }
                if (action.ActionType == ActionType.Heal && action.Target != null)
                {
                    if (action.Target.GetType() == typeof(Unit) && !HealUnit((Unit)action.Target))
                        Rest();
                }
                else
                {
                    Rest();
                }
            }
        }

        public override void Die()
        {
            currentTile.UnlinkUnit();
            base.Die();
        }
        #endregion
        
        #region Action controlls
        public void Rest()
        {
            CurrentHealthPoints += HpGainedByResting;
            Debug.Log("Unit rested!");
            HasActed = true;
        }
        
        public void AttackDistantTargetable(Targetable target)
        {
            var adjacentTile = gridController.FindAvailableAdjacentTile(target.CurrentTile, this);
            if (adjacentTile != null)
                MoveByAction(new Action(PrepareMove(adjacentTile), ActionType.Attack, target));
        }
        public bool Attack(Targetable target, bool isCountering = false)
        {
            if (TargetIsInRange(target))
            {
                StartCoroutine(Attack(target, isCountering, Constants.ATTACK_DURATION));
                return true;
            }
            return false;
        }
        private IEnumerator Attack(Targetable target, bool isCountering, float duration)
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

            Debug.Log(WeaponAdvantage.ToString());
            
            float hitRate = Stats.HitRate - target.CurrentTile.DefenseRate;
            int damage = Random.value <= hitRate ? Stats.AttackStrength : 0;
            
            if (target.GetType() == typeof(Unit) && !isCountering && ((Unit)target).WeaponType == WeaponAdvantage)
            {
                damage *= Random.value <= Stats.CritRate ? 2 : 1;
            }
            Finder.SoundManager.PlaySingle(Finder.SoundClips.HurtSound);
            
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
            if (target.GetType() == typeof(Unit) && !isCountering && !target.NoHealthLeft)
                ((Unit)target).Attack(this, true);
            
            
            if (!isCountering)
            {
                HasActed = true;
            }
        }
        
        public void RecruitDistantUnit(Unit target)
        {
            var adjacentTile = gridController.FindAvailableAdjacentTile(target.CurrentTile, this);
            if (adjacentTile != null)
                MoveByAction(new Action(PrepareMove(adjacentTile), ActionType.Recruit, target));
        }
        public bool RecruitUnit()
        {
            if (IsRecruitable)
            {
                playerType = PlayerType.Ally;
                HumanPlayer.Instance.AddOwnedUnit(this);
                GetComponent<DialogueTrigger>()?.TriggerDialogue();
                Debug.Log(name + " has been recruited!");
            }
            return IsRecruitable;
        }
        public bool RecruitUnit(Unit unitToRecruit)
        {
            if (TargetIsInRange(unitToRecruit))
            {
                return unitToRecruit.RecruitUnit();
            }
            return false;
        }
        
        public bool HealUnit(Unit target)
        {
            if (TargetIsInRange(target))
            {
                target.Heal();
                HasActed = true;
                return true;
            }
            return false;
        }
        private void Heal()
        {
            Debug.Log("Unit healed by : " + HpGainedByHealing.ToString());
            CurrentHealthPoints += HpGainedByHealing;
        }
        public void HealDistantUnit(Unit target)
        {
            var adjacentTile = gridController.FindAvailableAdjacentTile(target.CurrentTile, this);
            if (adjacentTile != null)
                MoveByAction(new Action(PrepareMove(adjacentTile), ActionType.Heal, target));
        }

        public bool TargetIsInMovementRange(Targetable target)
        {
            if (currentTile.IsWithinRange(target.CurrentTile, 1))
                return true;
            return gridController.FindAvailableAdjacentTile(target.CurrentTile, this) != null;
        }
        public bool TargetIsInRange(Targetable target)
        {
            if (target != null && currentTile != null)
                return currentTile.IsWithinRange(target.CurrentTile, AttackRange);
            return false;
        }
        #endregion
        
        public enum Gender
        {
            Male,
            Female,
            Mork
        }
    } 
}
