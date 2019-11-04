using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static Game.Constants;

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
        [SerializeField] private KeyCode moveUpKey = KeyCode.W;
        [SerializeField] private KeyCode moveDownKey = KeyCode.S;
        [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
        [SerializeField] private KeyCode moveRightKey = KeyCode.D;
        [SerializeField] private KeyCode dragKey = KeyCode.Mouse2;

        private Vector2Int lastScreenSize;

        private float maxZoom;

        private float targetOrthographicSize;
        private Vector3 targetPos;

        private bool controlsEnabled = true;

        private Camera camera;
        
        private float yMovement;
        private float xMovement;
        
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
        
        private bool IsMovingLeft => Input.mousePosition.x < scrollArea || Input.GetKey(moveLeftKey);
        private bool IsMovingRight => Input.mousePosition.x >= Screen.width - scrollArea || Input.GetKey(moveRightKey);
        private bool IsMovingDown => Input.mousePosition.y < scrollArea || Input.GetKey(moveDownKey);
        private bool IsMovingUp => Input.mousePosition.y >= Screen.height - scrollArea || Input.GetKey(moveUpKey);

        public float MaxZoom => maxZoom;

        private void Awake()
        {
            camera = GetComponent<Camera>();
            targetOrthographicSize = camera.orthographicSize;
            targetPos = transform.position;
            OnScreenSizeChanged();
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
            if (!Input.GetKey(dragKey))
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
            if (Input.GetKeyDown(dragKey)) StartCoroutine(Drag());
        }

        private IEnumerator Drag()
        {
            var origin = camera.ScreenToWorldPoint(Input.mousePosition);
            while (Input.GetKey(dragKey))
            {
                var difference = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
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
            var oldPos = camera.ScreenToWorldPoint (Input.mousePosition);
            camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);
            targetPos += oldPos - camera.ScreenToWorldPoint (Input.mousePosition);
            ClampCameraPosition();
        }

        private void ClampCameraPosition()
        {
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
            var topRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, 0));
            var bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);

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
            targetOrthographicSize = Mathf.Clamp(camera.orthographicSize - 1, minZoom, maxZoom);
        }

        private void ZoomOut()
        {
            targetOrthographicSize = Mathf.Clamp(camera.orthographicSize + 1, minZoom, maxZoom);
        }

        public void EnableControls()
        {
            controlsEnabled = true;
        }

        public void DisableControls()
        {
            controlsEnabled = false;
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

