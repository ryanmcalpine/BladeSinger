// reference: Dapper Dino @ YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private GameObject explosionVFX;

    void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.tag != "Player" )
        {
            Debug.Log( "Fireball hit " + other.gameObject.name );
            Instantiate( explosionVFX, transform.position, transform.rotation );
            Destroy( gameObject );
        }
    }
}
