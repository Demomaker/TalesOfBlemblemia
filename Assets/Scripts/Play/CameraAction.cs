using UnityEngine;

namespace Play
{
    [System.Serializable]
    public class CameraAction
    {
        [SerializeField] private Vector2Int camTarget;
        [SerializeField] private float duration = 0;
        [SerializeField] private KeyCode keyToPress;
        [SerializeField] private Dialogue dialogue;
        public Vector2Int CamTarget => camTarget;
        public float Duration => duration;
        public KeyCode KeyToClick => keyToPress;
        public Dialogue Dialogue => dialogue;
    }
    
}