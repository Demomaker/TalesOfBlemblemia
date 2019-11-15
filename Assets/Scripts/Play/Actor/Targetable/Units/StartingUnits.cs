using UnityEngine;

namespace Harmony
{
    [CreateAssetMenu(fileName = "New Starting Units", menuName = "Game/Starting Units")]
    public class StartingUnits : ScriptableObject
    {
        [SerializeField] private UnitData[] units;
    }
}