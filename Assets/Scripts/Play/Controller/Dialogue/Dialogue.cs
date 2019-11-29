using System.Collections.Generic;
using UnityEngine;

//Author: Pierre-Luc Maltais, Jérémie Bertrand
[System.Serializable]
public class Dialogue
{
    [SerializeField] private Quote[] sentences;
    public IEnumerable<Quote> Sentences => sentences;

    public Dialogue(Quote[] sentences)
    {
        this.sentences = sentences;
    }
    public Dialogue(Quote sentence)
    {
        sentences = new []{sentence};
    }

}
