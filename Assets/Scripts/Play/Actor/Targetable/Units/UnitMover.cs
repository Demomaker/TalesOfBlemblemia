using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    //Author : Zacharie Lavigne, Pierre-Luc Maltais (UI)
    public class UnitMover
    {
        private readonly UIController uiController;
        private readonly LevelController levelController;
        private readonly GameSettings gameSettings;
        private readonly Unit associatedUnit;

        public UnitMover(Unit associatedUnit, LevelController levelController, UIController uiController, GameSettings gameSettings)
        {
            this.uiController = uiController;
            this.gameSettings = gameSettings;
            this.levelController = levelController;
            this.associatedUnit = associatedUnit;
        }

        private void LookAt(Vector3 target)
        {
            var xModifier = 1;
            if (target.x < associatedUnit.Appearance.transform.position.x)
            {
                xModifier = -1;
            }
            var localScale = associatedUnit.Appearance.localScale;
            var scale = new Vector2(Math.Abs(localScale.x), localScale.y);
            scale.x *= xModifier;
            associatedUnit.Appearance.localScale = scale;
        }
        
        public List<Tile> PrepareMove(Tile targetTile, bool forArrow = true)
        {
            if (targetTile != associatedUnit.CurrentTile)
            {
                if (forArrow)
                {
                    associatedUnit.CurrentTile.UnlinkUnit();
                }
                var path = PathFinder.FindPath(associatedUnit.MovementCosts, associatedUnit.CurrentTile.LogicalPosition, targetTile.LogicalPosition, associatedUnit);
                path.Add(targetTile);
                return path;
            }
            return null;
        }
        
        public IEnumerator MoveByAction(Action action, float duration)
        {
            var path = action?.Path;
            if (path != null)
            {
                associatedUnit.IsMoving = true;
                associatedUnit.UnitAnimator?.PlayMoveAnimation();
                Tile finalTile = null;
                var pathCount = path.Count;
                for (int i = 0; i < pathCount; i++)
                {
                    if (path[i] != null)
                        finalTile = path[i];
                    float counter = 0;

                    if (finalTile != null)
                    {
                        if (path.IndexOf(finalTile) != pathCount - 1)
                            associatedUnit.MovesLeft -= finalTile.CostToMove;
                        var startPos = associatedUnit.Transform.position;
                        LookAt(finalTile.WorldPosition);

                        while (counter < duration)
                        {
                            counter += Time.deltaTime;

                            associatedUnit.Transform.position =
                                Vector3.Lerp(startPos, finalTile.WorldPosition, counter / duration);
                            yield return null;
                        }

                        if (associatedUnit.MovesLeft < 0 && path.IndexOf(finalTile) != pathCount - 1)
                        {
                            i = pathCount;
                        }
                    }
                }

                associatedUnit.OnUnitMove.Publish(associatedUnit);

                associatedUnit.CurrentTile = finalTile;
                if (associatedUnit.CurrentTile != null)
                    associatedUnit.Transform.position = associatedUnit.CurrentTile.WorldPosition;
                associatedUnit.IsMoving = false;
                associatedUnit.UnitAnimator?.StopMoveAnimation();
            }

            if (action != null)
            {
                if (action.ActionType != ActionType.Nothing)
                {
                    if (action.ActionType == ActionType.Attack && action.Target != null)
                    {
                        associatedUnit.OnAttack.Publish(associatedUnit);
                        if (associatedUnit.TargetIsInRange(action.Target))
                        {
                            Harmony.Finder.LevelController.BattleOngoing = true;
                            yield return associatedUnit.Attack(action.Target);
                            if (action.Target is Unit)
                                if (!Harmony.Finder.LevelController.CinematicController.IsPlayingACinematic)
                                {
                                    uiController.LaunchBattleReport(associatedUnit.IsEnemy);
                                    while (uiController.IsBattleReportActive) yield return null;
                                }
                            Harmony.Finder.LevelController.BattleOngoing = false;
                        }
                        else
                            associatedUnit.Rest();
                    }
                    else if (action.ActionType == ActionType.Recruit && action.Target != null)
                    {
                        associatedUnit.UnitAnimator?.PlayAttackAnimation();
                        if (action.Target.GetType() == typeof(Unit) && !associatedUnit.RecruitUnit((Unit) action.Target))
                            associatedUnit.Rest();
                        associatedUnit.UnitAnimator?.StopAttackAnimation();
                    }
                    else if (action.ActionType == ActionType.Heal && action.Target != null)
                    {
                        associatedUnit.UnitAnimator?.PlayAttackAnimation();
                        if (action.Target.GetType() == typeof(Unit) && !associatedUnit.HealUnit((Unit) action.Target))
                            associatedUnit.Rest();
                        associatedUnit.UnitAnimator?.StopAttackAnimation();
                    }
                    else
                    {
                        associatedUnit.Rest();
                    }
                }
            }
            else
            {
                associatedUnit.Rest();
            }
        }
        
        public IEnumerator Attack(Targetable target, bool isCountering, float duration)
        {
            if (associatedUnit.IsAttacking) yield break;
            associatedUnit.IsAttacking = true;
            associatedUnit.UnitAnimator?.PlayAttackAnimation();
            
            var counter = 0f;
            var startPos = associatedUnit.Transform.position;
            //2f to go only halfway
            var targetPos = (target.CurrentTile.WorldPosition + startPos) / 2f;
            LookAt(targetPos);
            duration /= 2;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                associatedUnit.Transform.position = Vector3.Lerp(startPos, targetPos, counter / duration);
                yield return null;
            }
            
            var hitRate = associatedUnit.Stats.HitRate - target.CurrentTile.DefenseRate;
            var damage = 0;
            var critModifier = 1;
            if (Random.value <= hitRate)
            {
                damage = associatedUnit.Stats.AttackStrength;
                if (target is Unit unit)
                {
                    associatedUnit.OnDodge.Publish(unit);
                    associatedUnit.UnitAnimator?.PlayBlockAnimation();
                }
                    
            }
            else if (target is Unit unit)
            {
                associatedUnit.OnHurt.Publish(unit);
                associatedUnit.UnitAnimator?.PlayHurtAnimation();
            }
            
            if (damage > 0 && !isCountering && !associatedUnit.IsImmuneToCrits && (target.GetType() == typeof(Unit) && (associatedUnit.CanCritOnEverybody || ((Unit)target).WeaponType == associatedUnit.WeaponAdvantage)))
            {
                critModifier = Random.value <= associatedUnit.Stats.CritRate ? 2 : 1;
                damage *= critModifier;
                if (critModifier > 1 && associatedUnit.CameraShake != null)
                {
                    associatedUnit.CameraShake.TriggerShake();
                }
            }
            
            target.CurrentHealthPoints -= damage;
            
            if (target is Unit)
                uiController.ChangeCharacterDamageTaken(damage, !associatedUnit.IsEnemy, critModifier);
            
            associatedUnit.UnitAnimator?.StopBlockAnimation();
            associatedUnit.UnitAnimator?.StopHurtAnimation();

            counter = 0;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                associatedUnit.Transform.position = Vector3.Lerp(targetPos, startPos, counter / duration);
                yield return null;
            }
            
            associatedUnit.Transform.position = startPos;
            associatedUnit.IsAttacking = false;
            associatedUnit.UnitAnimator?.StopAttackAnimation();

            //A unit cannot make a critical hit on a counter && cannot counter on a counter
            if (!target.NoHealthLeft && !isCountering && target is Unit targetUnit)
                yield return targetUnit.UnitMover.Attack(associatedUnit, true, gameSettings.AttackDuration);
            
            if (!isCountering)
                associatedUnit.HasActed = true;
        }
    }
}