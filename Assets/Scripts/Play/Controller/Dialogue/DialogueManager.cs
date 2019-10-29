using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;
using UnityEngine.UI;

//Parts of this code is taken from Brackeys youtube channel tutorial on how to make Dialogue System for Unity.
//https://www.youtube.com/watch?v=_nRzoTzeyxU
namespace Game
{
    [Findable("DialogueManager")]
    public class DialogueManager : MonoBehaviour
    {

        /*[SerializeField] private Text nameText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private Animator animator;*/
        [SerializeField] private Texture[] textures;

        private bool skipTypingCoroutine = false;
        private bool isDisplayingDialogue = false;
        private bool isTyping = false;
        private Queue<Quote> sentences;
        private Text nameText;
        private Text dialogueText;
        private Animator animator;

        public void Start()
        {
            try
            {
                nameText = GameObject.Find("DialogueName").GetComponent<Text>();
                dialogueText = GameObject.Find("DialogueText").GetComponent<Text>();
                animator = GameObject.Find("DialogueBox").GetComponent<Animator>();
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't find the component");
            }
            
        }

        public bool IsDisplayingDialogue => isDisplayingDialogue;

        public void StartDialogue(Dialogue dialogue)
        {
            isDisplayingDialogue = true;
            sentences = new Queue<Quote>();
            animator.SetBool("IsOpen",true);
            
            sentences.Clear();

            foreach (Quote sentence in dialogue.Sentences)
            {
                sentences.Enqueue(sentence);
            }
            
            DisplayNextSentence();
        }

        private void ChangePortrait(Texture texture)
        {
            RawImage portrait = GameObject.FindWithTag("UiPortrait").GetComponent<RawImage>();
            portrait.texture = texture;
        }

        private void Update()
        {
            if (!isDisplayingDialogue) return;
            if (Input.anyKeyDown)
            {
                if (isTyping) skipTypingCoroutine = true;
                else
                {
                    DisplayNextSentence();
                }
            }
        }

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
            animator.SetBool("IsOpen",false);
            isDisplayingDialogue = false;
        }
    }
}

