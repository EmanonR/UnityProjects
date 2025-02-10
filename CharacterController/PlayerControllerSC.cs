using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    public Transform overrideCamera;

    //These are classes to make them collapsable in Inspector
    public MovementClass Movement;
    public GroundedClass Grounded;
    public ColissionClass Collision;


    float horizontalInput, verticalInput;

    RaycastHit2D groundedHit;
    RaycastHit2D leftHit, rightHit, topHit;
    Rigidbody2D rb;

    [HideInInspector] public float currentSpeed;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleVariables();
        UpdateGroundedStatus();
        UpdateColStatus();
        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleVariables()
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

        //Raycast down, this is simple to understand
        groundedHit = RaycastCheck(tl, br, tr, bl, Grounded.groundedMask);

        //set grounded based on if collider
        Grounded.isGrounded = groundedHit.collider != null;
    }

    void UpdateColStatus()
    {
        Vector2 tl = transform.position + new Vector3(-Collision.baseWidth, Collision.endHeight);
        Vector2 tr = transform.position + new Vector3(Collision.baseWidth, Collision.endHeight);
        Vector2 bl = transform.position + new Vector3(-Collision.baseWidth, Collision.baseHeight);
        Vector2 br = transform.position + new Vector3(Collision.baseWidth, Collision.baseHeight);

        Vector2 tll = tl + new Vector2(-Collision.outLength, 0);
        Vector2 trr = tr + new Vector2(Collision.outLength, 0);
        Vector2 bll = bl + new Vector2(-Collision.outLength, 0);
        Vector2 brr = br + new Vector2(Collision.outLength, 0);
        Vector2 ttl = tl + new Vector2(0, Collision.outLength);
        Vector2 ttr = tr + new Vector2(0, Collision.outLength);

        //Raycast in X shapes, left right and on top
        leftHit = RaycastCheck(tl, bll, bl, tll, Collision.colissionMask);
        rightHit = RaycastCheck(tr, brr, br, trr, Collision.colissionMask);
        topHit = RaycastCheck(tl, ttr, tr, ttl, Collision.colissionMask);

        //set grounded based on if collider
        Collision.onLeft = leftHit.collider != null;
        Collision.onRight = rightHit.collider != null;
        Collision.onTop = topHit.collider != null;
    }

    RaycastHit2D RaycastCheck(Vector2 sA, Vector2 eA, Vector2 sB, Vector2 eB, LayerMask mask)
    {
        RaycastHit2D tempHit;
        float dist = Vector2.Distance(sA, eA);

        tempHit = Physics2D.Raycast(sA, (eA - sA).normalized, dist, mask);
        if (tempHit.collider == null)
            tempHit = Physics2D.Raycast(sB, (eB - sB).normalized, dist, mask);

        return tempHit;
    }

    void HandleJump()
    {
        if (Grounded.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Movement.jumpPower);
            }
        }
    }

    void HandleMovement()
    {
        switch (horizontalInput)
        {
            case > 0:   //Right
                if (Collision.onRight) return;
                rb.linearVelocity = new Vector2(horizontalInput * currentSpeed,
                                        rb.linearVelocity.y);
                break;

            case < 0:   //Left
                if (Collision.onLeft) return;
                rb.linearVelocity = new Vector2(horizontalInput * currentSpeed,
                                        rb.linearVelocity.y);
                break;

            default:    //Default
                if (rb.linearVelocity.magnitude > 0.1f && Grounded.isGrounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x * .8f, rb.linearVelocity.y);
                }
                break;
        }
    }


    private void OnDrawGizmosSelected()
    {
        float outPosWidth = .066f;
        DrawGroundRayGizmos(outPosWidth);
        DrawColRayGizmos(outPosWidth);
    }

    void DrawGroundRayGizmos(float outPosWidth)
    {
        Vector2 tl = transform.position + new Vector3(-Grounded.groundRayWidth, Grounded.groundRayTop);
        Vector2 tr = transform.position + new Vector3(Grounded.groundRayWidth, Grounded.groundRayTop);
        Vector2 bl = transform.position + new Vector3(-Grounded.groundRayWidth, Grounded.groundRayBot);
        Vector2 br = transform.position + new Vector3(Grounded.groundRayWidth, Grounded.groundRayBot);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(tl, .1f);
        Gizmos.DrawWireSphere(tr, .1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(bl, outPosWidth);
        Gizmos.DrawWireSphere(br, outPosWidth);


        Gizmos.color = Color.blue;
        Gizmos.DrawRay(tl, (br - tl).normalized * Vector2.Distance(tl, br));
        Gizmos.DrawRay(tr, (bl - tr).normalized * Vector2.Distance(tl, br));
    }

    void DrawColRayGizmos(float outPosWidth)
    {
        Vector2 tl = transform.position + new Vector3(-Collision.baseWidth, Collision.endHeight);
        Vector2 tr = transform.position + new Vector3(Collision.baseWidth, Collision.endHeight);
        Vector2 bl = transform.position + new Vector3(-Collision.baseWidth, Collision.baseHeight);
        Vector2 br = transform.position + new Vector3(Collision.baseWidth, Collision.baseHeight);

        Vector2 tll = tl + new Vector2(-Collision.outLength, 0);
        Vector2 trr = tr + new Vector2(Collision.outLength, 0);
        Vector2 bll = bl + new Vector2(-Collision.outLength, 0);
        Vector2 brr = br + new Vector2(Collision.outLength, 0);
        Vector2 ttl = tl + new Vector2(0, Collision.outLength);
        Vector2 ttr = tr + new Vector2(0, Collision.outLength);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(tl, .1f);
        Gizmos.DrawWireSphere(tr, .1f);
        Gizmos.DrawWireSphere(bl, .1f);
        Gizmos.DrawWireSphere(br, .1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(tll, outPosWidth);
        Gizmos.DrawWireSphere(trr, outPosWidth);
        Gizmos.DrawWireSphere(bll, outPosWidth);
        Gizmos.DrawWireSphere(brr, outPosWidth);
        Gizmos.DrawWireSphere(ttl, outPosWidth);
        Gizmos.DrawWireSphere(ttr, outPosWidth);

        //Left
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(tl, bll);
        Gizmos.DrawLine(bl, tll);
        Gizmos.DrawLine(tr, brr);
        Gizmos.DrawLine(br, trr);
        Gizmos.DrawLine(bl, ttr);
        Gizmos.DrawLine(br, ttl);
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

    [System.Serializable]
    public class ColissionClass
    {
        public float baseHeight = .2f, endHeight = 1f;
        public float baseWidth = .3f, outLength = .5f;
        public LayerMask colissionMask;
        public bool onLeft, onRight, onTop;
    }
}
