using System;
using System.Collections.Generic;
using UnityEngine;

namespace OfficeFood.Enemy
{
    public class FieldOfView : MonoBehaviour
    {
        public event Action<Transform> OnTargetEnter;
        public event Action<Transform> OnTargetExit;
        public event Action<Transform> OnTargetStay;

        public Vector2 direction;
        public float viewAngle;
        public float viewRadius;
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        public Transform ClosestTarget { get; private set; }
        public IReadOnlyCollection<Transform> VisibleTargets => _visibleTargets;
        private HashSet<Transform> _visibleTargets = new();
        private HashSet<Transform> _lastVisibleTargets = new();

        private void Update()
        {
            HashSet<Transform> newVisibleTargets = new();
            ClosestTarget = null;

            // Get all targets in a circle radius
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                // Filter out targets that aren't in the angle radius
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(direction, dirToTarget) >= viewAngle / 2)
                {
                    continue;
                }

                // Filter out targets that have something blocking the fov
                if (Physics2D.Linecast(transform.position, target.position, obstacleMask))
                {
                    continue;
                }

                newVisibleTargets.Add(target);
                if (ClosestTarget == null) 
                { 
                    ClosestTarget = target;
                }

                // If currently visible, invoke stay
                if (_visibleTargets.Contains(target)) 
                {
                    OnTargetStay?.Invoke(target); 
                }
                else if (_lastVisibleTargets.Contains(target))
                {
                    // If visible before, but not now, exit
                    OnTargetExit?.Invoke(target);
                }
                else
                {
                    // If not visible before, and not visible now, enter
                    OnTargetEnter?.Invoke(target);
                }
            }

            _lastVisibleTargets = _visibleTargets;
            _visibleTargets = newVisibleTargets;
        }
    }
}