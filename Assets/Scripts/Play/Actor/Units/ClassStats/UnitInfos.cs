using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class UnitInfos : ScriptableObject
    {
        //The name shown in the UI
        public string characterName;

        //The class shown in the UI
        public string className;
        
        //The weapon class shown in the ui
        public string weaponName;
    }
}