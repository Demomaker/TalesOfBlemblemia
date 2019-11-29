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
        [SerializeField] private UnitInfos unitInfos;
        [SerializeField] private Transform appearance;
        [SerializeField] private UnitStats classStats;
        [SerializeField] private PlayerType playerType;
        [SerializeField] private bool isImmuneToCrits;
        [SerializeField] private bool canCritOnEverybody;
        [SerializeField] private int detectionRadius;
        
        private OnHurt onHurt;
        private OnAttack onAttack;
        private OnDodge onDodge;
        private OnUnitMove onUnitMove;
        private OnUnitDeath onUnitDeath;
        private OnPlayerUnitLoss onPlayerUnitLoss;
        private GridController gridController;
        private OnHealthChange onHealthChange;
        private OnMovementChange onMovementChange;
        private GameSettings gameSettings;
        private UIController uiController;
        private LevelController levelController;
        private CoroutineStarter coroutineStarter;
        private Weapon weapon;
        private CameraShake cameraShake;
        private Animator animator;
        private UnitMover unitMover;
        private bool hasActed;
        private bool hasDiedOnce;
        private bool isAwake;
        private bool isMoving;
        private bool isAttacking;
        private bool isDodging;
        private bool isBeingHurt;
        private bool isResting;
        private bool isGoingToDie;
        private int[,] movementCosts;
        private int movesLeft;
        private int tileUpdateKeeper;

        public OnUnitMove OnUnitMove => onUnitMove;
        public OnAttack OnAttack => onAttack;
        public OnDodge OnDodge => onDodge;
        public OnHurt OnHurt => onHurt;
        public OnHealthChange OnHealthChange => onHealthChange == null ? onHealthChange = gameObject.AddOrGetComponent<OnHealthChange>() : onHealthChange;
        public OnMovementChange OnMovementChange => onMovementChange == null ? onMovementChange = gameObject.AddOrGetComponent<OnMovementChange>() : onMovementChange;
        public UnitStats Stats => classStats + weapon.WeaponStats;
        public UnitInfos UnitInfos => unitInfos;
        public Transform Transform => transform;
        public CameraShake CameraShake => cameraShake;
        public UnitMover UnitMover => unitMover;
        public Transform Appearance => appearance;
        public WeaponType WeaponType => weapon.WeaponType;
        public WeaponType WeaponAdvantage => weapon.Advantage;
        public bool IsAwake
        {
            get => isAwake;
            set
            {
                //Once awake, the unit cannot go back to sleep
                if (isAwake != true)
                {
                    isAwake = value;
                }
            }
        }
        public bool IsMoving
        {
            get => isMoving;
            set => isMoving = value;
        }
        public bool IsAttacking
        {
            get => isAttacking;
            set => isAttacking = value;
        }
        public bool IsEnemy => playerType == PlayerType.Enemy;
        public bool IsPlayer => playerType == PlayerType.Ally;
        public bool IsRecruitable => playerType == PlayerType.Recruitable;
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
        public bool IsImmuneToCrits => isImmuneToCrits;
        public bool CanCritOnEverybody => canCritOnEverybody;
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
        private int HpGainedByHealing
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
        public int MovesLeft
        {
            get => movesLeft;
            set
            {
                movesLeft = value;
                OnMovementChange.Publish();
            }
        }
        private int AttackRange => 1;
        protected override void Awake()
        {
            onHurt = Harmony.Finder.OnHurt;
            onAttack = Harmony.Finder.OnAttack;
            onDodge = Harmony.Finder.OnDodge;
            onUnitMove = Harmony.Finder.OnUnitMove;
            onUnitDeath = Harmony.Finder.OnUnitDeath;
            onPlayerUnitLoss = Harmony.Finder.OnPlayerUnitLoss;
            levelController = Harmony.Finder.LevelController;
            coroutineStarter = Harmony.Finder.CoroutineStarter;
            uiController = Harmony.Finder.UIController;

            if (Camera.main != null) cameraShake = Camera.main.GetComponent<CameraShake>();
            weapon = GetComponentInParent<Weapon>();
            if (weapon == null)
                throw new Exception("A unit gameObject should have a weapon script");
            gridController = Harmony.Finder.GridController;
            animator = GetComponent<Animator>();
            gameSettings = Harmony.Finder.GameSettings;
            unitMover = new UnitMover(this, levelController, uiController, gameSettings);
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
            CurrentHealthPoints = Stats.MaxHealthPoints;
            MovesLeft = Stats.MoveSpeed;
        }
        private void OnEnable()
        {
            onHurt.Notify += Hurt;
            onDodge.Notify += MakeDodge;
        }
        private void OnDisable()
        {
            onHurt.Notify -= Hurt;
            onDodge.Notify -= MakeDodge;
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

        
        public Coroutine MoveByAction(Action action)
        {
            return coroutineStarter.StartCoroutine(unitMover.MoveByAction(action, gameSettings.MovementDuration));
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
            AttackRoutineHandle = coroutineStarter.StartCoroutine(unitMover.Attack(target, isCountering, gameSettings.AttackDuration));
            return AttackRoutineHandle;
        }


        protected override IEnumerator Die()
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


        public bool RecruitUnit(Unit unitToRecruit)
        {
            if (TargetIsInRange(unitToRecruit))
            {
                return unitToRecruit.RecruitAdjacentUnit();
            }
            return false;
        }
        
        private bool RecruitAdjacentUnit()
        {
            if (IsRecruitable)
            {
                playerType = PlayerType.Ally;
                levelController.HumanPlayer.AddOwnedUnit(this);
                GetComponentInChildren<Cinematic>()?.TriggerCinematic();
                
            }
            return IsRecruitable;
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
        
        private void Heal()
        {
            CurrentHealthPoints += HpGainedByHealing;
        }
        
        public void Rest()
        {
            CurrentHealthPoints += HpGainedByResting;
            isResting = true;
            
            HasActed = true;
        }
        
        
        public void ResetAlpha()
        {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = gameSettings.OpaqueAlpha;
            }
        }
        
        public void ResetTurnStats()
        {
            HasActed = false;
            MovesLeft = Stats.MoveSpeed;
        }
    } 
}