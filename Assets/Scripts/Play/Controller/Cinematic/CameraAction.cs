using UnityEngine;
using static Game.CameraConstants;
using Vector3 = System.Numerics.Vector3;

namespace Game
{
    // Authors: Jérémie Bertrand
    [System.Serializable]
    public class CameraAction
    {
        [SerializeField] private Vector2 cameraTarget;
        [Range(MIN_CINEMATIC_TIME, MAX_CINEMATIC_TIME)][SerializeField] private float duration = MIN_CINEMATIC_TIME;
        
        [Range(MIN_CAM_ORTHOGRAPHIC_SIZE, MAX_CAM_ORTHOGRAPHIC_SIZE)][SerializeField] private float cameraZoom = MAX_CAM_ORTHOGRAPHIC_SIZE;
        
        [SerializeField] private Quote[] quotes;
        
        public Vector2 CameraTarget => cameraTarget;
        public float Duration => duration;
        public float CameraZoom => cameraZoom;
        public Dialogue Dialogue => new Dialogue(quotes);
        public bool HasADialog => quotes.Length > 0;
    }
}