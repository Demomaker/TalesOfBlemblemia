using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    // Authors: Jérémie Bertrand
    public class CinematicController : MonoBehaviour
    {
        private Camera camera;
        private DialogueManager dialogueManager;
        private bool isPlayingACinematic;
        private CameraController cameraController;
        private Transform camTransform;
        private GameObject dialogueUI;

        public bool IsPlayingACinematic
        {
            get => isPlayingACinematic;
            private set 
            { 
                isPlayingACinematic = value;
                if (isPlayingACinematic)
                {
                    cameraController.DisableControls();
                }
                else
                {
                    cameraController.EnableControls();
                }
            }
        }

        private void Awake()
        {
            dialogueUI = GameObject.FindWithTag("DialogueUi");
            camera = Camera.main;
            cameraController = camera.GetComponent<CameraController>();
            camTransform = camera.transform;
            dialogueManager = Harmony.Finder.DialogueManager;
        }

        private void Start()
        {
            if (dialogueUI != null) dialogueUI.SetActive(true);
        }

        private IEnumerator PlayCameraActions(IEnumerable<CameraAction> cameraActions)
        {
            IsPlayingACinematic = true;

            foreach (var cameraAction in cameraActions)
            {
                //BR : Remplacer ceci par cela :
                //        yield return PlayCameraAction(cameraAction);
                //     Ça fait la même chose de manière plus efficace.
                //
                //     Même chose ailleurs dans le code.
                yield return StartCoroutine(PlayCameraAction(cameraAction));
            }

            IsPlayingACinematic = false;
        }


        private IEnumerator PlayCameraAction(CameraAction cameraAction)
        {
            if (cameraAction.CameraTarget == null) yield break;

            var startPosition = camTransform.position;
            var endPosition = new Vector3(cameraAction.CameraTarget.position.x, cameraAction.CameraTarget.position.y, startPosition.z);
            var startZoom = camera.orthographicSize;
            var endZoom = cameraAction.CameraZoom;
            var duration = cameraAction.Duration;

            for (float elapsedTime = 0; elapsedTime < duration; elapsedTime += Time.deltaTime)
            {
                camTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
                camera.orthographicSize = Mathf.Lerp(startZoom, endZoom, elapsedTime / duration);
                yield return null;
            }

            camTransform.position = endPosition;
            camera.orthographicSize = endZoom;

            if (cameraAction.HasADialog)
            {
                yield return StartCoroutine(StartCinematicDialogue(cameraAction.Dialogue));
            }
        }

        private IEnumerator StartCinematicDialogue(Dialogue dialogue)
        {
            dialogueManager.StartDialogue(dialogue);
            while (dialogueManager.IsDisplayingDialogue)
            {
                yield return null;
            }
        }

        public void LaunchCinematic(Cinematic cinematic)
        {
            StartCoroutine(PlayCameraActions(cinematic.CameraActions));
        }
    }
}
