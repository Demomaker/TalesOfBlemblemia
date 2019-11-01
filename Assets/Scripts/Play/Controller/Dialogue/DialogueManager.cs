using System.Collections;
using System.Collections.Generic;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

//Author: Pierre-Luc Maltais et Jérémie Bertrand
//Parts of this code is taken from Brackeys youtube channel tutorial on how to make Dialogue System for Unity.
//https://www.youtube.com/watch?v=_nRzoTzeyxU
namespace Game
{
    [Findable("DialogueManager")]
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private Texture[] textures;
        [SerializeField] private Text nameText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private Animator animator;
        [SerializeField] private RawImage portrait;
        
        private bool skipTypingCoroutine;
        private bool isDisplayingDialogue;
        private bool isTyping;
        private Queue<Quote> sentences;
        private static readonly int IS_OPEN = Animator.StringToHash("IsOpen");

        public bool IsDisplayingDialogue => isDisplayingDialogue;

        public void StartDialogue(Dialogue dialogue)
        {
            isDisplayingDialogue = true;
            sentences = new Queue<Quote>();
            animator.SetBool(IS_OPEN,true);
            
            sentences.Clear();

            foreach (Quote sentence in dialogue.Sentences)
            {
                sentences.Enqueue(sentence);
            }
            
            DisplayNextSentence();
        }

        private void ChangePortrait(Texture texture)
        {
            portrait.texture = texture;
        }

        private void Update()
        {
            if (!isDisplayingDialogue) return;
            if (Input.anyKeyDown && !Input.GetKey(KeyCode.Escape))
            {
                if (isTyping) skipTypingCoroutine = true;
                else
                {
                    DisplayNextSentence();
                }
            }
        }

        [UsedImplicitly]
        public void DisplayNextSentence()
        {
            if(isTyping) return;
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }
            Quote quote = sentences.Dequeue();

            nameText.text = quote.Name;
            foreach (var texture in textures)
            {
                if (texture.name == quote.Name)
                {
                    ChangePortrait(texture);
                    break;
                }
            }
            StartCoroutine(TypeSentence(quote.Sentence));
        }

        IEnumerator TypeSentence(string sentence)
        {
            isTyping = true;
            dialogueText.text = "";
            foreach (char letter in sentence)
            {
                if (skipTypingCoroutine)
                {
                    dialogueText.text = sentence;
                    skipTypingCoroutine = false;
                    break;
                }
                dialogueText.text += letter;
                yield return null;
            }
            
            isTyping = false;
        }
        
        private void EndDialogue()
        {
            animator.SetBool(IS_OPEN,false);
            isDisplayingDialogue = false;
        }
    }
}

