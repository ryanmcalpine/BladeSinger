using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference: bramdal @ github

public class SwordBehaviour : MonoBehaviour
{
    [SerializeField] int swordDamage = 15;

    private void OnCollisionEnter( Collision other )
    {
        if( other.gameObject.tag == "Enemy" )
        {
            Debug.Log( "Attacked " + other.gameObject.name );
            Health enemyHealth = other.gameObject.GetComponent<Health>();
            enemyHealth.TakeDamage( swordDamage );
        }
    }
}
