
using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class OverHeadHpController : MonoBehaviour
    {
        [SerializeField] private Sprite[] numbersSprites;

        private SpriteRenderer number1;
        private SpriteRenderer number2;

        public void Awake()
        {
            try
            {
                SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
                number1 = spriteRenderers[1];
                number2 = spriteRenderers[2];
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            
        }
        
        public void ModifyOverHeadHp(int hp)
        {
            if (hp < 10)
            {
                number1.sprite = numbersSprites[0];
                number2.sprite = numbersSprites[hp];
            }
            else
            {
                char[] numbers = hp.ToString().ToCharArray();
                number1.sprite = numbersSprites[numbers[0]];
                number2.sprite = numbersSprites[numbers[1]];
            }
        }
    }
}

