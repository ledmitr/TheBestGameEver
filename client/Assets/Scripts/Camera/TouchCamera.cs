// Just add this script to your camera. It doesn't need any configuration.

using System.Collections;
using System.Linq;
using UnityEngine;

public class TouchCamera : MonoBehaviour
{
    public GameObject Land;

    private float minCameraXPosition;
    private float minCameraZPosition;
    private float maxCameraXPosition;
    private float maxCameraZPosition;
    private float minCameraYPosition;
    private float maxCameraYPosition;

    private float zoomSpeed = 1.5f;
    private float moveSpeed = 100f;

    public void Start()
    {
        var landSize = Land.GetComponent<Terrain>().terrainData.size;
        minCameraXPosition = -landSize.x/2;
        minCameraZPosition = -landSize.z/2;
        maxCameraXPosition = landSize.x/2;
        maxCameraZPosition = landSize.z/2;

        minCameraYPosition = 25;
        maxCameraYPosition = 100;

        
    }

    private readonly Vector2?[] _oldTouchPositions =
    {
        null,
        null
    };

    private Vector2 _oldTouchVector;
    private float _oldTouchDistance;

    private void Update()
    {
        if (Input.touchCount == 0)
        {
            _oldTouchPositions[0] = null;
            _oldTouchPositions[1] = null;
        }
        else
        {
            var cameraComponent = GetComponent<Camera>();
            if (Input.touchCount == 1)
            {
                if (_oldTouchPositions[0] == null || _oldTouchPositions[1] != null)
                {
                    _oldTouchPositions[0] = Input.GetTouch(0).position;
                    _oldTouchPositions[1] = null;
                }
                else
                {
                    var newTouchPosition = Input.GetTouch(0).position;

                    var newCameraPosition = transform.position +
                        transform.TransformDirection((Vector3)((_oldTouchPositions[0] - newTouchPosition) * moveSpeed / cameraComponent.pixelHeight));

                    if (newCameraPosition.x >= minCameraXPosition && newCameraPosition.x <= maxCameraXPosition
                            && newCameraPosition.z >= minCameraZPosition && newCameraPosition.z <= maxCameraZPosition
                                && newCameraPosition.y >= minCameraYPosition && newCameraPosition.y <= maxCameraYPosition)
                    {
                        transform.position = newCameraPosition;
                    }
                    
                    _oldTouchPositions[0] = newTouchPosition;
                }
            }
            else
            {
                if (_oldTouchPositions[1] == null)
                {
                    _oldTouchPositions[0] = Input.GetTouch(0).position;
                    _oldTouchPositions[1] = Input.GetTouch(1).position;
                    _oldTouchVector = (Vector2) (_oldTouchPositions[0] - _oldTouchPositions[1]);
                    _oldTouchDistance = _oldTouchVector.magnitude;
                }
                else
                {
                    Vector2[] newTouchPositions =
                    {
                        Input.GetTouch(0).position,
                        Input.GetTouch(1).position
                    };
                    var newTouchVector = newTouchPositions[0] - newTouchPositions[1];
                    var newTouchDistance = newTouchVector.magnitude;

                    // kind of buggy but idk why
                    float k = newTouchDistance/_oldTouchDistance;
                    Vector3 pos = Camera.main.ScreenToViewportPoint(newTouchPositions[0]);
                    Vector3 move = pos.y * zoomSpeed * (k > 1 ? transform.forward : -transform.forward);
                    transform.Translate(move, Space.World);

                    _oldTouchPositions[0] = newTouchPositions[0];
                    _oldTouchPositions[1] = newTouchPositions[1];
                    _oldTouchVector = newTouchVector;
                    _oldTouchDistance = newTouchDistance;
                }
            }
        }
    }
}
