using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// by Ryan McAlpine
// references: bramdal @ github

public class MeleeController : MonoBehaviour
{
    public PlayerController player;

    void Update()
    {
        Block();

        if( Input.GetMouseButtonUp( 0 ) )
        {
            Attack( player.CameraMoveDir );
        }

        Feint();
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
            case 6:
                player.anim.SetTrigger( "atk6" );
                break;
            case 7:
                player.anim.SetTrigger( "atk7" );
                break;
        }
    }

    void Feint()
    {

    }
}
