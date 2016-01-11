using System.Collections.Generic;
using UnityEngine;

namespace Assets.TD.scripts.Utils
{
    /// <summary>
    /// Сравнивает объекты по расстоянию до указанной точки.
    /// </summary>
    public class ByDistanceComparer : IComparer<GameObject>
    {
        private Vector3 Position { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="position">Точка, по которой осуществляется сравнение.</param>
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