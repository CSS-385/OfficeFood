using UnityEngine;
using UnityEditor;

namespace OfficeFood.Enemy
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView)target;

            Handles.color = Color.white;
            Vector3 leftDir = fov.direction.Rotate(fov.viewAngle / 2);
            Vector3 rightDir = fov.direction.Rotate(-fov.viewAngle / 2);

            Handles.DrawWireArc(fov.transform.position, Vector3.forward, rightDir, fov.viewAngle, fov.viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + leftDir * fov.viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + rightDir * fov.viewRadius);

            Handles.color = Color.red;
            foreach (Transform visibleTarget in fov.VisibleTargets)
            {
                Handles.DrawLine(fov.transform.position, visibleTarget.position);
            }
        }
    }
}
