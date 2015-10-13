// Just add this script to your camera. It doesn't need any configuration.

using UnityEngine;

public class TouchCamera : MonoBehaviour
{
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

                    transform.position +=
                        transform.TransformDirection((Vector3) ((_oldTouchPositions[0] - newTouchPosition)
                                                                *cameraComponent.orthographicSize/
                                                                cameraComponent.pixelHeight*2f));

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
                    var screen = new Vector2(cameraComponent.pixelWidth, cameraComponent.pixelHeight);

                    Vector2[] newTouchPositions =
                    {
                        Input.GetTouch(0).position,
                        Input.GetTouch(1).position
                    };
                    var newTouchVector = newTouchPositions[0] - newTouchPositions[1];
                    var newTouchDistance = newTouchVector.magnitude;

                    transform.position +=
                        transform.TransformDirection((Vector3) ((_oldTouchPositions[0] + _oldTouchPositions[1] - screen)
                                                                *cameraComponent.orthographicSize/screen.y));

                    transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, Mathf.Asin(
                        Mathf.Clamp((_oldTouchVector.y*newTouchVector.x
                                     - _oldTouchVector.x*newTouchVector.y)/_oldTouchDistance/newTouchDistance, -1f, 1f))/
                                                                                  0.0174532924f));

                    cameraComponent.orthographicSize *= _oldTouchDistance/newTouchDistance;

                    transform.position -=
                        transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen)
                                                     *cameraComponent.orthographicSize/screen.y);

                    _oldTouchPositions[0] = newTouchPositions[0];
                    _oldTouchPositions[1] = newTouchPositions[1];
                    _oldTouchVector = newTouchVector;
                    _oldTouchDistance = newTouchDistance;
                }
            }
        }
    }
}
