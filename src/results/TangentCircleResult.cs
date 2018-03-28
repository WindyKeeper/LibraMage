using UnityEngine;

namespace LibraMage
{
    public struct TangentCircleResult
    {
        public Circle Circle { get; }
        public Vector2 FirstIntersectionPoint { get; }
        public Vector2 SecondIntersectionPoint { get; }

        internal TangentCircleResult(Circle circle, Vector2 firstIntersectionPoint, Vector2 secondIntersectionPoint)
        {
            Circle = circle;
            FirstIntersectionPoint = firstIntersectionPoint;
            SecondIntersectionPoint = secondIntersectionPoint;
        }
    }
}