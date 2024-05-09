using System;
using UnityEngine;
using UnityEngine.AI;

namespace OfficeFood.Enemy
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Human.Human), typeof(FieldOfView))]
    public class Enemy : MonoBehaviour
    {
        public event Action<Enemy, float> OnDetectionChange;

        public EnemyState State 
        { 
            get => _state;
            private set
            {
                LastState = _state;
                _state = value;
            }
        }
        private EnemyState _state;
        public EnemyState LastState { get; private set; }

        [Header("Detection")]
        public float detectTime = 0;
        public AnimationCurve detectionDistanceCurve;
        public float detectCoolDownMult = 1;
        private float _detectTimer = 0;

        [Header("Pausing")]
        public float patrolPauseTime;
        public float lostTargetPauseTime;
        public float wanderPauseTime;
        private float _pauseEnd = float.NaN;
        private EnemyState _stateAfterPause = EnemyState.Patrolling;

        [Header("Wandering")]
        public float wanderTime;
        public float wanderDistance;
        private Vector2? _wanderStartPos;
        private Vector2 _wanderPos;
        private float _wanderEndTime;

        [Header("Patrolling")]
        public Vector2[] patrolPoints;
        public float patrolStopDistance;
        public float pathStopDistance;
        private int _targetPatrol;
        private int _lastPatrolPoint = -1;
        private Vector3 CurrentPathPos => _pathPoint < _agent.path.corners.Length 
            ? _agent.path.corners[_pathPoint] : transform.position;
        private int _pathPoint = 0;

        private NavMeshAgent _agent;
        private Human.Human _human;
        private FieldOfView _fov;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = 0;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.updatePosition = false;

            _human = GetComponent<Human.Human>();

            _fov = GetComponent<FieldOfView>();

            _targetPatrol = 0;
        }

        private void Update()
        {
            // Update necessary props on other objects
            _agent.nextPosition = transform.position;
            _fov.direction = _human.FaceDirection;

            // If no targets and back to patrolling, detect timer slow down
            if (_fov.VisibleTargets.Count == 0 && 
                (_state == EnemyState.Patrolling || (LastState == EnemyState.Patrolling && _state == EnemyState.Paused)))
            {
                _detectTimer = Mathf.Max(_detectTimer - Time.deltaTime * detectCoolDownMult, 0);
            }
            else if (_fov.VisibleTargets.Count != 0)
            {
                // Update detect timer
                // Scale detection based off distance to target
                Transform target = _fov.ClosestTarget;
                float distancePercent = (target.position - transform.position).magnitude / _fov.viewRadius;
                _detectTimer += Time.deltaTime * detectionDistanceCurve.Evaluate(1 - distancePercent) * 3;
                if (_detectTimer > detectTime)
                {
                    _detectTimer = detectTime;
                }
            }
            OnDetectionChange?.Invoke(this, _detectTimer / detectTime);
        }

        private void FixedUpdate()
        {
            playerColliding = _playerColliding && _state == EnemyState.Following;

            // Always prioritize finding targets no matter what state
            if (_fov.VisibleTargets.Count > 0 && _detectTimer >= detectTime)
            {
                SetDestination(_fov.ClosestTarget.position);
                _pathPoint = 0;
                State = EnemyState.Following;
            }

            // Sometimes the agent goes off the navmesh and the path gets
            // recalculated, so this is needed
            _pathPoint %= _agent.path.corners.Length;
            
            // If in path stop distance, go to next path point
            if ((CurrentPathPos - transform.position).magnitude < pathStopDistance)
            {
                _pathPoint = (_pathPoint + 1) % _agent.path.corners.Length;
            }

            switch (_state)
            {
                case EnemyState.Paused:
                    if (Time.time >= _pauseEnd)
                    {
                        State = _stateAfterPause;
                    }
                    break;
                case EnemyState.Following:
                    // If no targets and patrol path distance is close, go back
                    // to patrol
                    if ((transform.position - _agent.pathEndPosition).magnitude < pathStopDistance 
                        && _fov.VisibleTargets.Count == 0)
                    {
                        _targetPatrol = -1;
                        _lastPatrolPoint = -2;
                        Pause(lostTargetPauseTime, EnemyState.Wandering);
                    }
                    break;
                case EnemyState.Patrolling:
                    if (_targetPatrol == -1)
                    {
                        // If there isn't a patrol point to go to, find the
                        // nearest one
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
                    }

                    if (patrolPoints.Length == 0)
                    {
                        return;
                    }

                    // If at patrol point, go to next one
                    if (((Vector2)transform.position - patrolPoints[_targetPatrol]).magnitude < patrolStopDistance)
                    {
                        _targetPatrol = (_targetPatrol + 1) % patrolPoints.Length;
                        Pause(patrolPauseTime, EnemyState.Patrolling);
                    }

                    // If current patrol point has changed, pathfind to the new one
                    if (_lastPatrolPoint != _targetPatrol)
                    {
                        SetDestination(patrolPoints[_targetPatrol]);
                        _pathPoint = 0;
                    }
                    break;
                case EnemyState.Wandering:
                    // Set startpos and wanderpos on starting wandering
                    if (_wanderStartPos == null) 
                    {
                        _wanderStartPos = transform.position;
                        GetRandomWanderPos();
                        // Sometimes the random pos will be outside the walls,
                        // so just try until a good pos
                        while (!SetDestination(_wanderPos))
                        {
                            GetRandomWanderPos();
                        }
                        _wanderEndTime = Time.time + wanderTime;
                    }

                    // If at wander pos, find new wander pos
                    if ((_wanderPos - (Vector2)transform.position).magnitude < patrolStopDistance)
                    {
                        GetRandomWanderPos();
                        while (!SetDestination(_wanderPos))
                        {
                            GetRandomWanderPos();
                        }
                        Debug.Log(_wanderPos);

                        // Stop wandering if wander time over
                        if (Time.time > _wanderEndTime)
                        {
                            _wanderStartPos = null;
                            Pause(wanderPauseTime, EnemyState.Patrolling);
                        }
                        else
                        {
                            Pause(wanderPauseTime, EnemyState.Wandering);
                        }
                    }

                    break;
            }

            // Update human movetarget
            if (_state != EnemyState.Paused)
            {
                _human.SetMoveTarget(CurrentPathPos);

                // Overshoot if current path point is not the last one
                if ((CurrentPathPos - transform.position).magnitude < patrolStopDistance)
                {
                    _human.ClearMoveTarget();
                }
            }

            _lastPatrolPoint = _targetPatrol;
        }

        public bool SetDestination(Vector2 destination)
        {
            _pathPoint = 0;
            return _agent.SetDestination(destination);
        }

        public void Pause(float time, EnemyState stateAfterPause)
        {
            _pauseEnd = Time.time + time;
            State = EnemyState.Paused;
            _stateAfterPause = stateAfterPause;

            // Don't ask why I needed to put this
            if (stateAfterPause == EnemyState.Paused)
            {
                Debug.LogWarning("Are you sure you want this? Pausing after pausing?");
            }
        }

        private void GetRandomWanderPos()
        {
            if (!_wanderStartPos.HasValue)
            {
                return;
            }

            Vector3 origin = UnityEngine.Random.insideUnitCircle.normalized * wanderDistance + _wanderStartPos.Value;
            NavMesh.SamplePosition(origin, out NavMeshHit hit, wanderDistance, _agent.areaMask);
            _wanderPos = hit.position;
        }

        // temporary for game functionality
        public bool playerColliding { get; private set; } = false;
        private bool _playerColliding = false;

        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    if (collision.transform && collision.transform.CompareTag("Player"))
        //    {
        //        _playerColliding = true;
        //    }
        //}
        //private void OnCollisionExit2D(Collision2D collision)
        //{
        //    if (collision.transform && collision.transform.CompareTag("Player"))
        //    {
        //        _playerColliding = false;
        //    }
        //}
    }
}
