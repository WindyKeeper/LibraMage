using LibraMage.Entities;
using UnityEngine;

namespace LibraMage.Results
{
    public struct TangentCircleResult
    {
        private Circle circle;
        private Vector2 firstIntersectionPoint;
        private Vector2 secondIntersectionPoint;

        public Circle Circle
        {
            get
            {
                return circle;
            }
        }

        public Vector2 FirstIntersectionPoint
        {
            get
            {
                return firstIntersectionPoint;
            }
        }

        public Vector2 SecondIntersectionPoint
        {
            get
            {
                return secondIntersectionPoint;
            }
        }

        internal TangentCircleResult(Circle circle, Vector2 firstIntersectionPoint, Vector2 secondIntersectionPoint)
        {
            this.circle = circle;
            this.firstIntersectionPoint = firstIntersectionPoint;
            this.secondIntersectionPoint = secondIntersectionPoint;
        }
    }
}