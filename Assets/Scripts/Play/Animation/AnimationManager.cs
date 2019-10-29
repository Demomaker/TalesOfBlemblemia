using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class AnimationManager : MonoBehaviour
    {
        
        private List<Animation> animations = new List<Animation>();

        public List<Animation> Animations => animations;

        private void Update()
        {
            foreach (Animation animation in animations)
            {
                if (animation.ActivateCondition())
                {
                    StartCoroutine(Animate(animation));
                }
            }
        }
        
        private IEnumerator Animate(Animation animation)
        {
            Sprite currentSprite = animation.GameObj.GetComponent<Sprite>();
            DateTime timeBeforeAnimationStart = DateTime.Now;
            while (!animation.EndCondition.Invoke() && (DateTime.Now.Millisecond - timeBeforeAnimationStart.Millisecond) < animation.TimeInMillisecondsBeforeAnimationEnd)
            {
                yield return animation.TimeInMillisecondsBeforeSpriteChange;
                yield return ChangeSpriteToNext(currentSprite, animation.AnimationSprites);
            }
        }

        private bool ChangeSpriteToNext(Sprite sprite, Sprite[] animationSprites)
        {
            int spriteIndex = Array.IndexOf(animationSprites, sprite);
            if (spriteIndex >= animationSprites.Length) spriteIndex = 0;
            sprite = animationSprites[spriteIndex];
            return true;
        }
    }
}