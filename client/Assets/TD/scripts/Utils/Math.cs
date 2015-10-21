using UnityEngine;

namespace Assets.TD.scripts.Utils
{
    public class Math
    {
        public static bool Approximately(float a, float b, float tolerance = 0.000001F)
        {
            return (Mathf.Abs(a - b) < tolerance);
        }
    }
}
