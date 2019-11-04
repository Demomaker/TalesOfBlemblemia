using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class Cinematic : ScriptableObject
    {
        [SerializeField] private List<CameraAction> cameraActions;
        public IEnumerable<CameraAction> CameraActions => cameraActions;
    }
}