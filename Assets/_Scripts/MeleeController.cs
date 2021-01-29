// by Ryan McAlpine
// references: bramdal @ github

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    public PlayerController player;
    public BoxCollider bladeCollider;
    private float colliderTimerMax = 3f;
    private float colliderTimer = 2f;

    void Start()
    {
        bladeCollider.enabled = false;
    }
    void Update()
    {
        Block();

        if( Input.GetMouseButtonUp( 0 ) )
        {
            Attack( player.CameraMoveDir );
        }

        Feint();
    }
    void FixedUpdate()
    {
        if( bladeCollider.enabled )
        {
            colliderTimer -= Time.deltaTime;

            if( colliderTimer <= 0 )
            {
                bladeCollider.enabled = false;
            }
        }
    }

    void Block()
    {
        if( Input.GetMouseButtonDown( 1 ) )
        {

        }
    }

    void Attack( int dir )
    {
        Debug.Log( "Attack Direction: " + dir );

        // todo: if not blocking
        bladeCollider.enabled = true;
        colliderTimer = colliderTimerMax;
        switch( dir )
        {
            case 0:
                player.anim.SetTrigger( "atk0" );
                break;
            case 1:
                player.anim.SetTrigger( "atk1" );
                break;
            case 2:
                player.anim.SetTrigger( "atk2" );
                break;
            case 3:
                player.anim.SetTrigger( "atk3" );
                break;
            case 4:
                player.anim.SetTrigger( "atk4" );
                break;
            case 5:
                player.anim.SetTrigger( "atk5" );
                break;
                /*
            case 6:
                player.anim.SetTrigger( "atk6" );
                break;
            case 7:
                player.anim.SetTrigger( "atk7" );
                break;
                */
        }
    }

    void Feint()
    {

    }
}
