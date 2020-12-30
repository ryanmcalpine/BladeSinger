// by Ryan McAlpine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    public PlayerController player;

    [SerializeField] private Transform castPoint;
    [SerializeField] private ProgressBar chargeBar;
    [SerializeField] private ProgressBar manaBar;

    [Header( "Mana" )]
    [SerializeField] private float manaMax = 100f;
    [SerializeField] private float manaRecharge;
    private float manaCurrent;

    [Header( "Fireball" )]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed;   // this could make for a fun upgrade - starts out real slow, evolves to be a laser nuke
    [SerializeField] private float fireballChargeTime = 2f;
    [SerializeField] private int fireballManaCost = 15;
    
    private float spellChargeTimer = 0f;
    private Vector3 projectileDestination;

    // This enum will be used when multiple spells are available
    private enum SelectedSpell
    {
        Fireball,
    }
    private SelectedSpell selectedSpell = SelectedSpell.Fireball;

    void Start()
    {
        player.anim.SetBool( "isCasting", false );
        manaCurrent = manaMax;
    }

    void Update()
    {
        // Q down: start casting
        // Q held: charge spell
        // Q released: cast spell & reset charge amount
        if( Input.GetKeyDown(KeyCode.Q) )
        {
            player.anim.SetBool("isCasting", true);
        }
        if( Input.GetKey( KeyCode.Q ) )
        {
            spellChargeTimer += Time.fixedDeltaTime;

            // Use our enum to get necessary charge time
            switch( selectedSpell )
            {
                case SelectedSpell.Fireball:
                    chargeBar.SetFill( spellChargeTimer / fireballChargeTime );
                    break;
            }
        }
        if( Input.GetKeyUp( KeyCode.Q ) )
        {
            CastSpell();
            player.anim.SetBool( "isCasting", false );

            // spell has been cast (or fizzled if uncharged),
            // so reset charge bar and charge timer
            chargeBar.SetFill( 0f );
            spellChargeTimer = 0;
        }

        // Recharge mana
        if( manaCurrent < manaMax )
        {
            manaCurrent += manaRecharge * Time.fixedDeltaTime;
        }
        else
        {
            manaCurrent = manaMax;
        }

        // Update UI mana bar
        manaBar.SetFill( manaCurrent / manaMax );
    }

    void CastSpell()
    {
        // Use enum again to call spell function
        switch( selectedSpell )
        {
            case SelectedSpell.Fireball:
                // Make sure spell is charged && we have enough mana
                if( spellChargeTimer >= fireballChargeTime && manaCurrent >= fireballManaCost )
                {
                    player.anim.SetTrigger( "cast" );
                    manaCurrent -= fireballManaCost;
                    Fireball();
                }
                break;
        }
    }

    void Fireball()
    {
        Ray ray = player.GetLookDirRay();
        RaycastHit hit;
        if( Physics.Raycast( ray, out hit ) )
        {
            //Debug.Log( "Raycast hit " + hit.collider.name );
            projectileDestination = hit.point;
        }
        else
        {
            projectileDestination = ray.GetPoint( 1000 );
        }

        GameObject projectile = Instantiate( fireballPrefab, castPoint.position, Quaternion.identity ) as GameObject;

        projectile.GetComponent<Rigidbody>().velocity = (projectileDestination - castPoint.position).normalized * fireballSpeed;
    }
}
