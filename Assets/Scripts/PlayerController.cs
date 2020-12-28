// by Ryan McAlpine
// references: Acacia Developer @ YouTube, Board To Bits Games @ YouTube, bramdal @ github
// This class implements a first-person player controller with sprinting and jumping

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;

    [Header( "Camera" )]
    [SerializeField] Camera camera;
    [SerializeField] Transform cameraTransform;
    [SerializeField] float mouseSensitivity = 10.0f;
    [SerializeField] float mouseSmoothTime = 0.05f;
    bool lockCursor = true; // unused as of yet
    float cameraPitch = 0.0f;
    Vector2 currentMousePosition = Vector2.zero;
    Vector2 currentMouseVelocity = Vector2.zero;
    Vector2 targetMousePosition = Vector2.zero;

    private int cameraMoveDir = -1;
    // cameraMoveDir will represent the direction of movement of the cursor
    // 8 possible swing directions, organized like so:
    //  0 1 2
    //  3   4
    //  5 6 7
    public int CameraMoveDir
    {
        // Making a property allows other classes (i.e. the attack controller)
        // to read the variable but not change it
        get { return cameraMoveDir; }
    }
    float previousRotX = 0f;
    float previousRotY = 0f;


    [Header( "Movement" )]
    [SerializeField] const float walkSpeed = 6.0f;
    [SerializeField] const float runSpeed = 10.0f;
    [SerializeField] float moveSmoothTime = 0.2f;
    float currentMoveSpeed;
    CharacterController controller;
    Vector2 currentDir = Vector2.zero;
    Vector2 currentVelocity = Vector2.zero;
    Vector2 inputDir = Vector2.zero;
    float velocityY = 0.0f;
    bool jumpTrigger = false;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    [Header( "Audio" )]
    [SerializeField] AudioClip[] ac_footsteps;
    [SerializeField] private float walkSFXTime;  // time between footstep sfx when walking
    [SerializeField] private float sprintSFXTime;    // time between footstep sfx when sprinting
    private float footstepSFXTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if( lockCursor )
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        currentMoveSpeed = walkSpeed;
    }

    // Get input in Update
    void Update()
    {
        targetMousePosition = new Vector2( Input.GetAxis( "Mouse X" ), Input.GetAxis( "Mouse Y" ) );

        // get WASD input
        inputDir = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );
        inputDir.Normalize();

        // Handle shift-to-sprint input
        if( Input.GetKey( KeyCode.LeftShift ) )
        {
            currentMoveSpeed = runSpeed;
            anim.SetBool( "isRunning", true );
        }
        else
        {
            currentMoveSpeed = walkSpeed;
            anim.SetBool( "isRunning", false );
        }

        // Space == jump
        if( Input.GetKeyDown( KeyCode.Space ) )
        {
            jumpTrigger = true;
        }

    }
    // Then do physics work in FixedUpdate
    // FixedUpdate is used because it is unaffected by framerate
    // that means physics (movement) will have consistent speed
    void FixedUpdate()
    {
        UpdateMouseLook();
        UpdateMovement();

        GetCameraMoveDir();
        previousRotY = transform.eulerAngles.y; // to be used for the next call to Get...Dir()
        previousRotX = cameraTransform.localEulerAngles.x; // ^

        CheckGrounded();
    }

    void UpdateMouseLook()
    {
        currentMousePosition = Vector2.SmoothDamp( currentMousePosition, targetMousePosition, ref currentMouseVelocity, mouseSmoothTime );

        cameraPitch -= currentMousePosition.y * mouseSensitivity;    // invert y-axis movement to account for camera orientation
        cameraPitch = Mathf.Clamp( cameraPitch, -90.0f, 85.0f );

        cameraTransform.localEulerAngles = Vector3.right * cameraPitch;    // Camera is a child object so must use local angles

        transform.Rotate( Vector3.up * currentMousePosition.x * mouseSensitivity );
    }

    void UpdateMovement()
    {
        // Set anim bool & play SFX if receiving WASD input
        if( inputDir.x > 0 || inputDir.y > 0 )
        {
            anim.SetBool( "isMoving", true );

            // Play footstep sound effects, as long as we're not jumping
            if( controller.isGrounded )
            {
                AudioClip ac_footstep = ac_footsteps[Random.Range( 0, ac_footsteps.Length )];
                switch( currentMoveSpeed )
                {
                    case walkSpeed:
                        if( footstepSFXTimer <= 0 )
                        {
                            AudioController.Instance.PlaySound( ac_footstep, transform.position, true );
                            footstepSFXTimer = walkSFXTime;
                        }
                        footstepSFXTimer -= Time.deltaTime;
                        break;
                    case runSpeed:
                        if( footstepSFXTimer <= 0 )
                        {
                            AudioController.Instance.PlaySound( ac_footstep, transform.position, true );
                            footstepSFXTimer = sprintSFXTime;
                        }
                        footstepSFXTimer -= Time.deltaTime;
                        break;
                }
            }
            else
            {
                footstepSFXTimer = 0f;
            }
        }
        else
        {
            anim.SetBool( "isMoving", false );
            footstepSFXTimer = 0f;
        }

        currentDir = Vector2.SmoothDamp( currentDir, inputDir, ref currentVelocity, moveSmoothTime );

        if( controller.isGrounded )
        {
            velocityY = 0.0f;

            // Handle jumping
            if( jumpTrigger )
            {
                anim.SetTrigger( "jump" );
                velocityY = jumpForce;
            }

        }
        jumpTrigger = false;
        velocityY += Physics.gravity.y * Time.deltaTime;

        // "Better Jumping"
        if( velocityY < 0 )
        {
            // Fall faster
            velocityY += Physics.gravity.y * ( fallMultiplier - 1 ) * Time.deltaTime;
        }
        else if( velocityY > 0 && !Input.GetKey( KeyCode.Space ) )
        {
            // Hold space bar to jump higher
            velocityY += Physics.gravity.y * ( lowJumpMultiplier - 1 ) * Time.deltaTime;
        }

        Vector3 velocity = ( ( transform.forward * currentDir.y ) + ( transform.right * currentDir.x ) ) * currentMoveSpeed + ( Vector3.up * velocityY );

        controller.Move( velocity * Time.deltaTime );
    }

    void GetCameraMoveDir()
    {
        // Get change in x- and y- rotation using previousRot variables
        float deltaX = cameraTransform.localEulerAngles.x - previousRotX;
        float deltaY = transform.eulerAngles.y - previousRotY;

        if( deltaX <= -1f && deltaY <= -1f )
        {
            // moved up & left
            cameraMoveDir = 0;
        }
        else if( deltaX <= -1f && deltaY > -1f && deltaY < 1f )
        {
            // moved up
            cameraMoveDir = 1;
        }
        else if( deltaX <= -1f && deltaY >= 1f )
        {
            // moved up & right
            cameraMoveDir = 2;
        }
        else if( deltaX > -1f && deltaX < 1f && deltaY <= -1f )
        {
            // moved left
            cameraMoveDir = 3;
        }
        else if( deltaX > -1f && deltaX < 1f && deltaY >= 1f )
        {
            // moved right
            cameraMoveDir = 4;
        }
        else if( deltaX >= 1f && deltaY <= -1f )
        {
            // moved down and left
            cameraMoveDir = 5;
        }
        else if( deltaX >= 1f && deltaY > -1f && deltaY < 1f )
        {
            // moved down
            cameraMoveDir = 6;
        }
        else if( deltaX >= 1f && deltaY >= 1f )
        {
            // moved down and right
            cameraMoveDir = 7;
        }
        else
        {
            cameraMoveDir = -1; // just in case?
        }
    }
    public Ray GetLookDirRay()
    {
        return camera.ViewportPointToRay( new Vector3( 0.5f, 0.5f, 0 ) );
    }

    void CheckGrounded()
    {
        // Tell the animator if we're on the ground or not
        if( controller.isGrounded )
        {
            anim.SetBool( "isGrounded", true );
        }
        else
        {
            anim.SetBool( "isGrounded", false );
        }
    }
}
