using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    //These are classes to make them collapsable in Inspector
    public MovementClass Movement;
    public GroundedClass Grounded;

    public Transform overrideCamera;

    bool isGrounded;
    float horizontalInput, verticalInput;
    float targetAngle;
    float turnSmoothVelocity;
    RaycastHit groundedHit;
    Rigidbody rb;
    Vector3 movementDir;

    [HideInInspector] public float currentSpeed;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleSettings();
        UpdateGroundedStatus();
        HandleJump();   
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleSettings()
    {
        //Directional input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        movementDir = new Vector3(horizontalInput, 0, verticalInput).normalized;

        //Set current speed
        if (movementDir.magnitude > .1f)
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? Movement.runSpeed : Movement.walkSpeed;
        else
            currentSpeed = 0;
    }

    void UpdateGroundedStatus()
    {
        //create the ray
        Ray ray = new (rb.transform.position + new Vector3(0, Grounded.groundRayHeight, 0), Vector3.down);
        //Spherecast down, this is simple to understand
        Physics.SphereCast(ray, Grounded.groundRayWidth, out groundedHit, Grounded.groundRayLength, Grounded.groundedMask);

        //set grounded based on if collider
        isGrounded = groundedHit.collider != null;
    }


    void HandleJump()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector3(rb.velocity.x, Movement.jumpPower);
            }
        }
    }

    void HandleMovement()
    {
        if (movementDir.magnitude > .1f)
        {
            if (overrideCamera == null)
                targetAngle = Mathf.Atan2(movementDir.x, movementDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            else
                targetAngle = Mathf.Atan2(movementDir.x, movementDir.z) * Mathf.Rad2Deg + overrideCamera.transform.eulerAngles.y;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir = moveDir.normalized;
            rb.velocity = new Vector3(moveDir.x * currentSpeed,
                                        rb.velocity.y,
                                            moveDir.z * currentSpeed);
        }
        else if (rb.velocity.magnitude > 0.1f && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x * .8f, rb.velocity.y, rb.velocity.z * .8f);
        }
    }

    void HandleRotation()
    {
        float rotationAngle = Mathf.SmoothDampAngle(rb.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, Movement.turnSmoothTime);
        rb.transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }


    private void OnDrawGizmosSelected()
    {
        DrawGroundRayGizmos();
    }

    void DrawGroundRayGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, Grounded.groundRayHeight), Grounded.groundRayWidth);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, Grounded.groundRayHeight - Grounded.groundRayLength), Grounded.groundRayWidth);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, Grounded.groundRayHeight), .02f);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, Grounded.groundRayHeight - Grounded.groundRayLength), .02f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0, Grounded.groundRayHeight), transform.position + new Vector3(0, Grounded.groundRayHeight - Grounded.groundRayLength));
    }


    [System.Serializable]
    public class MovementClass
    {
        public float walkSpeed = 5, runSpeed = 10;
        public float jumpPower = 7;
        public float turnSmoothTime = .1f;
    }

    [System.Serializable]
    public class GroundedClass
    {
        public float groundRayLength = .2f;
        public float groundRayWidth = .1f, groundRayHeight = .1f;
        public LayerMask groundedMask;
    }
}
