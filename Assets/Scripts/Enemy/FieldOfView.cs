using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FieldOfView : MonoBehaviour
{
    public Transform target;

    private Vector3 _lastTarget;
    private NavMeshAgent _agent;

    public float viewRadius;

    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;  

    [SerializeField]
    private float detectTimer = 0;
    [SerializeField]
    private float detectTime = 0;

    public Transform[] patrolPoints;
    public int targetPoint;



    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        StartCoroutine("FindTargetsWithDelay", .2);
        targetPoint = 0;



    }
    private void Update()
    {

        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
        Debug.Log(transform.position.x);
        Debug.Log(patrolPoints[targetPoint].position.x);

        if ((int)transform.position.x == (int)patrolPoints[targetPoint].position.x && (int)transform.position.y == (int)patrolPoints[targetPoint].position.y)
        {
            targetPoint++; 
        }
        if(targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
        if (targetsInViewRadius.Length == 0 && detectTimer < detectTime)
        {
            _agent.SetDestination(patrolPoints[targetPoint].position);

        }
        else if(targetsInViewRadius.Length != 0 && detectTimer < detectTime)
        {
            _agent.SetDestination(patrolPoints[targetPoint].position);


        }
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();

        }
    }

    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));

    }
    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform Target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (Target.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float disToTarget = Vector3.Distance(transform.position, Target.position);
                Debug.Log(!Physics2D.Raycast(transform.position, dirToTarget, disToTarget, obstacleMask));
                if (!Physics2D.Raycast(transform.position, dirToTarget, disToTarget, obstacleMask))
                {  
                    detectTimer += Time.deltaTime;
                    if (detectTimer > detectTime)
                    {
                        detectTimer = detectTime;
                        _agent.SetDestination(target.position);
                        visibleTargets.Add(Target);

                    }
                    if(detectTimer < detectTime)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[0].position, 5 * Time.deltaTime);

                    }
                }
                else 
                {
                    Debug.Log("Else");
                    detectTimer = 0;

                }
            }
        }
        if(targetsInViewRadius.Length == 0)
        {
            if(detectTimer > 0)
            {
               detectTimer -= Time.deltaTime;

            }
            else
            {
                detectTimer = 0;
            }


        }
    }
}
