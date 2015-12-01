using Assets.TD.scripts.Utils;
using UnityEngine;

namespace Assets.TD.scripts
{
    public class ViewDrag : MonoBehaviour
    {
        private Camera _lCamera;
        public GameObject Land;

        private Vector3 _hitPosition = Vector3.zero;
        private Vector3 _currentPosition = Vector3.zero;
        private Vector3 _cameraPosition = Vector3.zero;

        public Vector2 ZoomRange = new Vector2(1, 15);
        public float EditorZoomSpeed = 0.5f;
        public float MobileZoomSpeed = 2.0f;
        public float MoveSpeed = 2.0F;

        private float _minCameraXPosition;
        private float _minCameraZPosition;
        private float _maxCameraXPosition;
        private float _maxCameraZPosition;

        // Use this for initialization
        void Start()
        {
            var landSize = Land.GetComponent<Terrain>().terrainData.size;
            _minCameraXPosition = 0.0F;
            _minCameraZPosition = -landSize.z;
            _maxCameraXPosition = landSize.x;
            _maxCameraZPosition = 0.0F;
            _lCamera = GetComponent<Camera>();
        }

        private static readonly Vector3 CameraRelativeUp = new Vector3(-1.0F, 0.0F, 1.0F);
        private static readonly Vector3 CameraRelativeDown = -CameraRelativeUp;
        private static readonly Vector3 CameraRelativeRight = new Vector3(1.0F, 0.0F, 1.0F);
        private static readonly Vector3 CameraRelativeLeft = -CameraRelativeRight;

        void Update()
        {
            /**/
#if UNITY_EDITOR
            var newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            if (Input.GetKey(KeyCode.RightArrow)){
                newPos += CameraRelativeRight * MoveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow)){
                newPos += CameraRelativeLeft * MoveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.UpArrow)){
                newPos += CameraRelativeUp * MoveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow)){
                newPos += CameraRelativeDown * MoveSpeed * Time.deltaTime;
            }

            transform.position = new Vector3(Mathf.Clamp(newPos.x, _minCameraXPosition, _maxCameraXPosition), newPos.y, Mathf.Clamp(newPos.z, _minCameraZPosition, _maxCameraZPosition));

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (_lCamera.orthographic && !Math.Approximately(scroll, 0.0F))
            {
                var newSize = _lCamera.orthographicSize + (-scroll * EditorZoomSpeed);
                _lCamera.orthographicSize = Mathf.Clamp(newSize, ZoomRange.x, ZoomRange.y);
            }

#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
        if (Input.touchCount == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _hitPosition = Input.mousePosition;
                _cameraPosition = transform.position;
            }
            if (Input.GetMouseButton(0))
            {
                _currentPosition = Input.mousePosition;
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
#endif
        }

        void LeftMouseDrag()
        {
            // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
            // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
            _currentPosition.z = _hitPosition.z = _cameraPosition.y;

            // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
            // anyways.  
            Vector3 direction = Camera.main.ScreenToWorldPoint(_currentPosition) - Camera.main.ScreenToWorldPoint(_hitPosition);

            // Invert direction to that terrain appears to move with the mouse.
            direction = direction * -1;

            Vector3 position = _cameraPosition + direction;

            transform.position = new Vector3(
                Mathf.Clamp(position.x, _minCameraXPosition, _maxCameraXPosition), 
                position.y,
                Mathf.Clamp(position.z, _minCameraZPosition, _maxCameraZPosition)
                );
        }

        private void ExecutePinchZoom()
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
                _lCamera.orthographicSize += deltaMagnitudeDiff * MobileZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                _lCamera.orthographicSize = Mathf.Clamp(_lCamera.orthographicSize, ZoomRange.x, ZoomRange.y);
            }
        }
    }
}