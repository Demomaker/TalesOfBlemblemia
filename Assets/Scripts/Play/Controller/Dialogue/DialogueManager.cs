using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//this code is taken from Brackeys youtube channel tutorial on how to make Dialogue System for Unity.
//https://www.youtube.com/watch?v=_nRzoTzeyxU
//The main difference is that i made the field private and serializable.
public class DialogueManager : MonoBehaviour
{

    [SerializeField] private Text nameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private Texture[] textures;

    private bool isDisplayingDialogue = false;
    public bool IsDisplayingDialogue => isDisplayingDialogue;
    private bool isTyping = false;
    
    private Queue<Quote> sentences;

    // Start is called before the first frame update

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
        
        //StopAllCoroutines();
        StartCoroutine(TypeSentence(quote.Sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
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
