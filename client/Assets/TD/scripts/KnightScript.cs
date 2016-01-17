using UnityEngine;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Управляет рыцарем.
    /// </summary>
    public class KnightScript : Unit
    {
        private float _sinceNewPositionSet;
        private Vector3 _prevPosition;
        private Vector3 _nextPosition;
        private Vector3 _newPosition;

        private void Start()
        {
            _prevPosition = transform.position;
        }

        private void Update()
        {
            _sinceNewPositionSet += Time.deltaTime;
            var extrapolatedPosition = Vector3.LerpUnclamped(_prevPosition, _newPosition, _sinceNewPositionSet);
            transform.position = extrapolatedPosition;
            //Debug.Log(extrapolatedPosition);
        }

        public void SetNewPosition(Vector3 newPosition)
        {
            _prevPosition = _newPosition;
            _newPosition = newPosition;
            _nextPosition = PredictNextPosition();
            //transform.position = newPosition;
            _sinceNewPositionSet = 1F;

            //Debug.Log(string.Format("new knight position. prev: {0}, new: {1}, next: {2}", _prevPosition, _newPosition, _nextPosition));
        }

        private Vector3 PredictNextPosition()
        {
            return Vector3.LerpUnclamped(_prevPosition, _newPosition, 2F);
        }
    }
}
