using UnityEngine;
using UnityEditor;

namespace OfficeFood.Enemy
{
    [CustomEditor(typeof(Enemy))]
    public class EnemyEditor : Editor 
    {
        void OnSceneGUI()
        {
            Enemy enemy = (Enemy)target;
            Human.Human human = enemy.GetComponent<Human.Human>();
            Handles.color = Color.white;

            Vector2 dir = human.moveTarget.magnitude < 0.01 ? Vector2.down : human.moveTarget.normalized;

            Vector3 leftDir = dir.Rotate(enemy.viewAngle / 2);
            Vector3 rightDir = dir.Rotate(-enemy.viewAngle / 2);

            Handles.DrawWireArc(enemy.transform.position, Vector3.forward, rightDir, enemy.viewAngle, enemy.viewRadius);
            Handles.DrawLine(enemy.transform.position, enemy.transform.position + leftDir * enemy.viewRadius);
            Handles.DrawLine(enemy.transform.position, enemy.transform.position + rightDir * enemy.viewRadius);

            Handles.color = Color.red;
            foreach (Transform visibleTarget in enemy.VisibleTargets)
            {
                Handles.DrawLine(enemy.transform.position, visibleTarget.position);
            }

            // Draw patrol point things
            for (int i = 0; i < enemy.patrolPoints.Length; i++)
            {
                enemy.patrolPoints[i] = Handles.FreeMoveHandle(enemy.patrolPoints[i], 0.1f, Vector3.zero, Handles.DotHandleCap);
                Handles.DrawLine(enemy.patrolPoints[i], enemy.patrolPoints[(i + 1) % enemy.patrolPoints.Length]);
            }
        }
    }
}
