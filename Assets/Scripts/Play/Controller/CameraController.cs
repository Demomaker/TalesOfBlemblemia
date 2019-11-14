using System;
using System.Collections;
using UnityEngine;
using static Game.CameraConstants;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour {
     
        [Range(MIN_CAM_SCROLL_AREA, MAX_CAM_SCROLL_AREA)][SerializeField] private float scrollArea;
        [Range(MIN_CAM_MOVE_SPEED, MAX_CAM_MOVE_SPEED)][SerializeField] private float moveSpeed;
        [Range(MIN_CAM_ZOOM_SPEED, MAX_CAM_ZOOM_SPEED)][SerializeField] private float zoomSpeed;
        [Range(MIN_CAM_ORTHOGRAPHIC_SIZE, MAX_CAM_ORTHOGRAPHIC_SIZE)][SerializeField] private float minZoom;
        [Range(MIN_CAM_X, MAX_CAM_X)][SerializeField] private int minX;
        [Range(MIN_CAM_X, MAX_CAM_X)][SerializeField] private int maxX;
        [Range(MIN_CAM_Y, MAX_CAM_Y)][SerializeField] private int minY;
        [Range(MIN_CAM_Y, MAX_CAM_Y)][SerializeField] private int maxY;

        private Vector2Int lastScreenSize;

        private float maxZoom;

        private float targetOrthographicSize;
        private Vector3 targetPos;

        private bool controlsEnabled = true;

        private Camera mainCamera;
        
        private float yMovement = 0;
        private float xMovement = 0;
        
        private const KeyCode MOVE_UP_KEY = KeyCode.W;
        private const KeyCode MOVE_DOWN_KEY = KeyCode.S;
        private const KeyCode MOVE_LEFT_KEY = KeyCode.A;
        private const KeyCode MOVE_RIGHT_KEY = KeyCode.D;
        private const KeyCode DRAG_KEY = KeyCode.Mouse2;
        
        private float YMovement
        {
            get => yMovement;
            set => yMovement = Mathf.Clamp(value, -1, 1);
        }
        
        private float XMovement
        {
            get => xMovement;
            set => xMovement = Mathf.Clamp(value, -1, 1);
        }
        
        public float MaxZoom => maxZoom;
        
        private bool IsMovingLeft => Input.mousePosition.x < scrollArea || Input.GetKey(MOVE_LEFT_KEY);
        private bool IsMovingRight => Input.mousePosition.x >= Screen.width - scrollArea || Input.GetKey(MOVE_RIGHT_KEY);
        private bool IsMovingDown => Input.mousePosition.y < scrollArea || Input.GetKey(MOVE_DOWN_KEY);
        private bool IsMovingUp => Input.mousePosition.y >= Screen.height - scrollArea || Input.GetKey(MOVE_UP_KEY);

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            OnScreenSizeChanged();
            InitCamera();
        }

        public IEnumerator MoveCameraTo(Vector3 to, float endZoom, float duration)
        {
            var controlsWereEnabled = controlsEnabled;
            DisableControls();
            var startPosition = transform.position;
            var endPosition = new Vector3(to.x, to.y, startPosition.z);
            var startZoom = mainCamera.orthographicSize;

            for (float elapsedTime = 0; elapsedTime < duration; elapsedTime += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
                ClampCameraPosition();
                mainCamera.orthographicSize = Mathf.Lerp(startZoom, endZoom, elapsedTime / duration);
                yield return null;
            }

            transform.position = endPosition;
            mainCamera.orthographicSize = endZoom;
            ClampCameraPosition();
            if(controlsWereEnabled) EnableControls(); 
        }

        private void LateUpdate()
        {
            if (!controlsEnabled) return;

            if (lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
            {
                OnScreenSizeChanged();
            }

            UpdateTargetPosition();
            UpdatePosition();
            UpdateZoom();
            UpdateDrag();
        }

        private void UpdateTargetPosition()
        {
            if (!Input.GetKey(DRAG_KEY))
            {
                if (IsMovingLeft && !IsMovingRight)
                {
                    XMovement -= Time.deltaTime;
                } 
                else if (XMovement < 0)
                {
                    XMovement = Mathf.Clamp(XMovement + Time.deltaTime, -1, 0);
                }

                if (IsMovingRight && !IsMovingLeft)
                {
                    XMovement += Time.deltaTime;
                }
                else if (XMovement > 0)
                {
                    XMovement = Mathf.Clamp(XMovement - Time.deltaTime, 0, 1);
                }

                if (IsMovingDown && !IsMovingUp)
                {
                    YMovement -= Time.deltaTime;
                }
                else if (YMovement < 0)
                {
                    YMovement = Mathf.Clamp(YMovement + Time.deltaTime, -1, 0);
                }

                if (IsMovingUp && !IsMovingDown)
                {
                    YMovement += Time.deltaTime;
                }
                else if (YMovement > 0)
                {
                    YMovement = Mathf.Clamp(YMovement - Time.deltaTime, 0, 1);
                }
                
                targetPos += moveSpeed * Time.deltaTime * new Vector3(XMovement, YMovement, 0);
            }
        }

        private void UpdateDrag()
        {
            if (Input.GetKeyDown(DRAG_KEY)) StartCoroutine(Drag());
        }

        private IEnumerator Drag()
        {
            var origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            while (Input.GetKey(DRAG_KEY))
            {
                var difference = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                targetPos = origin - difference;
                ClampCameraPosition();
                yield return null;
            }
        }

        private void UpdatePosition()
        {
            transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * zoomSpeed);
            ClampCameraPosition();
        }

        private void UpdateZoom()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                ZoomIn();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                ZoomOut();
            }
            var oldPos = mainCamera.ScreenToWorldPoint (Input.mousePosition);
            mainCamera.orthographicSize = Mathf.Lerp (mainCamera.orthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);
            targetPos += oldPos - mainCamera.ScreenToWorldPoint (Input.mousePosition);
            ClampCameraPosition();
        }

        private void ClampCameraPosition()
        {
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
            var topRight = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth, mainCamera.pixelHeight, 0));
            var bottomLeft = mainCamera.ScreenToWorldPoint(Vector3.zero);

            if (topRight.x > maxX)
            {
                var position = transform.position -= new Vector3(topRight.x - maxX, 0, 0);
                targetPos.x = position.x;
                XMovement = 0;
            }

            if (topRight.y > maxY)
            {
                var position = transform.position -= new Vector3(0, topRight.y - maxY, 0);
                targetPos.y = position.y;
                YMovement = 0;
            }

            if (bottomLeft.x < minX)
            {
                var position = transform.position += new Vector3(minX - bottomLeft.x, 0, 0);
                targetPos.x = position.x;
                XMovement = 0;
            }

            if (bottomLeft.y < minY)
            {
                var position = transform.position += new Vector3(0, minY - bottomLeft.y, 0);
                targetPos.y = position.y;
                YMovement = 0;
            }
        }

        private void ZoomIn()
        {
            targetOrthographicSize = Mathf.Clamp(mainCamera.orthographicSize - 1, minZoom, maxZoom);
        }

        private void ZoomOut()
        {
            targetOrthographicSize = Mathf.Clamp(mainCamera.orthographicSize + 1, minZoom, maxZoom);
        }

        public void EnableControls()
        {
            controlsEnabled = true;
            InitCamera();
        }

        public void DisableControls()
        {
            controlsEnabled = false;
        }

        private void InitCamera()
        {
            targetPos = transform.position;
            mainCamera.orthographicSize = targetOrthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
        }


        private void OnScreenSizeChanged()
        {
            lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            var screenAspectRatio = lastScreenSize.x / (float) lastScreenSize.y;
            var worldAspectRatio = (maxX - minX) / (float)(maxY - minY);
            maxZoom = Math.Min((maxY - minY) * worldAspectRatio / screenAspectRatio, maxY - minY) / 2f;
            ClampCameraPosition();
        }
    }
}

