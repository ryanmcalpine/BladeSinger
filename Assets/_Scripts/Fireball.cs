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

    [SerializeField] private int fireballDmg; // damage dealt from one meter away
    [SerializeField] private float explosionForce;
    [SerializeField] private float maxExplosionRadius = 10; // Things farther away are ignored for performance reasons

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

            // do explosion effects to nearby things
            Collider[] colliders = Physics.OverlapSphere(transform.position, maxExplosionRadius);
            foreach(Collider col in colliders ) {
                // Do stuff if we hit an enemy
                if( col.gameObject.tag == "Enemy" )
                {
                    // Deal damage to enemy. Currenty scales by inverse square, caps out at 0.2 meter distance
                    Health enemyHealth = col.gameObject.GetComponent<Health>();
                    float damageDealt = ( (float)fireballDmg / ((col.transform.position - transform.position).sqrMagnitude + 0.04f )); 
                    
                    enemyHealth.TakeDamage( (int)damageDealt);
                    
                    // Debug.Log( "F - Calling coroutine" );
                    // Delay explosion force til next frame to ensure ragdoll is activated
                    StartCoroutine( DelayedExplosionForce( enemyHealth ) );
  
                }

            }

            //Debug.Log( "F - Destroying gameObject" );
            Destroy( gameObject );
            
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
                Vector3 displacement = rb.transform.position - transform.position; // displacement between the joint and the explosion
                Vector3 direction = Vector3.Normalize(displacement);
                // The force the explsion imparts on the limb is in proportion to the inverse of the distance.
                // Modified so that at about a tenth of a meter, going closer won't increase the force. This is to avoid division by zero errors.
                // float localForce =  explosionForce / (displacement.sqrMagnitude + 0.01f); // inverse-square law is for energy, not force
                float localForce = explosionForce/(displacement.magnitude + 0.1f);

                //Debug.Log( "Explosion! Force added to " + rb.gameObject.name );
                //Debug.Log( "PushPoint: " + pushPoint.x + ", " + pushPoint.y + ", " + pushPoint.z );
                rb.AddForceAtPosition( direction * localForce, pushPoint, ForceMode.Impulse );
            }
        }
        
    }
}
