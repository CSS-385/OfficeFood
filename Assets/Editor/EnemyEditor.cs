using UnityEngine;
using UnityEditor;

namespace OfficeFood.Enemy
{
    [CustomEditor(typeof(Enemy))]
    public class EnemyEditor : Editor 
    {
        private void OnSceneGUI()
        {
            Enemy enemy = (Enemy)target;

            // Draw patrol point things
            Undo.RecordObject(enemy, "change patrol point");
            for (int i = 0; i < enemy.patrolPoints.Length; i++)
            {
                enemy.patrolPoints[i] = Handles.FreeMoveHandle(enemy.patrolPoints[i], 0.1f, Vector3.zero, Handles.DotHandleCap);
                Handles.DrawLine(enemy.patrolPoints[i], enemy.patrolPoints[(i + 1) % enemy.patrolPoints.Length]);
            }

            PrefabUtility.RecordPrefabInstancePropertyModifications(enemy);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Enemy enemy = (Enemy)target;
            EnemyState res = (EnemyState)EditorGUILayout.EnumPopup("State", enemy.State);
            if (enemy.State != res)
            {
                enemy.State = res;
            }
        }
    }
}
