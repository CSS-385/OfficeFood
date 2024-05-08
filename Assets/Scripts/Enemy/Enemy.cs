using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OfficeFood.Enemy
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Human.Human))]
    public class Enemy : MonoBehaviour
    {
        public event Action<Enemy, float> OnDetectionChange;

        public float viewRadius;
        [Range(0, 360)]
        public float viewAngle;

        public LayerMask targetMask;
        public LayerMask obstacleMask;

        public float detectTime = 0;
        public AnimationCurve detectionDistanceCurve;
        public float detectCoolDownMult = 1;
        private float detectTimer = 0;

        public float patrolPauseTime;
        public float lostTargetPauseTime;
        private float _pauseStart = float.NaN;
        public bool IsPaused => !float.IsNaN(_pauseStart);

        public Vector2[] patrolPoints;
        public float patrolStopDistance;
        public float pathStopDistance;
        private int _targetPatrol;
        private int _lastPatrolPoint = -1;
        private int _pathPoint = 0;
        private int _lastPathPoint = -1;

        public IReadOnlyList<Transform> VisibleTargets => _visibleTargets;
        private readonly List<Transform> _visibleTargets = new();
        private bool _followingTarget;

        private NavMeshAgent _agent;
        private Human.Human _human;

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = 0;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.updatePosition = false;

            _human = GetComponent<Human.Human>();

            _targetPatrol = 0;
        }

        private void FixedUpdate()
        {
            playerColliding = _playerColliding && _followingTarget;

            _agent.nextPosition = transform.position;
            FindVisibleTargets();

            // Sometimes the agent goes off the navmesh and the path gets
            // recalculated, so this is needed
            _pathPoint %= _agent.path.corners.Length;
            if ((_agent.path.corners[_pathPoint] - transform.position).magnitude < pathStopDistance)
            {
                if (_followingTarget && _visibleTargets.Count == 0)
                {
                    if (!IsPaused)
                    {
                        _pauseStart = Time.time;
                    }
                    else if (Time.time - _pauseStart > lostTargetPauseTime)
                    {
                        // If following target, but no target anymore, find nearest
                        // patrol point to snap back to
                        Vector2 pos = transform.position;
                        int closest = 0;
                        for (int i = 1; i < patrolPoints.Length; i++)
                        {
                            if ((pos - patrolPoints[i]).magnitude < (pos - patrolPoints[closest]).magnitude)
                            {
                                closest = i;
                            }
                        }

                        _targetPatrol = closest;
                        _lastPatrolPoint = -1;
                        _followingTarget = false;
                        _pauseStart = float.NaN;
                    }
                }
                else if (!IsPaused)
                {
                    // If not then just go to next corner or path
                    _pathPoint = (_pathPoint + 1) % _agent.path.corners.Length;
                }
            }

            if (_visibleTargets.Count == 0 && patrolPoints.Length > 0)
            {
                // If at patrol point, go to next one
                if (((Vector2)transform.position - patrolPoints[_targetPatrol]).magnitude < patrolStopDistance)
                {
                    if (!IsPaused)
                    {
                        _pauseStart = Time.time;
                    }
                    else if (Time.time - _pauseStart > patrolPauseTime)
                    {
                        _targetPatrol = (_targetPatrol + 1) % patrolPoints.Length;
                        _pauseStart = float.NaN;
                    }
                }

                // If current patrol point has changed, pathfind to the new one
                if (_lastPatrolPoint != _targetPatrol)
                {
                    _agent.SetDestination(patrolPoints[_targetPatrol]);
                    _pathPoint = 0;
                    _lastPathPoint = -1;
                }
            }
            else if (_visibleTargets.Count > 0)
            {
                // Go to closest visible target if there is one
                _agent.SetDestination(_visibleTargets[0].position);
                _pauseStart = float.NaN;
                _followingTarget = true;
            }

            // Update human movetarget
            _human.SetMoveTarget(_agent.path.corners[_pathPoint]);
            if ((_agent.path.corners[_pathPoint] - transform.position).magnitude < patrolStopDistance)
            {
                _human.ClearMoveTarget();
            }

            _lastPathPoint = _pathPoint;
            _lastPatrolPoint = _targetPatrol;
        }

        private void FindVisibleTargets()
        {
            _visibleTargets.Clear();
            // Get the direction of the enemy
            Vector2 dir = _human.FaceDirection;// _human.moveTarget.magnitude < 0.01 ? Vector2.down : _human.moveTarget.normalized;

            // Get all targets in a circle radius
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                // Filter out targets that aren't in the angle radius
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(dir, dirToTarget) >= viewAngle / 2)
                {
                    continue;
                }

                // Filter out targets that have something blocking the fov
                if (Physics2D.Linecast(transform.position, target.position, obstacleMask))
                {
                    continue;
                }

                // Update detect timer
                // Scale detection based off distance to target
                float distancePercent = (target.position - transform.position).magnitude / viewRadius;
                detectTimer += Time.deltaTime * detectionDistanceCurve.Evaluate(1 - distancePercent);
                if (detectTimer > detectTime)
                {
                    detectTimer = detectTime;
                    _visibleTargets.Add(target);
                }
            }

            if (targetsInViewRadius.Length == 0)
            {
                detectTimer = Mathf.Max(detectTimer - Time.deltaTime * detectCoolDownMult, 0);
            }
            OnDetectionChange?.Invoke(this, detectTimer / detectTime);
        }

        // temporary for game functionality
        public bool playerColliding { get; private set; } = false;
        private bool _playerColliding = false;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform && collision.transform.CompareTag("Player"))
            {
                _playerColliding = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.transform && collision.transform.CompareTag("Player"))
            {
                _playerColliding = false;
            }
        }
    }
}
