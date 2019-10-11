using UnityEditor;
using UnityEngine;
[System.Serializable]
public class Quote
{
    [SerializeField] private string name;
    [SerializeField] [TextArea(3,2)] private string sentence;
    public string Name => name;
    public string Sentence => sentence;
}