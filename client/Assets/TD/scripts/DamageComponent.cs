using System.Collections.Generic;
using UnityEngine;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Наносит урон.
    /// </summary>
    public class DamageComponent : MonoBehaviour
    {
        private GameObject _closestTarget;

        public string TargetTag;

        private void Start()
        {

        }

        private void Update()
        {
            TryToDamageClosestTarget();
        }

        private void TryToDamageClosestTarget()
        {
            if (_closestTarget == null)
                _closestTarget = GetClosestTarget();
            
            if (Vector3.Distance(_closestTarget.transform.position, gameObject.transform.position) < DamageStats.Distance)
                _closestTarget.GetComponent<EnemyHealth>().Damage(DamageStats.Amount);
        }

        private GameObject GetClosestTarget()
        {
            // Get enemies as a list:
            var targets = GameObject.FindGameObjectsWithTag(TargetTag);
            var targetsList = new List<GameObject>(targets);

            // Sort the list by distance from ourselves
            targetsList.Sort(new ByDistanceComparer(GetComponent<Transform>().localPosition));
            var closestTower = targetsList[0];
            return closestTower;
        }
    }
}
