using System.Collections.Generic;
using UnityEngine;

namespace Assets.TD.scripts
{
    public class ByDistanceComparer : IComparer<GameObject>
    {
        private Vector3 Position { get; set; }

        public ByDistanceComparer(Vector3 position)
        {
            Position = position;
        }
        
        public int Compare(GameObject x, GameObject y)
        {
            var dstToA = Vector3.Distance(Position, x.transform.position);
            var dstToB = Vector3.Distance(Position, y.transform.position);
            return dstToA.CompareTo(dstToB);
        }
    }
}