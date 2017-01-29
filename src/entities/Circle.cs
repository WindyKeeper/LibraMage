using UnityEngine;

namespace LibraMage.Entities
{
    public struct Circle
    {
        private Vector2 center;
        private float radius;

        public Vector2 Center
        {
            get
            {
                return center;
            }
        }

        public float Radius
        {
            get
            {
                return radius;
            }
        }

        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }
}