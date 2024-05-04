using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerControllerSC : MonoBehaviour
{
    //These are classes to make them collapsable in Inspector
    public MovementClass Movement;
    public GroundedClass Grounded;

    public Transform overrideCamera;

    float horizontalInput, verticalInput;

    RaycastHit2D groundedHit;
    Rigidbody2D rb;

    [HideInInspector] public float currentSpeed;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    void HandleSettings()
    {
        //Directional input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Set current speed
        if (horizontalInput != 0)
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? Movement.runSpeed : Movement.walkSpeed;
        else
            currentSpeed = 0;
    }

    void UpdateGroundedStatus()
    {
        Vector2 tl = transform.position + new Vector3(-Grounded.groundRayWidth, Grounded.groundRayTop);
        Vector2 tr = transform.position + new Vector3(Grounded.groundRayWidth, Grounded.groundRayTop);
        Vector2 bl = transform.position + new Vector3(-Grounded.groundRayWidth, Grounded.groundRayBot);
        Vector2 br = transform.position + new Vector3(Grounded.groundRayWidth, Grounded.groundRayBot);

        //Spherecast down, this is simple to understand
        groundedHit = Physics2D.Raycast(tl, (br - tl).normalized, Vector2.Distance(tl, br), Grounded.groundedMask);
        if (groundedHit.collider == null)
            groundedHit = Physics2D.Raycast(tr, (bl - tr).normalized, Vector2.Distance(tr, bl), Grounded.groundedMask);

        //set grounded based on if collider
        Grounded.isGrounded = groundedHit.collider != null;
    }


    void HandleJump()
    {
        if (Grounded.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(rb.velocity.x, Movement.jumpPower);
            }
        }
    }

    void HandleMovement()
    {
        if (horizontalInput != 0f)
        {
            rb.velocity = new Vector2(horizontalInput * currentSpeed,
                                        rb.velocity.y);
        }
        else if (rb.velocity.magnitude > 0.1f && Grounded.isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x * .8f, rb.velocity.y);
        }
    }


    private void OnDrawGizmosSelected()
    {
        DrawGroundRayGizmos();
    }

    void DrawGroundRayGizmos()
    {
        Vector2 tl = transform.position + new Vector3(-Grounded.groundRayWidth, Grounded.groundRayTop);
        Vector2 tr = transform.position + new Vector3(Grounded.groundRayWidth, Grounded.groundRayTop);
        Vector2 bl = transform.position + new Vector3(-Grounded.groundRayWidth, Grounded.groundRayBot);
        Vector2 br = transform.position + new Vector3(Grounded.groundRayWidth, Grounded.groundRayBot);


        Gizmos.DrawWireSphere(tl, .1f);
        Gizmos.DrawWireSphere(tr, .1f);

        Gizmos.DrawWireSphere(bl, .1f);
        Gizmos.DrawWireSphere(br, .1f);


        Gizmos.color = Color.red;
        Gizmos.DrawRay(tl, (br - tl).normalized * Vector2.Distance(tl, br));
        Gizmos.DrawRay(tr, (bl - tr).normalized * Vector2.Distance(tl, br));
    }


    [System.Serializable]
    public class MovementClass
    {
        public float walkSpeed = 4, runSpeed = 8;
        public float jumpPower = 8;
    }

    [System.Serializable]
    public class GroundedClass
    {
        public float groundRayTop = .2f, groundRayBot = -.2f;
        public float groundRayWidth = .3f;
        public LayerMask groundedMask;
        public bool isGrounded;
    }
}
