﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{    
    //Author: Jérémie Bertrand
    public class CinematicController : MonoBehaviour
    {
        private Camera mainCamera;
        private DialogueManager dialogueManager;
        private CameraController cameraController;
        private GameObject uiController;
        
        private bool isPlayingACinematic;

        public bool IsPlayingACinematic
        {
            get => isPlayingACinematic;
            private set 
            { 
                isPlayingACinematic = value;
                if (isPlayingACinematic)
                {
                    if (cameraController != null) cameraController.DisableControls();
                    if (uiController != null) uiController.SetActive(false);
                }
                else
                {
                    if (cameraController != null) cameraController.EnableControls();
                    if (uiController != null) uiController.SetActive(true);
                }
            }
        }
        private void Awake()
        {
            uiController = Harmony.Finder.UIController.gameObject;
            dialogueManager = Harmony.Finder.DialogueManager;
            mainCamera = Camera.main;
            if (mainCamera != null)
                cameraController = mainCamera.GetComponent<CameraController>();
            else
                Debug.LogError(gameObject.name + ": No main camera in the current scene. (CinematicController.cs)");
        }
        
        private IEnumerator PlayCameraActions(IEnumerable<CinematicAction> actions)
        {
            IsPlayingACinematic = true;
            foreach (var action in actions)
            {
                yield return PlayCameraAction(action);
            }
            IsPlayingACinematic = false;
        }
        
        private IEnumerator PlayCameraAction(CinematicAction action)
        {
            switch (action.CinematicActionType)
            {
                case CinematicActionType.Damage:
                    yield return PlayDamage(action.TargetUnit, action.Damage);
                    break;
                case CinematicActionType.Quote:
                    yield return PlayQuote(action.Dialogue);
                    break;
                case CinematicActionType.CameraMovement:
                    yield return PlayCameraMovement(action.CameraTarget.position, action.CameraZoom, action.Duration);
                    break;
                case CinematicActionType.GameObjectMovement:
                    yield return PlayGameObjectMovement(action.GameObjectToMove, action.GameObjectTarget.position, action.Duration, action.CameraZoom, action.CameraFollow);
                    break;
                case CinematicActionType.Activate:
                    yield return ActivateObject(action.GameObjectToActivate);
                    break;
            }
        }

        private IEnumerator ActivateObject(GameObject actionGameObjectToActivate)
        {
            actionGameObjectToActivate.SetActive(true);
            yield break;
        }

        private IEnumerator PlayQuote(Dialogue dialogue)
        {
            dialogueManager.StartDialogue(dialogue);
            while (dialogueManager.IsDisplayingDialogue)
            {
                yield return null;
            }
        }

        private IEnumerator PlayDamage(Targetable target, int damage)
        {
            target.CurrentHealthPoints -= damage;
            yield break;
        }

        private IEnumerator PlayCameraMovement(Vector3 targetPos, float targetZoom, float duration)
        {
            yield return cameraController.MoveCameraTo(targetPos, targetZoom, duration);
        }

        private IEnumerator PlayGameObjectMovement(Transform movingObject, Vector3 targetPos, float duration, float targetZoom, bool moveWithCam = false)
        {
            var startPosition = movingObject.transform.position;
            if (moveWithCam)
            {
                var camTransform = mainCamera.transform;
                camTransform.position = new Vector3(startPosition.x, startPosition.y, camTransform.position.z);
                StartCoroutine(cameraController.MoveCameraTo(targetPos, targetZoom, duration));
            }
            for (float elapsedTime = 0; elapsedTime < duration; elapsedTime += Time.deltaTime)
            {
                movingObject.position = Vector3.Lerp(startPosition, targetPos, elapsedTime / duration);
                yield return null;
            }
            movingObject.position = targetPos;
        }

        public void LaunchCinematic(Cinematic cinematic)
        {
            StartCoroutine(PlayCameraActions(cinematic.Actions));
        }
    }
}