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
        #region Serialized fields
        [SerializeField] private PlayerType playerType;
        [SerializeField] private UnitStats classStats;
        [SerializeField] private UnitGender gender;
        #endregion
        
        #region Fields
        private OnHurt onHurt;
        private OnAttack onAttack;
        private OnDodge onDodge;
        private OnUnitMove onUnitMove;
        private OnUnitDeath onUnitDeath;
        private OnPlayerUnitLoss onPlayerUnitLoss;
        private GridController gridController;
        private Weapon weapon;
        private int[,] movementCosts;
        private int movesLeft;
        private int tileUpdateKeeper;
        private bool isMoving = false;
        private bool isAttacking = false;
        private bool isDodging = false;
        private bool isBeingHurt = false;
        private bool isResting = false;
        private bool isGoingToDie = false;
        private Animator animator;
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

        public UnitGender Gender => gender;

        #endregion
        
        private void Awake()
        {
            weapon = GetComponentInParent<Weapon>();
            if (weapon == null)
                throw new Exception("A unit gameObject should have a weapon script");
            gridController = Finder.GridController;
            CurrentHealthPoints = Stats.MaxHealthPoints;
            movesLeft = Stats.MoveSpeed;
            onHurt = new OnHurt();
            onAttack = new OnAttack();
            onDodge = new OnDodge();
            onUnitMove = new OnUnitMove();
            onUnitDeath = new OnUnitDeath();
            onPlayerUnitLoss = new OnPlayerUnitLoss();
            animator = GetComponent<Animator>();

        }

        private void OnEnable()
        {
            OnHurt.Notify += Hurt;
            OnDodge.Notify += MakeDodge;
        }

        private void OnDisable()
        {
            OnHurt.Notify -= Hurt;
            OnDodge.Notify -= MakeDodge;
        }

        public void Hurt(Unit unit)
        {
            unit.SetIsBeingHurt(true);
        }

        public void MakeDodge(Unit unit)
        {
            unit.SetIsDodging(true);
        }

        public void SetIsBeingHurt(bool isBeingHurt)
        {
            this.isBeingHurt = isBeingHurt;
        }

        public void SetIsDodging(bool isDodging)
        {
            this.isDodging = isDodging;
        }

        private void FixedUpdate()
        {
            if (isMoving) isResting = false;
            if (animator == null) return;
            animator.SetBool(Constants.AnimationProperties.IS_MOVING, isMoving);
            animator.SetBool(Constants.AnimationProperties.IS_ATTACKING, isAttacking);
            animator.SetBool(Constants.AnimationProperties.IS_DODGING, isDodging);
            animator.SetBool(Constants.AnimationProperties.IS_BEING_HURT, isBeingHurt);
            animator.SetBool(Constants.AnimationProperties.IS_RESTING, isResting);
            animator.SetBool(Constants.AnimationProperties.IS_GOING_TO_DIE, isGoingToDie);
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
            if (path != null)
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
                
                onUnitMove.Publish(this);

                CurrentTile = finalTile;
                if (currentTile != null) transform.position = currentTile.WorldPosition;
                isMoving = false;
            }

            if (action.ActionType != ActionType.Nothing)
            {
                if (action.ActionType == ActionType.Attack && action.Target != null)
                {
                    onAttack.Publish(this);
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
            isGoingToDie = true;
            onUnitDeath.Publish(this);
            if(playerType == PlayerType.Ally)
                onPlayerUnitLoss.Publish(this);
            isGoingToDie = false;
            base.Die();
        }
        #endregion
        
        #region Action controlls
        public void Rest()
        {
            CurrentHealthPoints += HpGainedByResting;
            isResting = true;
            
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
            
            float hitRate = Stats.HitRate - target.CurrentTile.DefenseRate;
            int damage = 0;
            if (Random.value <= hitRate)
            {
                damage = Stats.AttackStrength;
                onDodge.Publish((Unit)target);
            }
            else
            {
                onHurt.Publish((Unit)target);
            }
            if (!isCountering && ((Unit)target).WeaponType == WeaponAdvantage)
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
            ((Unit)target).SetIsBeingHurt(false);
            ((Unit)target).SetIsDodging(false);
            
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
        
    } 
}
