using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using Play;
using UnityEngine;
using Finder = Game.Finder;

public class CinematicController : MonoBehaviour
{
    private Camera camera = null;
    private Transform camTransform = null;
    private DialogueManager dialogueManager;

    [SerializeField] private Vector2Int initialPosition;
    [SerializeField] private CameraAction[] startActions;
    [SerializeField] private CameraAction[] endActions;

    private void Awake()
    {
        camera = Camera.main;
        camTransform = camera.gameObject.transform;
        camera.orthographicSize = 4;
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void Start()
    {
        StartCoroutine(Lerp());
    }

    private Vector3 GetWorldPosition(Vector2Int positionInGrid)
    {
        return Finder.GridController.GetTile(positionInGrid.x, positionInGrid.y).WorldPosition + new Vector3(0, 0, -10);
    }

    IEnumerator Lerp()
    {
        yield return new WaitForEndOfFrame();
        camera.transform.position = GetWorldPosition(initialPosition);
        for (int i = 0; i < startActions.Length; i++)
        {
            var target = startActions[i];
            var from = camTransform.position;
            var to = GetWorldPosition(target.CamTarget);
            var duration = target.Duration;
            for (float t = 0; t < duration; t += Time.deltaTime) {
                camTransform.position = Vector3.Lerp(from, to, t / duration);
                yield return null;
            }
            camTransform.position = to;
            dialogueManager.StartDialogue(target.Dialogue);
            while (!Input.GetKeyDown(target.KeyToClick))
            {
                yield return null;
            }
        }
        var startPos = camTransform.position;
        var startSize = camera.orthographicSize;
        var endSize = 8.5f;
        float dur = 0.5f;
        for (float t = 0; t < dur; t += Time.deltaTime) {
            camTransform.position = Vector3.Lerp(startPos, GetWorldPosition(new Vector2Int(7, 7)), t / dur);
            camera.orthographicSize = Mathf.Lerp(startSize, endSize, t / dur);
            yield return null;
        }
        camTransform.position = GetWorldPosition(new Vector2Int(7, 7));
        camera.orthographicSize = endSize;
    }
}
