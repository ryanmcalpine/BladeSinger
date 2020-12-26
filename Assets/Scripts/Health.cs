// by Ryan McAlpine
// references: How To Do @ YouTube
// This script contains the necessary logic for starting with max
//      health, taking damage, dying, and (if applicable) activating
//      a ragdoll on death.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private bool hasRagdoll = false;
    [SerializeField] private GameObject animatedModel;
    [SerializeField] private GameObject ragdollModel;

    private int currentHealth;
    private bool isDead = false;
    public bool IsDead  // property
    {
        get { return isDead; }
    }

    void Awake()
    {
        animatedModel.SetActive( true );

        // Ragdoll remains inactive but its animator still keeps the models
        // in sync. Then on death they can be swapped out seamlessly
        if( hasRagdoll )
        {
            ragdollModel.SetActive( false );
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if( currentHealth <= 0 )
        {
            Debug.Log( gameObject.name + " has died. RIP :(" );
            Die();
        }
    }

    public void TakeDamage( int dmg )
    {
        Debug.Log( gameObject.name + " took " + dmg + " damage" );
        currentHealth -= dmg;
        Debug.Log( "Remaining health: " + currentHealth );
    }

    private void Die()
    {
        isDead = true;

        if( hasRagdoll )
        {
            ragdollModel.transform.parent = null;
            ragdollModel.GetComponent<Animator>().enabled = false;
            animatedModel.SetActive( false );
            ragdollModel.SetActive( true );
        }
        else
        {
            // TODO: implement death without ragdoll (just destroy gameobject?
            //       maybe spawn particles or somethin?)
        }
    }
}
