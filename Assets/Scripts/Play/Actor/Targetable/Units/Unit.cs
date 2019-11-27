using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    //Authors: Jérémie Bertrand, Zacharie Lavigne, Pierre-Luc Maltais (Ce qui touche au ui)
    public class Unit : Targetable
    {
        #region Serialized fields
        
        [SerializeField] private UnitInfos unitInfos;
        [SerializeField] private PlayerType playerType;
        [SerializeField] private UnitStats classStats;
        [SerializeField] private bool isImmuneToCrits;
        [SerializeField] private bool canCritOnEverybody;
        [SerializeField] private int detectionRadius;
        [SerializeField] private Transform appearance;
        
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
        private bool hasActed;
        private GameSettings gameSettings;
        private UIController uiController;

        /// <summary>
        /// Determines if an ai unit has been triggered by a player unit entering it's radius
        /// </summary>
        private bool isAwake;

        /// <summary>
        /// Array representing the movement cost needed to move to every tile on the grid
        /// </summary>
        private int[,] movementCosts;
        private int movesLeft;
        private int tileUpdateKeeper;
        private bool isMoving;
        private bool isAttacking;
        private bool isDodging;
        private bool isBeingHurt;
        private bool isResting;
        private bool isGoingToDie;
        private Animator animator;
        private OnHealthChange onHealthChange;
        private OnMovementChange onMovementChange;

        #endregion
        
        #region Properties
        
        public OnHealthChange OnHealthChange => onHealthChange == null ? onHealthChange = gameObject.AddOrGetComponent<OnHealthChange>() : onHealthChange;
        public OnMovementChange OnMovementChange => onMovementChange == null ? onMovementChange = gameObject.AddOrGetComponent<OnMovementChange>() : onMovementChange;
        
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
        public int DetectionRadius => detectionRadius;
        /// <summary>
        /// Once a unit is awake, it cannot go back to sleep
        /// </summary>
        public bool IsAwake
        {
            get => isAwake;
            set
            {
                if (isAwake != true)
                {
                    isAwake = value;
                }
            }
        }

        public bool IsMoving => isMoving;
        public bool IsAttacking => isAttacking;
        public int[,] MovementCosts
        {
            get
            {
                if (tileUpdateKeeper == Harmony.Finder.LevelController.LevelTileUpdateKeeper) return movementCosts;
                if (currentTile != null)
                    MovementCosts = PathFinder.ComputeCost(currentTile.LogicalPosition, IsEnemy);
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
        public bool IsRecruitable => playerType == PlayerType.Recruitable;
        public UnitStats Stats => classStats + weapon.WeaponStats;
        public WeaponType WeaponType => weapon.WeaponType;
        public WeaponType WeaponAdvantage => weapon.Advantage;

        public int MovesLeft
        {
            get => movesLeft;
            set
            {
                movesLeft = Mathf.Clamp(value, 0, Stats.MoveSpeed);
                OnMovementChange.Publish();
            }
        }

        public bool HasActed
        {
            get => hasActed;
            set
            {
                //if the character has now acted
                    if (!hasActed && value)
                    {
                       SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
                       foreach (var spriteRenderer in spriteRenderers)
                       {
                           spriteRenderer.color = gameSettings.PaleAlpha;
                       }
                    }
                    //if the character had previously acted but can now act
                    else if (hasActed && value == false)
                    {
                        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
                        foreach (var spriteRenderer in spriteRenderers)
                        {
                            spriteRenderer.color = gameSettings.OpaqueAlpha;
                        }
                    }
                    hasActed = value;
            }
        }
        public int AttackRange => 1;

        public UnitInfos UnitInfos => unitInfos;

        #endregion

        public override void Awake()
        {
            InitializeEvents();
            uiController = Harmony.Finder.UIController;
            weapon = GetComponentInParent<Weapon>();
            if (weapon == null)
                throw new Exception("A unit gameObject should have a weapon script");
            gridController = Finder.GridController;
            animator = GetComponent<Animator>();
            gameSettings = Harmony.Finder.GameSettings;
            base.Awake();
        }
        
        protected override void Start()
        {
            base.Start();
            CurrentHealthPoints = Stats.MaxHealthPoints;
            MovesLeft = Stats.MoveSpeed;
        }

        private void InitializeEvents()
        {
            onHurt = Harmony.Finder.OnHurt;
            onAttack = Harmony.Finder.OnAttack;
            onDodge = Harmony.Finder.OnDodge;
            onUnitMove = Harmony.Finder.OnUnitMove;
            onUnitDeath = Harmony.Finder.OnUnitDeath;
            onPlayerUnitLoss = Harmony.Finder.OnPlayerUnitLoss;
        }

        private void OnEnable()
        {
            EnableEvents();
        }

        private void EnableEvents()
        {
            onHurt.Notify += Hurt;
            onDodge.Notify += MakeDodge;
        }

        private void OnDisable()
        {
            DisableEvents();
        }

        private void DisableEvents()
        {
            onHurt.Notify -= Hurt;
            onDodge.Notify -= MakeDodge;
        }

        [UsedImplicitly]
        public void Hurt(Unit unit)
        {
            unit.SetIsBeingHurt(true);
        }

        [UsedImplicitly]
        public void MakeDodge(Unit unit)
        {
            unit.SetIsDodging(true);
        }

        [UsedImplicitly]
        public void SetIsBeingHurt(bool isBeingHurt)
        {
            this.isBeingHurt = isBeingHurt;
        }

        [UsedImplicitly]
        public void SetIsDodging(bool isDodging)
        {
            this.isDodging = isDodging;
        }

        private void FixedUpdate()
        {
            if (isMoving) isResting = false;
            if (animator == null) return;
            animator.SetBool(gameSettings.IsMoving, isMoving);
            animator.SetBool(gameSettings.IsAttacking, isAttacking);
            animator.SetBool(gameSettings.IsDodging, isDodging);
            animator.SetBool(gameSettings.IsBeingHurt, isBeingHurt);
            animator.SetBool(gameSettings.IsResting, isResting);
            animator.SetBool(gameSettings.IsGoingToDie, isGoingToDie);
        }
        
        public void ResetTurnStats()
        {
            HasActed = false;
            MovesLeft = Stats.MoveSpeed;
        }
        
        private void LookAt(Vector3 target)
        {
            int xModifier = 1;
            if (target.x < appearance.transform.position.x)
            {
                xModifier = -1;
            }
            var localScale = appearance.localScale;
            Vector2 scale = new Vector2(Math.Abs(localScale.x), localScale.y);
            scale.x *= xModifier;
            appearance.localScale = scale;
        }

        #region Movements
        public List<Tile> PrepareMove(Tile targetTile, bool forArrow = true)
        {
            if (targetTile != currentTile)
            {
                if (forArrow)
                {
                    currentTile.UnlinkUnit();
                    MovesLeft -= currentTile.CostToMove;
                }
                List<Tile> path = PathFinder.FindPath(gridController, MovementCosts, new List<Tile>(), currentTile.LogicalPosition, targetTile.LogicalPosition, this);
                path.RemoveAt(0);
                path.Add(targetTile);
                return path;
            }
            return null;
        }
        public Coroutine MoveByAction(Action action)
        {
            //TODO coroutine starter
            return Harmony.Finder.LevelController.StartCoroutine(MoveByAction(action, gameSettings.MovementDuration));
        }
        private IEnumerator MoveByAction(Action action, float duration)
        {
            var path = action?.Path;
            if (path != null)
            {
                isMoving = true;
                Tile finalTile = null;
                var pathCount = path.Count;
                for (int i = 0; i < pathCount; i++)
                {
                    if (path[i] != null)
                        finalTile = path[i];
                    float counter = 0;

                    if (path.IndexOf(finalTile) != pathCount - 1)
                        MovesLeft -= finalTile.CostToMove;
                    Vector3 startPos = transform.position;
                    LookAt(finalTile.WorldPosition);

                    while (counter < duration)
                    {
                        counter += Time.deltaTime;
                        
                        transform.position = Vector3.Lerp(startPos, finalTile.WorldPosition, counter / duration);
                        yield return null;
                    }

                    if (MovesLeft <= 0 && path.IndexOf(finalTile) != pathCount - 1)
                    {
                        i = pathCount;
                    }
                }
                
                onUnitMove.Publish(this);

                CurrentTile = finalTile;
                if (currentTile != null) transform.position = currentTile.WorldPosition;
                isMoving = false;
            }

            if (action != null)
            {
                if (action.ActionType != ActionType.Nothing)
                {
                    if (action.ActionType == ActionType.Attack && action.Target != null)
                    {
                        onAttack.Publish(this);
                        if (TargetIsInRange(action.Target))
                        {
                            yield return Attack(action.Target);
                            if (action.Target.GetType() == typeof(Unit))
                                if (!Harmony.Finder.LevelController.CinematicController.IsPlayingACinematic)
                                    yield return uiController.LaunchBattleReport(IsEnemy);
                                else
                                    yield break;
                        }
                        else
                            Rest();
                    }
                    else if (action.ActionType == ActionType.Recruit && action.Target != null)
                    {
                        if (action.Target.GetType() == typeof(Unit) && !RecruitUnit((Unit) action.Target))
                            Rest();
                    }
                    else if (action.ActionType == ActionType.Heal && action.Target != null)
                    {
                        if (action.Target.GetType() == typeof(Unit) && !HealUnit((Unit) action.Target))
                            Rest();
                    }
                    else
                    {
                        Rest();
                    }
                }
            }
            else
            {
                Rest();
            }
        }

        private bool hasDiedOnce = false;
        
        public override IEnumerator Die()
        {
            if (hasDiedOnce) yield break;
            hasDiedOnce = true;
            GetComponent<Cinematic>()?.TriggerCinematic();
            while (Harmony.Finder.LevelController.CinematicController.IsPlayingACinematic ||
                   Harmony.Finder.UIController.IsBattleReportActive)
            {
                yield return null;
            }

            isGoingToDie = true;
            onUnitDeath.Publish(this);
            if (playerType == PlayerType.Ally)
                onPlayerUnitLoss.Publish(this);
            isGoingToDie = false;
            yield return base.Die();
        }
        #endregion
        
        #region Action controlls
        public void Rest()
        {
            CurrentHealthPoints += HpGainedByResting;
            isResting = true;
            
            HasActed = true;
        }
        
        public Coroutine Attack(Targetable target, bool isCountering = false)
        {
            Coroutine AttackRoutineHandle;
            
            if(target.GetType() == typeof(Unit))
            {
                uiController.SetupCharactersBattleInfo(
                    this.Stats.MaxHealthPoints, 
                    this.CurrentHealthPoints,
                    ((Unit)target).classStats.MaxHealthPoints,
                    target.CurrentHealthPoints, 
                    IsEnemy
                );
            }
            //TODO créer un CouroutineStarter qui sera dans le finder qui remplacera le Level Controller de la ligne suivante
            AttackRoutineHandle = Harmony.Finder.LevelController.StartCoroutine(Attack(target, isCountering, gameSettings.AttackDuration));
            return AttackRoutineHandle;
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
            var critModifier = 1;
            if (Random.value <= hitRate)
            {
                damage = Stats.AttackStrength;
                if(target is Unit unit)
                    onDodge.Publish(unit);
            }
            else if (target is Unit unit)
            {
                onHurt.Publish(unit);
            }
            if (!isCountering && !isImmuneToCrits && (target.GetType() == typeof(Unit) && (canCritOnEverybody || ((Unit)target).WeaponType == WeaponAdvantage)))
            {
                critModifier = Random.value <= Stats.CritRate ? 2 : 1;
                damage *= critModifier;
            }
            
            target.CurrentHealthPoints -= damage;
            
            //todo Will have to check for Doors in the future.
            if (target is Unit)
                uiController.ChangeCharacterDamageTaken(damage, !IsEnemy, critModifier);
            counter = 0;
            
            while (counter < duration)
            {
                counter += Time.deltaTime;
                transform.position = Vector3.Lerp(targetPos, startPos, counter / duration);
                yield return null;
            }
            
            transform.position = startPos;
            isAttacking = false;
            //TODO verifier si cast est valide ((Unit)target).SetIsBeingHurt(false);
            //TODO verifier si cast est valide ((Unit)target).SetIsDodging(false);
            
            //A unit cannot make a critical hit on a counter
            //A unit cannot counter on a counter
            if (!target.NoHealthLeft && !isCountering && target is Unit targetUnit)
                yield return targetUnit.Attack(this, true, gameSettings.AttackDuration);
            
            if (!isCountering)
            {
                HasActed = true;
            }
        }
        
        public bool RecruitUnit()
        {
            if (IsRecruitable)
            {
                playerType = PlayerType.Ally;
                HumanPlayer.Instance.AddOwnedUnit(this);
                GetComponentInChildren<Cinematic>()?.TriggerCinematic();
                
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

        public bool TargetIsInMovementRange(Targetable target)
        {
            if (currentTile == null || target == null || target.CurrentTile == null)
                return false;
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

        public void RemoveInitialMovement()
        {
            MovesLeft -= currentTile.CostToMove;
        }
    } 
}
