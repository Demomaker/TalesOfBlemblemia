using System;
using UnityEngine;

namespace Game
{
    public class Animation
    {
        public Func<Boolean> ActivateCondition { get; }
        public float TimeInMillisecondsBeforeSpriteChange { get; }
        public GameObject GameObj { get; }
        public Sprite[] AnimationSprites { get; }
        public Func<Boolean> EndCondition { get; }
        public float TimeInMillisecondsBeforeAnimationEnd { get; }

        public Animation(
            Func<Boolean> activateCondition, 
            float timeInMillisecondsBeforeSpriteChange, 
            GameObject gameObj, 
            Sprite[] animationSprites, 
            Func<Boolean> endCondition = null, 
            float timeInMillisecondsBeforeAnimationEnd = 0
            )
        {
            ActivateCondition = activateCondition;
            TimeInMillisecondsBeforeSpriteChange = timeInMillisecondsBeforeSpriteChange;
            GameObj = gameObj;
            AnimationSprites = animationSprites;
            if (endCondition == null)
                endCondition = () => { return true; };
            EndCondition = endCondition;
            TimeInMillisecondsBeforeAnimationEnd = timeInMillisecondsBeforeAnimationEnd;
        }
    }
}