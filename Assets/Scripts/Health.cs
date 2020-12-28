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
    [SerializeField] private Rigidbody[] ragdollRBs;
    public Rigidbody[] RagdollRBs
    {
        get { return ragdollRBs; }
    }

    //private List<Rigidbody> ragdollRBs;

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
            ragdollRBs = GetRagdollRBs();
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if( currentHealth <= 0 && !isDead ) // just in case
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
        if( currentHealth <= 0 )
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log( "IsDead = true" );
        isDead = true;

        if( hasRagdoll )
        {
            Debug.Log( "Enabling ragdoll" );
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

    private Rigidbody[] GetRagdollRBs()
    {
        Rigidbody[] rbs = ragdollModel.GetComponentsInChildren<Rigidbody>( true );
        return rbs;
    }
    
    /*
    private List<Rigidbody> GetRagdollRBs( GameObject go )
    {
        Transform[] children = go.GetComponentsInChildren<Transform>();
        
        foreach( Transform child in children )
        {
            if( child.gameObject.GetComponent<Rigidbody>() != null )
            {
                ragdollRBs.Add( child.gameObject.GetComponent<Rigidbody>() )
            }
            GetRagdollRBs( child.gameObject );
        }
    }
    */
}
