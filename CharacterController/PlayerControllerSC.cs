using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerControllerSC : MonoBehaviour
{
    #region variables
    public Transform overrideCamera;

    //These are classes to make them collapsable in Inspector
    public MovementClass Movement;
    public ColissionClass Collision;

    #region Movement
    float horizontalInput, verticalInput;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public int jumpCountAir;
    #endregion

    #region Collision
    RaycastHit2D groundedHit;
    RaycastHit2D leftHit, rightHit, topHit;
    Rigidbody2D rb;

    Vector2 rightBot, rightTop, leftBot, leftTop, tll, bll, trr, brr, ttr, ttl, bbl, bbr;
    #endregion
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CalculateRayPositions();
        UpdateColStatus();
        HandleVariables();
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

        if (rb.linearVelocity.y < 1f)
        {
            if (Collision.isGrounded && jumpCountAir != 0) jumpCountAir = 0;
        }
    }

    void UpdateColStatus()
    {
        //Raycast down, this is simple to understand
        groundedHit = RaycastCheck(leftBot, bbr, rightBot, bbl, Collision.groundedMask);

        //Raycast in X shapes, left right and on top
        leftHit = RaycastCheck(leftBot, bll, rightBot, tll, Collision.colissionMask);
        rightHit = RaycastCheck(leftBot, brr, rightBot, trr, Collision.colissionMask);
        topHit = RaycastCheck(leftBot, ttr, rightBot, ttl, Collision.colissionMask);

        //set onhit based on collider
        Collision.onLeft = leftHit.collider != null;
        Collision.onRight = rightHit.collider != null;
        Collision.onTop = topHit.collider != null;
        Collision.isGrounded = groundedHit.collider != null;
    }

    void HandleJump()
    {
        if (Collision.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Movement.jumpPower);
            }
        }
        else
        {
            if (jumpCountAir >= Movement.airJumps)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpCountAir++;
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
                if (rb.linearVelocity.magnitude > 0.1f && Collision.isGrounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x * .8f, rb.linearVelocity.y);
                }
                break;
        }
    }

    #region Helpers
    RaycastHit2D RaycastCheck(Vector2 sA, Vector2 eA, Vector2 sB, Vector2 eB, LayerMask mask)
    {
        RaycastHit2D tempHit;
        float dist = Vector2.Distance(sA, eA);

        tempHit = Physics2D.Raycast(sA, (eA - sA).normalized, dist, mask);
        if (tempHit.collider == null)
            tempHit = Physics2D.Raycast(sB, (eB - sB).normalized, dist, mask);

        return tempHit;
    }

    void CalculateRayPositions()
    {
        // Base
        rightBot = transform.position + new Vector3(Collision.baseWidth, Collision.baseHeight);
        rightTop = transform.position + new Vector3(Collision.baseWidth, Collision.endHeight);
        leftBot = transform.position + new Vector3(-Collision.baseWidth, Collision.baseHeight);
        leftTop = transform.position + new Vector3(-Collision.baseWidth, Collision.endHeight);

        // Grounded
        bbl = transform.position + new Vector3(-Collision.baseWidth, -Collision.groundLength);
        bbr = transform.position + new Vector3(Collision.baseWidth, -Collision.groundLength);

        // Outer
        tll = leftTop + new Vector2(-Collision.outLength, 0);
        trr = rightTop + new Vector2(Collision.outLength, 0);
        bll = leftBot + new Vector2(-Collision.outLength, 0);
        brr = rightBot + new Vector2(Collision.outLength, 0);
        ttl = leftTop + new Vector2(0, Collision.outLength);
        ttr = rightTop + new Vector2(0, Collision.outLength);
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            CalculateRayPositions();
        }

        DrawColRayGizmos(.066f, .1f);
    }

    void DrawColRayGizmos(float outerWidth, float innerWidth)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftTop, innerWidth);
        Gizmos.DrawWireSphere(leftBot, innerWidth);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rightTop, innerWidth);
        Gizmos.DrawWireSphere(rightBot, innerWidth);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(tll, outerWidth);
        Gizmos.DrawWireSphere(trr, outerWidth);
        Gizmos.DrawWireSphere(bll, outerWidth);
        Gizmos.DrawWireSphere(brr, outerWidth);
        Gizmos.DrawWireSphere(ttl, outerWidth);
        Gizmos.DrawWireSphere(ttr, outerWidth);
        Gizmos.DrawWireSphere(bbl, outerWidth);
        Gizmos.DrawWireSphere(bbr, outerWidth);

        //Left
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftTop, bll);
        Gizmos.DrawLine(leftBot, tll);
        Gizmos.DrawLine(rightTop, brr);
        Gizmos.DrawLine(rightBot, trr);
        Gizmos.DrawLine(leftBot, ttr);
        Gizmos.DrawLine(rightBot, ttl);

        Gizmos.DrawRay(leftBot, (bbr - leftBot).normalized * Vector2.Distance(leftBot, bbr));
        Gizmos.DrawRay(rightBot, (bbl - rightBot).normalized * Vector2.Distance(rightBot, bbr));
    }
    #endregion

    #region Classes
    [System.Serializable]
    public class MovementClass
    {
        public float walkSpeed = 4, runSpeed = 8;
        public float jumpPower = 8;
        public int airJumps = 1;
    }

    [System.Serializable]
    public class ColissionClass
    {
        public float baseHeight = .2f, endHeight = 1f;
        public float baseWidth = .3f, outLength = .5f, groundLength = .2f;
        public LayerMask colissionMask;
        public LayerMask groundedMask;
        public bool onLeft, onRight, onTop, isGrounded;
    }
    #endregion
}
