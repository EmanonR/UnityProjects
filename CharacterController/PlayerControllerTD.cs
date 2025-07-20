using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerControllerTD : MonoBehaviour
{
    #region variables
    public MovementType movementType;
    //These are classes to make them collapsable in Inspector
    public MovementClass Movement;
    public PlayerCollisionInfo collision;

    public enum MovementType
    {
        normal,
        isometric
    }

    [HideInInspector] public float currentSpeed;
    [HideInInspector] public int jumpCountAir;

    float horizontalInput, verticalInput;

    Vector3[] collisionPoints = new Vector3[12];
    Vector2 moveVec;

    SpriteRenderer spriteRen;
    #endregion

    private void Awake()
    {
        if (spriteRen == null)
            spriteRen = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        HandleVariables();
        
        if (GameManager.playerFrozen) return;
    }

    private void FixedUpdate()
    {
        switch (movementType)
        {
            case MovementType.normal:
                SetCollisionPoints(transform.position, 1);
                break;

            case MovementType.isometric:
                SetCollisionPoints(transform.position, 2);
                break;
        }

        UpdateColStatus();

        if (GameManager.playerFrozen) return;


        HandleMovement();
    }

    void HandleVariables()
    {
        //Directional input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveVec = new Vector2(horizontalInput, verticalInput).normalized;

        //Set current speed
        if (moveVec.magnitude != 0)
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? Movement.runSpeed : Movement.walkSpeed;
        else
            currentSpeed = 0;
    }

    void UpdateColStatus()
    {
        //onTop
        collision.hitTop = ReturnColInfo(0);

        if (collision.hitTop.Length != 0)
        {
            collision.onTop = true;
            AdjustForCollision(collision.hitTop[0], collision.hitTop[1]);
        }
        else
            collision.onTop = false;

        //onRight
        collision.hitRight = ReturnColInfo(1);

        if (collision.hitRight.Length != 0)
        {
            collision.onRight = true;
            AdjustForCollision(collision.hitRight[0], collision.hitRight[1]);
        }
        else
            collision.onRight = false;

        //onBottom
        collision.hitBottom = ReturnColInfo(2);

        if (collision.hitBottom.Length != 0)
        {
            collision.onBottom = true;
            AdjustForCollision(collision.hitBottom[0], collision.hitBottom[1]);
        }
        else
            collision.onBottom = false;

        //onLeft
        collision.hitLeft = ReturnColInfo(3);

        if (collision.hitLeft.Length != 0)
        {
            collision.onLeft = true;
            AdjustForCollision(collision.hitLeft[0], collision.hitLeft[1]);
        }
        else
            collision.onLeft = false;
    }

    Vector2[] ReturnColInfo(int pointID)
    {
        // Return startPos and endPos
        Vector3 offsetX = new(collision.outSize * .9f, 0);
        Vector3 offsetY = new(0, collision.outSize * .9f);

        RaycastHit2D value;

        value = GetRayHit2D(transform.position, collisionPoints[pointID]);
        if (value != false)
        {
            return new Vector2[]{
                transform.position,
                value.point
                };
        }

        if (pointID % 2 == 0)
        {
            value = GetRayHit2D(transform.position - offsetX, collisionPoints[pointID] - offsetX);
            if (value != false)
            {
                return new Vector2[]{
                transform.position - offsetX,
                value.point
                };
            }

            value = GetRayHit2D(transform.position + offsetX, collisionPoints[pointID] + offsetX);
            if (value != false)
            {
                return new Vector2[]{
                transform.position + offsetX,
                value.point
                };
            }
        }
        else
        {
            value = GetRayHit2D(transform.position - offsetY, collisionPoints[pointID] - offsetY);
            if (value != false)
            {
                return new Vector2[]{
                transform.position - offsetY,
                value.point
                };
            }

            value = GetRayHit2D(transform.position + offsetY, collisionPoints[pointID] + offsetY);
            if (value != false)
            {
                return new Vector2[]{
                transform.position + offsetY,
                value.point
                };
            }
        }
        return new Vector2[0];
    }

    void AdjustForCollision(Vector2 origin, Vector2 hitPoint)
    {
        float distance = Vector2.Distance(origin, hitPoint);
        float distanceToMove = collision.outSize - distance;

        transform.position += (Vector3)(origin - hitPoint) * distanceToMove;
    }

    void HandleMovement()
    {
        switch (movementType)
        {
            case MovementType.normal:
                if (collision.onTop) moveVec.y = Mathf.Clamp(moveVec.y, -1, 0);
                if (collision.onRight) moveVec.x = Mathf.Clamp(moveVec.x, -1, 0);
                if (collision.onBottom) moveVec.y = Mathf.Clamp(moveVec.y, 0, 1);
                if (collision.onLeft) moveVec.x = Mathf.Clamp(moveVec.x, 0, 1);

                if (moveVec.x > 0) spriteRen.flipX = true;
                if (moveVec.x < 0) spriteRen.flipX = false;

                transform.Translate(currentSpeed * Time.deltaTime * moveVec.normalized);
                break;
            case MovementType.isometric:
                Vector2 isoMoveDir = new(horizontalInput + verticalInput, (-horizontalInput + verticalInput) / 2);

                transform.Translate(currentSpeed * Time.deltaTime * isoMoveDir.normalized);
                break;
        }
    }



    public void SetCollisionPoints(Vector3 playerPos, int type)
    {
        switch (type)
        {
            // Top Down
            case 1:
                collisionPoints = new Vector3[] { 
                // Top
                playerPos + new Vector3(0, collision.outSize),
                // Right
                playerPos + new Vector3(collision.outSize, 0),
                // Down
                playerPos + new Vector3(0, -collision.outSize),
                // Left
                playerPos + new Vector3(-collision.outSize, 0)
                };
                break;

            // Isomtric
            case 2:
                collisionPoints = new Vector3[] { 
                // Top
                playerPos + new Vector3(collision.outSize, collision.outSize / 2),
                // Right
                playerPos + new Vector3(collision.outSize, -collision.outSize / 2),
                // Down
                playerPos + new Vector3(-collision.outSize, -collision.outSize / 2),
                // Left
                playerPos + new Vector3(-collision.outSize, collision.outSize / 2)
                };
                break;
        }
    }

    public RaycastHit2D GetRayHit2D(Vector2 startPos, Vector2 endPos)
    {
        float length = Vector2.Distance(startPos, endPos);

        return Physics2D.Raycast(startPos, endPos - startPos, length, collision.mask);
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
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        DrawColGizmos();
    }

    void DrawColGizmos()
    {
        float sphereSize = .1f;

        switch (movementType)
        {
            case MovementType.normal:
                SetCollisionPoints(transform.position, 1);
                Vector3 offsetX = new(collision.outSize, 0);
                Vector3 offsetY = new(0, collision.outSize); 
                
                Vector3 startPos = transform.position;
                Vector3 endPos = transform.position;

                for (int i = 0; i < 12; i++)
                {
                    switch (i)
                    {
                        case > 8:
                            // Left
                            if (i % 3 == 0)
                            {
                                startPos = transform.position - offsetY * collision.rayBufferOffset;
                                endPos = transform.position - offsetY * collision.rayBufferOffset - offsetX;
                            }
                            else
                            {
                                startPos += offsetY * collision.rayBufferOffset;
                                endPos += offsetY * collision.rayBufferOffset;
                            }
                            break;
                        case > 5:
                            // Bot
                            if (i % 3 == 0)
                            {
                                startPos = transform.position + offsetX * collision.rayBufferOffset;
                                endPos = transform.position - offsetY + offsetX * collision.rayBufferOffset;
                            }
                            else
                            {
                                startPos -= offsetX * collision.rayBufferOffset;
                                endPos -= offsetX * collision.rayBufferOffset;
                            }
                            break;
                        case > 2:
                            // Right
                            if (i % 3 == 0)
                            {
                                startPos = transform.position + offsetY * collision.rayBufferOffset;
                                endPos = transform.position + (offsetY * collision.rayBufferOffset) + offsetX;
                            }
                            else
                            {
                                startPos -= offsetY * collision.rayBufferOffset;
                                endPos -= offsetY * collision.rayBufferOffset;
                            }
                            break;
                        default:
                            // Top
                            if (i % 3 == 0)
                            {
                                startPos = transform.position - offsetX * collision.rayBufferOffset;
                                endPos = transform.position + offsetY - offsetX * collision.rayBufferOffset;
                            }
                            else
                            {
                                startPos += offsetX * collision.rayBufferOffset;
                                endPos += offsetX * collision.rayBufferOffset;
                            }
                            break;
                    }

                    // Tips
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(endPos, sphereSize / 2);
                    // Rays
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(startPos, endPos);
                }

                // Corners/Bounds
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(transform.position + new Vector3(collision.outSize, collision.outSize), sphereSize);
                Gizmos.DrawWireSphere(transform.position + new Vector3(-collision.outSize, collision.outSize), sphereSize);
                Gizmos.DrawWireSphere(transform.position + new Vector3(-collision.outSize, -collision.outSize), sphereSize);
                Gizmos.DrawWireSphere(transform.position + new Vector3(collision.outSize, -collision.outSize), sphereSize);

                break;

            case MovementType.isometric:
                SetCollisionPoints(transform.position, 2);

                for (int i = 0; i < collisionPoints.Length; i++)
                {
                    // Main
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireSphere(collisionPoints[i], sphereSize);
                    // Rays
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, collisionPoints[i]);
                }
                break;
        }
    }
    #endregion

    #region Classes
    [System.Serializable]
    public class MovementClass
    {
        public float walkSpeed = 4, runSpeed = 8;
    }

    [System.Serializable]
    public class PlayerCollisionInfo
    {
        // Side
        public float outSize;
        [Range(0, 1)]
        public float rayBufferOffset = .9f;

        public LayerMask mask;

        [Header("Collision")]
        public bool onRight;
        public bool onLeft, onTop, onBottom;

        [HideInInspector] public Vector2[] hitTop, hitRight, hitBottom, hitLeft;
    }
    #endregion
}
