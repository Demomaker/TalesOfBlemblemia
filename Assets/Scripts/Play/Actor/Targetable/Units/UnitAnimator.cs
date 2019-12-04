using System;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public class UnitAnimator : MonoBehaviour
    {
        private Animator unitAnimator;

        private void Awake()
        {
            unitAnimator = GetComponent<Animator>();
        }

        public void PlayAttackAnimation()
        {
            PlayAnimation(AnimationType.IS_ATTACKING);
        }

        public void StopAttackAnimation()
        {
            StopAnimation(AnimationType.IS_ATTACKING);
        }

        public void PlayMoveAnimation()
        {
            PlayAnimation(AnimationType.IS_MOVING);
        }

        public void StopMoveAnimation()
        {
            StopAnimation(AnimationType.IS_MOVING);
        }

        public void PlayDeathAnimation()
        {
           PlayAnimation(AnimationType.IS_GOING_TO_DIE); 
        }

        public void StopDeathAnimation()
        {
            StopAnimation(AnimationType.IS_GOING_TO_DIE);
        }

        public void PlayBlockAnimation()
        {
            PlayAnimation(AnimationType.IS_BLOCKING);
        }

        public void StopBlockAnimation()
        {
            StopAnimation(AnimationType.IS_BLOCKING);
        }

        public void PlayHurtAnimation()
        {
            PlayAnimation(AnimationType.IS_HURT);
        }

        public void StopHurtAnimation()
        {
            StopAnimation(AnimationType.IS_HURT);
        }

        private void PlayAnimation(AnimationType animationType)
        {
            unitAnimator?.SetBool(animationType.GetAnimationName(),true);
        }

        private void StopAnimation(AnimationType animationType)
        {
            unitAnimator?.SetBool(animationType.GetAnimationName(),false);
        }
    }
    
    public enum AnimationType
    {
            IS_MOVING,
            IS_ATTACKING,
            IS_GOING_TO_DIE,
            IS_HURT,
            IS_BLOCKING
    }
        
    public static class AnimationTypeExt
    {
        public static string GetAnimationName(this AnimationType animationType)
        {
            switch (animationType)
            {
                case AnimationType.IS_MOVING:
                    return "IsMoving";
                case AnimationType.IS_ATTACKING:
                    return "IsAttacking";
                case AnimationType.IS_GOING_TO_DIE:
                    return "IsGoingToDie";
                case AnimationType.IS_HURT:
                    return "IsBeingHurt";
                case AnimationType.IS_BLOCKING:
                    return "IsDodging";
                default:
                    throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
            }
        }
    }
}