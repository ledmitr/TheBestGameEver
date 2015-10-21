using UnityEngine;

namespace Assets.TD.scripts
{
    public class TentScript : MonoBehaviour
    {
        private Vector3[] _pathToMainTower;

        public Vector3[] GetPath()
        {
            return _pathToMainTower;
        }

        public void Start()
        {
            _pathToMainTower = new[]
            {
                new Vector3(23.43F, 3.48F, 40.72F),
                new Vector3(21.93F, 2.22F, 35.85F),
                new Vector3(21.14F, 2.22F, 29.34F),
                new Vector3(20.29F, 2.22F, 24.23F),
                new Vector3(16.0F, 2.22F, 13.31F),
                new Vector3(8.73F, 2.22F, 0.18F),
                new Vector3(8.18F, 2.22F, -6.42F),
                new Vector3(5.54F, 3.00F, -8.56F)
            };
        }

        public void Update()
        {

        }
    }
}
