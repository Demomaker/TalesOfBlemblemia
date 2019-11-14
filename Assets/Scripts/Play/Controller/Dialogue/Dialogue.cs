using UnityEngine;

//this code is taken from Brackeys youtube channel tutorial on how to make Dialogue System for Unity.
//https://www.youtube.com/watch?v=_nRzoTzeyxU
[System.Serializable]
public class Dialogue
{
    [SerializeField] private Quote[] sentences;
    public Quote[] Sentences => sentences;

    public Dialogue(Quote[] sentences)
    {
        this.sentences = sentences;
    }
    public Dialogue(Quote sentence)
    {
        this.sentences = new []{sentence};
    }

}
