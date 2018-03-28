using UnityEngine;

namespace LibraMage
{
    public static class Physicsga
    {
        public static TrajectoryRenderer CreateTrajectoryRenderer(Transform parentTransform)
        {
            GameObject trajectoryRenderer = new GameObject();
            trajectoryRenderer.transform.parent = parentTransform;
            trajectoryRenderer.transform.localPosition = Vector3.zero;
            trajectoryRenderer.name = "TrajectoryRenderer";
            trajectoryRenderer.hideFlags = HideFlags.HideInHierarchy;
            
            return trajectoryRenderer.AddComponent<TrajectoryRenderer>();
        }
    }
}