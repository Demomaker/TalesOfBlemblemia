using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour {
     
        [Range(5, 20)][SerializeField] private float scrollArea = 10f;
        [Range(10, 20)][SerializeField] private float moveSpeed = 20f;
        [Range(5, 15)][SerializeField] private float zoomSpeed = 10f;
        [Range(2, 4.5f)][SerializeField] private float minZoom = 4.5f;
        [Range(-100, 100)][SerializeField] private int minX = -14;
        [Range(-100, 100)][SerializeField] private int maxX = 14;
        [Range(-100, 100)][SerializeField] private int minY = -14;
        [Range(-100, 100)][SerializeField] private int maxY = 14;
        [SerializeField] private KeyCode moveUpKey = KeyCode.W;
        [SerializeField] private KeyCode moveDownKey = KeyCode.S;
        [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
        [SerializeField] private KeyCode moveRightKey = KeyCode.D;
        [SerializeField] private KeyCode dragKey = KeyCode.Mouse2;

        private Vector2Int lastScreenSize;

        private float maxZoom = 8.5f;

        private float targetOrtho = 0;
        private Vector3 targetPos;

        private bool zoomEnabled = true;
        private bool moveEnabled = true;
        private bool dragEnabled = true;

        private bool IsMovingLeft => Input.mousePosition.x < scrollArea || Input.GetKey(moveLeftKey);
        private bool IsMovingRight => Input.mousePosition.x >= Screen.width - scrollArea || Input.GetKey(moveRightKey);
        private bool IsMovingDown => Input.mousePosition.y < scrollArea || Input.GetKey(moveDownKey);
        private bool IsMovingUp => Input.mousePosition.y >= Screen.height - scrollArea || Input.GetKey(moveUpKey);

        private float yMovement = 0;
        private float xMovement = 0;

        private Camera camera;

        public bool ZoomEnabled => zoomEnabled;
        public bool MoveEnabled => moveEnabled;
        public bool DragEnabled => dragEnabled;

        private void Awake()
        {
            camera = GetComponent<Camera>();
            targetOrtho = camera.orthographicSize;
            targetPos = transform.position;
            OnScreenSizeChanged();
        }

        private void LateUpdate()
        {
            if (lastScreenSize.x != Screen.width ||  lastScreenSize.y != Screen.height)
            {
                OnScreenSizeChanged();
            }
            
            if (ZoomEnabled)
            {
                if (Input.mouseScrollDelta.y > 0)
                {
                    ZoomIn();
                }

                if (Input.mouseScrollDelta.y < 0)
                {
                    ZoomOut();
                }

            }

            if (MoveEnabled && !Input.GetKey(dragKey))
            {
                if (IsMovingLeft && !IsMovingRight)
                {
                    xMovement = Mathf.Clamp(xMovement - Time.deltaTime, -1, 1);
                } 
                else if (xMovement < 0)
                {
                    xMovement = Mathf.Clamp(xMovement + Time.deltaTime, -1, 0);
                }

                if (IsMovingRight && !IsMovingLeft)
                {
                    xMovement = Mathf.Clamp(xMovement + Time.deltaTime, -1, 1);
                }
                else if (xMovement > 0)
                {
                    xMovement = Mathf.Clamp(xMovement - Time.deltaTime, 0, 1);
                }

                if (IsMovingDown && !IsMovingUp)
                {
                    yMovement = Mathf.Clamp(yMovement - Time.deltaTime, -1, 1);
                }
                else if (yMovement < 0)
                {
                    yMovement = Mathf.Clamp(yMovement + Time.deltaTime, -1, 0);
                }

                if (IsMovingUp && !IsMovingDown)
                {
                    yMovement = Mathf.Clamp(yMovement + Time.deltaTime, -1, 1);
                }
                else if (yMovement > 0)
                {
                    yMovement = Mathf.Clamp(yMovement - Time.deltaTime, 0, 1);
                }
                targetPos += moveSpeed * Time.deltaTime * new Vector3(xMovement, yMovement, 0);
            }

            UpdateMovement();
            UpdateZoom();

            if (DragEnabled)
            {
                if (Input.GetKeyDown(dragKey)) StartCoroutine(Drag());
            }
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

        private void UpdateMovement()
        {
            transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * zoomSpeed);
            ClampCameraPosition();
        }

        private void UpdateZoom()
        {
            Vector3 oldPos = camera.ScreenToWorldPoint (Input.mousePosition);
            camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, targetOrtho, Time.deltaTime * zoomSpeed);
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
                transform.position -= new Vector3(topRight.x - maxX, 0, 0);
                targetPos.x = transform.position.x;
                xMovement = 0;
            }

            if (topRight.y > maxY)
            {
                transform.position -= new Vector3(0, topRight.y - maxY, 0);
                targetPos.y = transform.position.y;
                yMovement = 0;
            }

            if (bottomLeft.x < minX)
            {
                transform.position += new Vector3(minX - bottomLeft.x, 0, 0);
                targetPos.x = transform.position.x;
                xMovement = 0;
            }

            if (bottomLeft.y < minY)
            {
                transform.position += new Vector3(0, minY - bottomLeft.y, 0);
                targetPos.y = transform.position.y;
                yMovement = 0;
            }
        }

        private void ZoomIn()
        {
            targetOrtho = Mathf.Clamp(camera.orthographicSize - 1, minZoom, maxZoom);
        }

        private void ZoomOut()
        {
            targetOrtho = Mathf.Clamp(camera.orthographicSize + 1, minZoom, maxZoom);
        }

        public void EnableControls()
        {
            EnableZoom();
            EnableMove();
            EnableDrag();
        }

        public void EnableZoom()
        {
            zoomEnabled = true;
        }

        public void EnableMove()
        {
            moveEnabled = true;
        }

        public void EnableDrag()
        {
            dragEnabled = true;
        }
        
        public void DisableControls()
        {
            DisableZoom();
            DisableMove();
            DisableDrag();
        }

        public void DisableZoom()
        {
            zoomEnabled = false;
        }

        public void DisableMove()
        {
            moveEnabled = false;
        }

        public void DisableDrag()
        {
            dragEnabled = false;
        }
        

        private void OnScreenSizeChanged()
        {
            lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            var screenAspectRatio = lastScreenSize.x / (float) lastScreenSize.y;
            var worldAspectRatio = (maxX - minX) / (maxY - minY);
            maxZoom = Math.Min((maxY - minY) * worldAspectRatio / screenAspectRatio, maxY - minY) / 2f;
            ClampCameraPosition();
        }
    }
}

