// reference: Dapper Dino @ YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private int fireballDmg;
    [SerializeField] private float explosionForce;

    void OnTriggerEnter( Collider other )
    {
        if( other.gameObject.tag != "Player" )
        {
            Debug.Log( "Fireball hit " + other.gameObject.name );

            Instantiate( explosionVFX, transform.position, transform.rotation );

            if( other.gameObject.tag == "Enemy" )
            {
                Health enemyHealth = other.gameObject.GetComponent<Health>();
                enemyHealth.TakeDamage( fireballDmg );
                //other.gameObject.GetComponent<Rigidbody>().velocity += gameObject.GetComponent<Rigidbody>().velocity;
                Component[] rbs = enemyHealth.GetRagdollRBs();

                StartCoroutine( DelayedExplosionForce( rbs ) );
                /*
                foreach( Rigidbody rb in rbs )
                {
                    Vector3 pushPoint = rb.gameObject.GetComponent<Collider>().ClosestPointOnBounds( transform.position );
                    Debug.Log( "Explosion! Force added to " + rb.gameObject.name );
                    rb.AddForceAtPosition( pushPoint, GetComponent<Rigidbody>().velocity.normalized * explosionForce, ForceMode.Impulse );
                }
                */
            }
            else
            {
                Destroy( gameObject );
            }

           // Destroy( gameObject );
        }
    }

    // Ragdoll is set active in Health script Update() function,
    // so delay adding force til next FixedUpdate() to ensure
    // it is active and recieves the force
    // (This may or may not be necessary - it wasn't working in
    // OnTriggerEnter so I tried this way but still not seeing results)
    IEnumerator DelayedExplosionForce( Component[] rbs )
    {
        yield return new WaitForFixedUpdate();
        foreach( Rigidbody rb in rbs )
        {
            Vector3 pushPoint = rb.gameObject.GetComponent<Collider>().ClosestPointOnBounds( transform.position );
            Debug.Log( "Explosion! Force added to " + rb.gameObject.name );
            Debug.Log( "PushPoint: " + pushPoint.x + ", " + pushPoint.y + ", " + pushPoint.z );
            rb.AddForceAtPosition( pushPoint, GetComponent<Rigidbody>().velocity.normalized * explosionForce, ForceMode.Impulse );
        }
        Destroy( gameObject );
    }
}
