using Assets.TD.scripts.Enums;
using UnityEngine;

namespace Assets.TD.scripts
{
    public class GameMap
    {
        public int[][] Map { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        
        private readonly Vector2[] _pointsAroundTent = {
                Vector2.up,
                Vector2.down,
                Vector2.left,
                Vector2.right,
                Vector2.up + Vector2.left,
                Vector2.up + Vector2.right,
                Vector2.down + Vector2.left,
                Vector2.down + Vector2.right
            };

        public Vector2 CalcTentClosestRoad(Vector2 tentPosition)
        {
            foreach (var point in _pointsAroundTent)
            {
                int x = (int) (point.x + tentPosition.x);
                int y = (int) (point.y + tentPosition.y);
                if (Map[x][y] == (int) MapCellType.Road)
                    return point;
            }
            return Vector2.zero;
        }

        public Vector2 TentPosition { get; set; }
        public Vector2 FortressPosition { get; set; }
    }
}