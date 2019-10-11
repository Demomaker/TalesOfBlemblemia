using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Harmony
{
    [CreateAssetMenu(fileName = "New Unit Data", menuName = "Game/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private int hp;
        [SerializeField] private int age;
    }
}