// by Ryan McAlpine
// reference: Dapper Dino @ YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private AudioClip explosion_ac;
    [SerializeField] private ShakeEventData explosion_shake;
    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float maxShakeDistance;

    [SerializeField] private int fireballDmg;
    [SerializeField] private float explosionForce;

    void OnTriggerEnter( Collider other )
    {
        if( other.gameObject.tag != "Player" )  // Avoid any clipping with player model
        {
            //Debug.Log( "Fireball hit " + other.gameObject.name );

            // Make it go boom (vfx and sfx)
            Instantiate( explosionVFX, transform.position, transform.rotation );
            AudioController.Instance.PlaySound( explosion_ac, transform.position, true );

            // Shake camera too
            float distance = Vector3.Distance( PlayerController.Instance.GetComponent<Transform>().position, transform.position );
            explosion_shake.amplitude = shakeAmplitude * Mathf.Clamp01( 1f - ( distance / maxShakeDistance) );
            Camera.main.GetComponentInParent<CameraShake>().AddShakeEvent( explosion_shake ); 

            // Do stuff if we hit an enemy, otherwise just Destroy()
            if( other.gameObject.tag == "Enemy" )
            {
                // Deal damage to enemy
                Health enemyHealth = other.gameObject.GetComponent<Health>();
                enemyHealth.TakeDamage( fireballDmg );
                
                //Debug.Log( "F - Calling coroutine" );
                // Delay explosion force til next frame to ensure ragdoll is activated
                StartCoroutine( DelayedExplosionForce( enemyHealth ) );
            }
            else
            {
                Destroy( gameObject );
            }
        }
    }

    // Ragdoll is set active in Health script Update() function,
    // so delay adding force til next FixedUpdate() to ensure
    // it is active and recieves the force
    IEnumerator DelayedExplosionForce( Health enemyHealth )
    {
        //Debug.Log( "F - Waiting" );
        yield return new WaitForFixedUpdate();
        //Debug.Log( "F - Waited" );
        if( enemyHealth.IsDead )
        {
            Rigidbody[] rbs = enemyHealth.RagdollRBs;
            foreach( Rigidbody rb in rbs )
            {
                Vector3 pushPoint = rb.gameObject.GetComponent<Collider>().ClosestPointOnBounds( transform.position );
                Vector3 direction = rb.transform.position - transform.position;
                //Debug.Log( "Explosion! Force added to " + rb.gameObject.name );
                //Debug.Log( "PushPoint: " + pushPoint.x + ", " + pushPoint.y + ", " + pushPoint.z );
                rb.AddForceAtPosition( direction * explosionForce, pushPoint, ForceMode.Impulse );
            }
        }
        //Debug.Log( "F - Destroying gameObject" );
        Destroy( gameObject );
    }
}
