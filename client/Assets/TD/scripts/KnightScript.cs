using Assets.TD.scripts.Utils;
using UnityEngine;

namespace Assets.TD.scripts
{
    public class KnightScript : Selectable
    {

        private Vector3 _targetPosition;

        private bool _isSelected;
        public override bool IsSelected()
        {
            return _isSelected;
        }

        private Vector3[] _pathToMainTower;

        private int _currentPosition;

        private Renderer _renderer; 
        public override void Select(bool isSelected)
        {
            _isSelected = isSelected;
            if (isSelected)
                _renderer.sharedMaterials = SelectMaterial;
            else
                _renderer.sharedMaterials = DefaultMaterial;
        }

        public Material[] SelectMaterial;
        private Material[] DefaultMaterial;
        
        private bool _isMoving;

        public float Speed = 1.5F;

        public void Start()
        {
            //HealthBar = new HealthBar();
            _isSelected = false;
            _isMoving = _pathToMainTower != null && _pathToMainTower.Length > 1;
            _renderer = GetComponent<Renderer>();
            DefaultMaterial = _renderer.sharedMaterials;
        }

        public void SetPath(Vector3[] path)
        {
            _pathToMainTower = path;
            _currentPosition = 0;
            _targetPosition = _pathToMainTower[_currentPosition];
            Debug.Log("initial target position: " + _targetPosition);
        }
        
        public void TargetPositionChanged(Vector3 newTargetPosition)
        {
            _targetPosition = newTargetPosition;
            _isSelected = false;
            _isMoving = true;
            Debug.Log("target position changed to: " + newTargetPosition);
        }

        public void Update()
        {
            if (_isMoving)
                Move();
        }

        void OnCollisionEnter(Collision collision)
        {
            var isItTargetTower = Math.Approximately(collision.gameObject.transform.position.magnitude, _targetPosition.magnitude);
            if (collision.gameObject.tag == ApplicationConst.TowerTag && isItTargetTower)
            {
                _isMoving = false;
            }
        }

        private Collider lCollider {
            get { return GetComponent<Collider>(); }
        }
        
        private void Move()
        {
            var magnitudePosition = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z).magnitude;
            var magnitudeTarget = new Vector3(_targetPosition.x, 0, _targetPosition.z).magnitude;
            var targetPostionReached = Math.Approximately(magnitudePosition, magnitudeTarget);
            if (!targetPostionReached)
            {
                var v = Vector3.Lerp(gameObject.transform.position, _targetPosition,
                    1/(Speed*Vector3.Distance(gameObject.transform.position, _targetPosition)));

                RaycastHit hit;
                if (Physics.Raycast(v, Vector3.down, out hit, 5.0f))
                {
                    // collider.bounds.extents.y will be half of the height of the bounding box
                    // only useful if your pivot point is in the center, if it's at the feet you don't need this
                    v.y = hit.point.y + lCollider.bounds.extents.y;
                }
                // set our position to the new destination
                gameObject.transform.position = v;
            }
            else if (++_currentPosition < _pathToMainTower.Length)
            {
                TargetPositionChanged(_pathToMainTower[_currentPosition]);
            }
            else
            {
                _isMoving = false;
                Debug.Log("path done!");
            }
        }
    }
}