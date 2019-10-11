using System.Collections;
using Play;
using UnityEngine;

public class CinematicController : MonoBehaviour
{
    public const float DEFAULT_CAMERA_ZOOM = 8.5f;
    public const float DEFAULT_CAMERA_Z_POSITION = -10;
    private Camera camera = null;
    private Transform camTransform = null;
    private DialogueManager dialogueManager;
    private bool isPlayingACutScene = false;

    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private Transform initialPosition;
    [SerializeField] private CameraAction[] startActions;
    [SerializeField] private CameraAction[] endActions;
    public bool IsPlayingACutScene => isPlayingACutScene;

    private void Awake()
    {
        camera = Camera.main;
        camTransform = camera.gameObject.transform;
        camTransform.position = initialPosition.position + new Vector3(0,0,DEFAULT_CAMERA_Z_POSITION);
        camera.orthographicSize = 4;
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void Start()
    {
        if (dialogueUI != null) dialogueUI.SetActive(true);
        StartCoroutine(PlayCameraActions(startActions));
    }

    private IEnumerator PlayCameraActions(CameraAction[] cameraActions)
    {
        isPlayingACutScene = true;
        for (int i = 0; i < cameraActions.Length; i++)
        {
            var target = cameraActions[i];
            var from = camTransform.position;
            var to = target.CameraTarget.position + new Vector3(0, 0, DEFAULT_CAMERA_Z_POSITION);
            var duration = target.Duration;
            
            for (float t = 0; t < duration; t += Time.deltaTime) {
                camTransform.position = Vector3.Lerp(from, to, t / duration);
                yield return null;
            }
            
            camTransform.position = to;
            dialogueManager.StartDialogue(target.Dialogue);
            while (dialogueManager.IsDisplayingDialogue)
            {
                yield return null;
            }
        }
        var startPos = camTransform.position;
        var startSize = camera.orthographicSize;
        var endSize = DEFAULT_CAMERA_ZOOM;
        float dur = 0.5f;
        for (float t = 0; t < dur; t += Time.deltaTime) {
            camTransform.position = Vector3.Lerp(startPos,  new Vector3(0,0, DEFAULT_CAMERA_Z_POSITION), t / dur);
            camera.orthographicSize = Mathf.Lerp(startSize, endSize, t / dur);
            yield return null;
        }

        camTransform.position = new Vector3(0, 0, DEFAULT_CAMERA_Z_POSITION);
        camera.orthographicSize = endSize;

        isPlayingACutScene = false;
    }

    public void LaunchCinematic(CameraAction[] actions)
    {
        StartCoroutine(PlayCameraActions(actions));
    }

    public void LaunchEndCinematic()
    {
        StartCoroutine(PlayCameraActions(endActions));
    }
}
