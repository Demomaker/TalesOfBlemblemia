using UnityEngine;
using static Game.Constants;

namespace Game
{
    // Authors: Jérémie Bertrand
    [System.Serializable]
    public class CameraAction
    {
        [Header("Camera Values")]
        [SerializeField] private Transform cameraTarget;
        
        [Range(MIN_CINEMATIC_TIME, MAX_CINEMATIC_TIME)][SerializeField] private float duration;
        
        [Range(MIN_CAM_ORTHOGRAPHIC_SIZE, MAX_CAM_ORTHOGRAPHIC_SIZE)][SerializeField] private float cameraZoom;
        
        [SerializeField] private Quote[] quotes;
        
        public Transform CameraTarget => cameraTarget;
        public float Duration => duration;
        public float CameraZoom => cameraZoom;
        public Dialogue Dialogue => new Dialogue(quotes);
        public bool HasADialog => quotes.Length > 0;
    }
}