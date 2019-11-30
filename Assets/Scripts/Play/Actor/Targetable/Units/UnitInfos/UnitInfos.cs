using UnityEngine;

namespace Game
{
    //Author: Pierre-Luc Maltais, Jérémie Bertrand
    [CreateAssetMenu]
    public class UnitInfos : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private UnitGender gender;

        public string CharacterName => characterName;
        public UnitGender Gender => gender;
    }
}