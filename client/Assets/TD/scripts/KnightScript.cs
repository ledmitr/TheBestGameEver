using UnityEngine;

namespace Assets.TD.scripts
{
    public class KnightScript : MonoBehaviour
    {
        public int Health { get; private set; }

        public Vector3 TargetPosition { get; private set; }

        public bool IsSelected { get; private set; }

        public void Select()
        {
            IsSelected = true;
        }

        private bool _isMoving;

        private float _speed = 1.5F;
        
        public void Start()
        {
            Health = 100;
            _isMoving = false;
            IsSelected = false;
        }

        public void TargetPositionChanged(Vector3 newTargetPosition)
        {
            TargetPosition = newTargetPosition;
            IsSelected = false;
            _isMoving = true;
        }

        public void Update()
        {
            if (_isMoving)
                Move();
        }

        private void Move()
        {
            var targetPostionReached = Mathf.Approximately(gameObject.transform.position.magnitude, TargetPosition.magnitude);
            if (!targetPostionReached)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, TargetPosition,
                    1/(_speed * Vector3.Distance(gameObject.transform.position, TargetPosition)));
            }
            else
            {
                _isMoving = false;
            }
        }
    }
}