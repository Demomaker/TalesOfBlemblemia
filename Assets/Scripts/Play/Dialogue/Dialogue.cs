using UnityEngine;


//this code is taken from Brackeys youtube channel tutorial on how to make Dialogue System for Unity.
//https://www.youtube.com/watch?v=_nRzoTzeyxU
[System.Serializable]
public class Dialogue
{
    [TextArea(3,2)]
    public string[] sentences;

    public Texture[] texture;

}
