using UnityEngine;

namespace Assets.TD.scripts
{
    public class RotationFreezer : MonoBehaviour
    {
        private Quaternion _initialRotation;

        private void Start()
        {
            _initialRotation = transform.rotation;
        }

        private void LateUpdate()
        {
            transform.rotation = _initialRotation;
        }
    }
}
