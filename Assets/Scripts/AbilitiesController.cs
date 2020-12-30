using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    public PlayerController player;

    [SerializeField] private Transform castPoint;
    [SerializeField] private ProgressBar chargeBar;

    [Header( "Fireball" )]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed;   // this could make for a fun upgrade - starts out real slow, evolves to be a laser nuke
    [SerializeField] private float fireballChargeTime = 2f;
    
    private float spellChargeTimer = 0f;
    private Vector3 projectileDestination;

    // This enum will be used when multiple spells are available
    private enum SelectedSpell
    {
        Fireball,
    }
    private SelectedSpell selectedSpell = SelectedSpell.Fireball;

    // Start is called before the first frame update
    void Start()
    {
        player.anim.SetBool( "isCasting", false );
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Q) )
        {
            player.anim.SetBool("isCasting", true);
        }
        if( Input.GetKey( KeyCode.Q ) )
        {
            spellChargeTimer += Time.fixedDeltaTime;

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

            chargeBar.SetFill( 0f );
            spellChargeTimer = 0;
        }
    }

    void CastSpell()
    {
        switch( selectedSpell )
        {
            case SelectedSpell.Fireball:
                if( spellChargeTimer >= fireballChargeTime )
                {
                    player.anim.SetTrigger( "cast" );
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
            Debug.Log( "Raycast hit " + hit.collider.name );
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
