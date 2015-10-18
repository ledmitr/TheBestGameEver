using UnityEngine;
using System.Collections;

public class ViewDrag : MonoBehaviour
{
    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;
    float z = 0.0f;

    private float oldDistance = 0f;

    public float CurrentZoom = 0;
    public float ZoomSpeed = 1;
    private Vector3 initialPosition;
    private Vector3 initialRotation;
    public Vector2 zoomRange = new Vector2(1, 15);

    // Use this for initialization
    void Start()
    {

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
            var touch0 = Input.GetTouch(0).position;
            var touch1 = Input.GetTouch(1).position;
            var distance = Vector2.Distance(touch0, touch1);
            var diffValue = (distance - oldDistance) / oldDistance;
            CurrentZoom -= diffValue * Time.deltaTime * 1000 * ZoomSpeed;
            CurrentZoom = Mathf.Clamp(CurrentZoom, zoomRange.x, zoomRange.y);
            GetComponent<Camera>().orthographicSize = CurrentZoom;
            oldDistance = distance;
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

        transform.position = position;
    }
}