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
    
    private Queue<string> sentences;
    private Texture[] textures;

    // Start is called before the first frame update

    public void StartDialogue(Dialogue dialogue)
    {
        sentences = new Queue<string>();
        animator.SetBool("IsOpen",true);
        
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        textures = dialogue.texture;
        DisplayNextSentence();
    }

    private void ChangePortrait(Texture texture)
    {
        RawImage portrait = GameObject.FindWithTag("UiPortrait").GetComponent<RawImage>();
        portrait.texture = texture;
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();

        string[] infos = sentence.Split(':');
        
        nameText.text = infos[0];
        foreach (var texture in textures)
        {
            if (texture.name == infos[0])
            {
                ChangePortrait(texture);
                break;
            }
        }
        
        StopAllCoroutines();
        StartCoroutine(TypeSentence(infos[1]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return null;
        }
    }
    
    private void EndDialogue()
    {
        animator.SetBool("IsOpen",false);
    }

  
}
