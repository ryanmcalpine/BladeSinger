using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public Health health;
    public Animator anim;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float minWanderDistance;
    [SerializeField] private float maxWanderDistance;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float attackRange = 1f;
    private float idleStartTime;
    [SerializeField] private float idleTimer;
    [SerializeField] private Vector3 targetPosition; // for movement
    private Transform target;   // for chasing player
    private bool isChasing = false;

    void Start()
    {
        health = gameObject.GetComponent<Health>();
        anim.SetBool( "isIdling", true );
        targetPosition = transform.position;
        idleTimer = UnityEngine.Random.Range( minIdleTime, maxIdleTime );
    }

    void FixedUpdate()
    {
        if( health.IsDead )
        {
            return;
        }

        if( !isChasing )
        {
            // If we are idling, stay here and tick the timer.
            if( anim.GetBool( "isIdling" ) == true )
            {
                targetPosition = transform.position;
                idleTimer -= Time.deltaTime;
            }
            // If we are not yet idling but we have reached our next wander
            // destination, then idle there for a bit.
            else if( Vector3.Distance( transform.position, targetPosition ) <= 0.5f )
            {
                anim.SetBool( "isIdling", true );
                idleTimer = UnityEngine.Random.Range( minIdleTime, maxIdleTime );
            }
            // If we are done idling, pick a new destination to move towards
            if( anim.GetBool( "isIdling" ) == true && idleTimer <= 0 )
            {
                anim.SetBool( "isIdling", false );

                SetRandomWanderPosition();
            }
        }

        if( Vector3.Distance( targetPosition, transform.position ) > 0.5f )
        {
            transform.position = Vector3.MoveTowards( transform.position, targetPosition, moveSpeed * Time.deltaTime );
        }
    }

    // Sets a random location to wander to (test function, only for flat ground)
    private void SetRandomWanderPosition()
    {
        Vector3 dir = new Vector3( UnityEngine.Random.Range( -1f, 1f ), 0f, UnityEngine.Random.Range( -1f, 1f ) ).normalized;
        float dist = UnityEngine.Random.Range( minWanderDistance, maxWanderDistance );
        targetPosition = transform.position + ( dir * dist );
        targetPosition.y = 0;

        Quaternion lookRot = Quaternion.LookRotation( dir );
        Vector3 rot = lookRot.eulerAngles;
        transform.rotation = Quaternion.Euler( 0f, rot.y, 0f );
    }
}
