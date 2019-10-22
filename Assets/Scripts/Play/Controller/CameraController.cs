using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour {
     
        [SerializeField] private float verticalScrollArea = 10f;
        [SerializeField] private float horizontalScrollArea = 10f;
        [SerializeField] private float horizontalMoveSpeed = 10f;
        [SerializeField] private float verticalMoveSpeed = 10f;
        [SerializeField] private float zoomSpeed = 10f;
        private float maxZoom = 8.5f;
        [SerializeField] private float minZoom = 4.5f;
        [SerializeField] private float minX = -14f;
        [SerializeField] private float maxX = 14f;
        [SerializeField] private float minY = -14f;
        [SerializeField] private float maxY = 14f;
        [SerializeField] private KeyCode moveUpKey = KeyCode.W;
        [SerializeField] private KeyCode moveDownKey = KeyCode.S;
        [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
        [SerializeField] private KeyCode moveRightKey = KeyCode.D;

        public bool ZoomEnabled => zoomEnabled;
        public bool MoveEnabled => moveEnabled;

        private bool zoomEnabled = true;
        private bool moveEnabled = true;
        
        private Camera camera;
       
        private Vector2 mousePos;
        private Vector3 moveVector;
        private int xMove;
        private int yMove;
        private int orthographicSize;

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        private void Start()
        {
            var screenAspect = Screen.width / Screen.height;
            var camHalfHeight = camera.orthographicSize;
            var camHalfWidth = screenAspect * camHalfHeight;
            maxZoom = (maxY - minY) / (camHalfWidth / camHalfHeight) / 2.0f;
        }
        
        void Update () {
            mousePos = Input.mousePosition;
           
            //Move camera if mouse is at the edge of the screen
            if(MoveEnabled)
            {
                if(mousePos.x < horizontalScrollArea || Input.GetKey(moveLeftKey)) { xMove = -1; }
                else if (mousePos.x >= Screen.width - horizontalScrollArea || Input.GetKey(moveRightKey)) { xMove = 1; }
                else { xMove = 0; }
                   
               
                if(mousePos.y < verticalScrollArea || Input.GetKey(moveDownKey)) { yMove = -1; }
                else if(mousePos.y >= Screen.height - verticalScrollArea || Input.GetKey(moveUpKey)) { yMove = 1; }
                else { yMove = 0; }
            }
            else
            {
                xMove = 0;
                yMove = 0;
            }
            
            // Zoom Camera in or out
            if(ZoomEnabled)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                    orthographicSize = 1;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                    orthographicSize = -1;
                }
                else
                {
                    orthographicSize = 0;
                }
            }
            else
            {
                orthographicSize = 0;
            }
            
            Move(xMove, yMove);

            if (orthographicSize != 0)
            {
                // Perform zoom
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + orthographicSize * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
                if (camera.orthographicSize != minZoom && orthographicSize < 0)
                {
                    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                    float xFactor = Mathf.Abs(mousePos.x - screenCenter.x) / screenCenter.x;
                    float yFactor = Mathf.Abs(mousePos.y - screenCenter.y) / screenCenter.y;
                    yMove = xMove = 0;
                    if (mousePos.x < screenCenter.x) xMove = -1;
                    else if (mousePos.x > screenCenter.x) xMove = 1;
                    if (mousePos.y < screenCenter.y) yMove = -1;
                    else if (mousePos.y > screenCenter.y) yMove = 1;
                    Move(xMove * xFactor, yMove * yFactor);
                }
            }
            
            ClampCameraPosition();
        }
           
        private void Move(float x, float y)
        {
            moveVector = new Vector3(x * horizontalMoveSpeed, y * verticalMoveSpeed, 0) * Time.deltaTime;
            transform.Translate(moveVector, Space.World);
        }

        private void ClampCameraPosition()
        {
            Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, 0));
            Vector3 bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, 0));

            if (topRight.x > maxX) transform.position = new Vector3(transform.position.x - (topRight.x - maxX), transform.position.y, transform.position.z);
            if (topRight.y > maxY) transform.position = new Vector3(transform.position.x, transform.position.y - (topRight.y - maxY), transform.position.z);
            if (bottomLeft.x < minX) transform.position = new Vector3(transform.position.x + (minX - bottomLeft.x), transform.position.y, transform.position.z);
            if (bottomLeft.y < minY) transform.position = new Vector3(transform.position.x, transform.position.y + (minY - bottomLeft.y), transform.position.z);
        }
        
        public void EnableControls()
        {
            zoomEnabled = moveEnabled = true;
        }
        
        public void DisableControls()
        {
            zoomEnabled = moveEnabled = false;
        }
    }
}

