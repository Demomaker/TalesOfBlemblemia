using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Play
{
    [System.Serializable]
    public class CameraAction
    {
        [Header("Camera Values")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float duration = 0;
        [Range(4f, 8.5f)][SerializeField] private float cameraZoom = CinematicController.DEFAULT_CAMERA_ZOOM;

        [SerializeField] private Quote[] quotes;
        public Transform CameraTarget => cameraTarget;
        public float Duration => duration;
        public Dialogue Dialogue => new Dialogue(quotes);
        public float CameraZoom => cameraZoom;
    }
}