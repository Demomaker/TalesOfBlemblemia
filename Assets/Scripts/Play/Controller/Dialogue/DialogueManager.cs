using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Authors: Pierre-Luc Maltais, Jérémie Bertrand
namespace Game
{
    [Findable("DialogueManager")]
    public class DialogueManager : MonoBehaviour
    {
        private static readonly int IS_OPEN = Animator.StringToHash("IsOpen");
        
        [SerializeField] private Texture[] textures;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Animator animator;
        [SerializeField] private RawImage portrait;
        
        private bool skipTypingCoroutine;
        private bool isDisplayingDialogue;
        private bool isTyping;
        private Queue<Quote> sentences;
        private CoroutineStarter coroutineStarter;
        
        public bool IsDisplayingDialogue => isDisplayingDialogue;

        private void Awake()
        {
            coroutineStarter = Harmony.Finder.CoroutineStarter;
        }

        public void StartDialogue(Dialogue dialogue)
        {
            isDisplayingDialogue = true;
            sentences = new Queue<Quote>();
            animator.SetBool(IS_OPEN,true);
            
            sentences.Clear();

            foreach (var sentence in dialogue.Sentences)
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
                    DisplayNextSentence();
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
            var quote = sentences.Dequeue();

            nameText.text = quote.Name;
            foreach (var texture in textures)
            {
                if (texture.name == quote.Name)
                {
                    ChangePortrait(texture);
                    break;
                }
            }
            coroutineStarter.StartCoroutine(TypeSentence(quote.Sentence));
        }

        private IEnumerator TypeSentence(string sentence)
        {
            isTyping = true;
            dialogueText.text = "";
            foreach (var letter in sentence)
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

