using UnityEngine;
using System.Collections;

public class ViewDrag : MonoBehaviour
{
    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;
    //float z = 0.0f;

    //private float oldDistance = 0f;

    //public float CurrentZoom = 0;
    //public float ZoomSpeed = 1;
    //private Vector3 initialPosition;
    //private Vector3 initialRotation;
    public Vector2 ZoomRange = new Vector2(1, 15);

    public GameObject Land;

    private float minCameraXPosition;
    private float minCameraZPosition;
    private float maxCameraXPosition;
    private float maxCameraZPosition;

    // Use this for initialization
    void Start()
    {
        var landSize = Land.GetComponent<Terrain>().terrainData.size;
        minCameraXPosition = -landSize.x / 2;
        minCameraZPosition = -landSize.z / 2;
        maxCameraXPosition = landSize.x / 2;
        maxCameraZPosition = landSize.z / 2;
        _lCamera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                hit_position = Input.mousePosition;
                camera_position = transform.position;
            }
            if (Input.GetMouseButton(0))
            {
                current_position = Input.mousePosition;
                LeftMouseDrag();
            }
        } else if (Input.touchCount >= 2)
        {
            ExecutePinchZoom();
            //var touch0 = Input.GetTouch(0).position;
            //var touch1 = Input.GetTouch(1).position;
            //var distance = Vector2.Distance(touch0, touch1);
            //var diffValue = (distance - oldDistance) / oldDistance + 1;
            //CurrentZoom -= diffValue * Time.deltaTime * 100 * ZoomSpeed;
            //CurrentZoom = Mathf.Clamp(CurrentZoom, zoomRange.x, zoomRange.y);
            //GetComponent<Camera>().orthographicSize = CurrentZoom;
            //oldDistance = distance;
        }
    }

    void LeftMouseDrag()
    {
        // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
        // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
        current_position.z = hit_position.z = camera_position.y;

        // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
        // anyways.  
        Vector3 direction = Camera.main.ScreenToWorldPoint(current_position) - Camera.main.ScreenToWorldPoint(hit_position);

        // Invert direction to that terrain appears to move with the mouse.
        direction = direction * -1;

        Vector3 position = camera_position + direction;

        transform.position = new Vector3(
            Mathf.Clamp(position.x, minCameraXPosition, maxCameraXPosition), 
            position.y,
            Mathf.Clamp(position.z, minCameraZPosition, maxCameraZPosition)
        );
    }

    public float PerspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float OrthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    private Camera _lCamera;
    public void ExecutePinchZoom()
    {
        
        // Store both touches.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        // If the camera is orthographic...
        if (_lCamera.orthographic)
        {
            // ... change the orthographic size based on the change in distance between the touches.
            _lCamera.orthographicSize += deltaMagnitudeDiff * OrthoZoomSpeed;

            // Make sure the orthographic size never drops below zero.
            _lCamera.orthographicSize = Mathf.Clamp(_lCamera.orthographicSize, ZoomRange.x, ZoomRange.y);
        }
        else
        {
            // Otherwise change the field of view based on the change in distance between the touches.
            _lCamera.fieldOfView += deltaMagnitudeDiff * PerspectiveZoomSpeed;

            // Clamp the field of view to make sure it's between 0 and 180.
            _lCamera.fieldOfView = Mathf.Clamp(_lCamera.fieldOfView, 0.1f, 179.9f);
        }
    }
}