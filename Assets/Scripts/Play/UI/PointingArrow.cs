using System;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Makes an arrow point to a specific transform to indicate a place to go to or click
    /// Author : Mike Bédard
    /// </summary>
    public class PointingArrow : MonoBehaviour
    {
        [SerializeField] private Unit unitToAttack;
        [SerializeField] private Unit unitToMove;
        [SerializeField] private Vector3 specificUnitMovePosition = nullVector3;
        [SerializeField] private float maxYPositionDifference = 0.5f;
        [SerializeField] private float yPositionChange = 0.01f;
        [SerializeField] private YDirection startingYDirection = YDirection.Up;
        [SerializeField] private Transform transformToPoint;
        [SerializeField] private PointingArrow nextArrowToActivate = null;
        [SerializeField] private Cinematic cinematicToTriggerOnStop = null;
        [SerializeField] private Cinematic cinematicToTriggerOnStart = null;

        private static readonly Vector3 nullVector3 = new Vector3(-1, -1, -1);
        private float yPositionDifference = 0;
        private YDirection yDirection;
        private OnHurt onHurt;
        private OnDodge onDodge;
        private OnUnitDeath onUnitDeath;
        private OnUnitMove onUnitMove;
        private Vector3 transformToPointPosition;

        private void Awake()
        {
            yDirection = startingYDirection;
            onHurt = Harmony.Finder.OnHurt;
            onDodge = Harmony.Finder.OnDodge;
            onUnitMove = Harmony.Finder.OnUnitMove;
            onUnitDeath = Harmony.Finder.OnUnitDeath;
            if (transformToPoint != null)
                transformToPointPosition = transformToPoint.position;
        }

        private void OnEnable()
        {
            if (cinematicToTriggerOnStart != null)
                cinematicToTriggerOnStart.TriggerCinematic();
            onHurt.Notify += UnitToAttackWasAttacked;
            onDodge.Notify += UnitToAttackWasAttacked;
            onUnitDeath.Notify += UnitToAttackWasAttacked;
            onUnitMove.Notify += UnitToMoveWasMoved;

        }

        private void OnDisable()
        {
            if (nextArrowToActivate == null || !nextArrowToActivate.gameObject.activeSelf)
                StopPointing();
            onHurt.Notify -= UnitToAttackWasAttacked;
            onDodge.Notify -= UnitToAttackWasAttacked;
            onUnitDeath.Notify -= UnitToAttackWasAttacked;
            onUnitMove.Notify -= UnitToMoveWasMoved;
        }

        private void Update()
        {
            DoPointingMovement();
        }

        public void OnStopEvent()
        {
            if (gameObject.activeSelf)
                StopPointing();
        }

        public void SetTransformToPointPosition(Vector3 position)
        {
            transformToPointPosition = position;
        }

        private void UnitToAttackWasAttacked(Unit unit)
        {
            if (unitToAttack != null && unit == unitToAttack)
                StopPointing();
        }

        private void UnitToMoveWasMoved(Unit unit)
        {
            if (unitToMove != null && unit == unitToMove)
            {
                if (specificUnitMovePosition == nullVector3)
                    StopPointing();
                if (specificUnitMovePosition != nullVector3 && unit.transform.position == specificUnitMovePosition)
                    StopPointing();
            }
        }
        
        private void StopPointing()
        {
            if (nextArrowToActivate != null)
                nextArrowToActivate.gameObject.SetActive(true);
            if (cinematicToTriggerOnStop != null)
                cinematicToTriggerOnStop.TriggerCinematic();
            gameObject.SetActive(false);
        }

        private void DoPointingMovement()
        {
            switch (yDirection)
            {
                case YDirection.Up:
                    yPositionDifference += yPositionChange;
                    break;
                case YDirection.Down:
                    yPositionDifference -= yPositionChange;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (yPositionDifference >= maxYPositionDifference)
            {
                yDirection = YDirection.Down;
            }

            if (yPositionDifference <= -maxYPositionDifference)
            {
                yDirection = YDirection.Up;
            }

            if (transformToPointPosition != nullVector3)
                transform.position = transformToPointPosition +
                                     new Vector3(0, GetComponent<SpriteRenderer>().size.y, 0) +
                                     transform.up * yPositionDifference;
        }
        
        /// <summary>
        /// Enum to determine if something is going up or down
        /// </summary>
        private enum YDirection
        {
            Up,
            Down
        }
    }
}