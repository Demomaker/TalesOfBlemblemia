using System;
using UnityEngine;
using System.Collections;
 
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

    public void EnableControls(bool _enable)
    {
        if(_enable)
        {
            ZoomEnabled = true;
            MoveEnabled = true;
        }
        else
        {
            ZoomEnabled = false;
            MoveEnabled = false;
        }
    }
    public bool ZoomEnabled = true;
    public bool MoveEnabled = true;

    private Camera camera;
   
    private Vector2 _mousePos;
    private Vector3 _moveVector;
    private int _xMove;
    private int _yMove;
    private int _orthographicSize;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        float screenAspect = (float) Screen.width / (float) Screen.height;
        float camHalfHeight = camera.orthographicSize;
        float camHalfWidth = screenAspect * camHalfHeight;
        maxZoom = (maxY - minY) / (camHalfWidth / camHalfHeight) / 2.0f;
    }

    // Update is called once per frame
    void Update () {
        _mousePos = Input.mousePosition;
       
        //Move camera if mouse is at the edge of the screen
        if(MoveEnabled)
        {
            if(_mousePos.x < horizontalScrollArea || Input.GetKey(moveLeftKey)) { _xMove = -1; }
            else if (_mousePos.x >= Screen.width - horizontalScrollArea || Input.GetKey(moveRightKey)) { _xMove = 1; }
            else { _xMove = 0; }
               
           
            if(_mousePos.y < verticalScrollArea || Input.GetKey(moveDownKey)) { _yMove = -1; }
            else if(_mousePos.y >= Screen.height - verticalScrollArea || Input.GetKey(moveUpKey)) { _yMove = 1; }
            else { _yMove = 0; }
        }
        else
        {
            _xMove = 0;
            _yMove = 0;
        }
        // Zoom Camera in or out
        if(ZoomEnabled)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                _orthographicSize = 1;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                _orthographicSize = -1;
            }
            else
            {
                _orthographicSize = 0;
            }
        }
        else
        {
            _orthographicSize = 0;
            
        }
        
        //move the object
        MoveMe(_xMove, _yMove);

        if (_orthographicSize != 0)
        {
            // Perform zoom
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + _orthographicSize * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
            if (camera.orthographicSize != minZoom && _orthographicSize < 0)
            {
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                float xFactor = Mathf.Abs(_mousePos.x - screenCenter.x) / screenCenter.x;
                float yFactor = Mathf.Abs(_mousePos.y - screenCenter.y) / screenCenter.y;
                _yMove = _xMove = 0;
                if (_mousePos.x < screenCenter.x) _xMove = -1;
                else if (_mousePos.x > screenCenter.x) _xMove = 1;
                if (_mousePos.y < screenCenter.y) _yMove = -1;
                else if (_mousePos.y > screenCenter.y) _yMove = 1;
                Debug.Log(" center y: " + Screen.height / 2 + "mouse y : " + _mousePos.y);
                MoveMe(_xMove * xFactor, _yMove * yFactor);
            }
        }

        //  Check if camera is out-of-bounds, if so, move back in-bounds
        Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, 0));
        Vector3 bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, 0));

        if (topRight.x > maxX)
        {
            transform.position = new Vector3(transform.position.x - (topRight.x - maxX), transform.position.y, transform.position.z);
        }

        if (topRight.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (topRight.y - maxY), transform.position.z);
        }

        if (bottomLeft.x < minX)
        {
            transform.position = new Vector3(transform.position.x + (minX - bottomLeft.x), transform.position.y, transform.position.z);
        }

        if (bottomLeft.y < minY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (minY - bottomLeft.y), transform.position.z);
        }
    }
       
    private void MoveMe(float x, float y)
    {
        _moveVector = new Vector3(x * horizontalMoveSpeed, y * verticalMoveSpeed, 0) * Time.deltaTime;
        transform.Translate(_moveVector, Space.World);
    }
}