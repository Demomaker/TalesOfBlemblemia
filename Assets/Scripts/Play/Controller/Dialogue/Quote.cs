using UnityEngine;
[System.Serializable]
public class Quote
{
    [SerializeField] private string name;
    [SerializeField] [TextArea(6,2)] private string sentence;
    public string Name => name;
    public string Sentence => sentence;
    
    public Quote(string name, string sentence)
    {
        this.name = name;
        this.sentence = sentence;
    }
}