using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    public Transform followTarget, lookAtTarget;

    [SerializeField] float headHeight;
    [SerializeField] Vector2 lookAtOffset;
    [Range(0, 1)]
    [SerializeField] float zoomClosest, zoomCurrent;
    [SerializeField] float zoomFurthest;

    [Header("Collision")]
    [SerializeField] float rayWidth = .5f;
    [SerializeField] LayerMask colMask;

    
    Vector3 headPos, lookatPos, nearPos, farPos, zoomPos;
    float mouseX;
    float mouseY;

    private void Awake()
    {
        if (followTarget || lookAtTarget != null)
        {
            #region setup
            if (followTarget == null) followTarget = lookAtTarget;
            if (lookAtTarget == null) lookAtTarget = followTarget;
            #endregion
        }
    }

    private void LateUpdate()
    {
        if (followTarget || lookAtTarget != null)
        {
            zoomCurrent = Mathf.Clamp(zoomCurrent += (Input.mouseScrollDelta.y * .1f), 0, 1);

            //position of head
            headPos = followTarget.transform.position + new Vector3(0, headHeight, 0);
            lookatPos = headPos + new Vector3(0, lookAtOffset.y, 0) + (transform.right * lookAtOffset.x);

            //Move to headposition
            transform.position = lookatPos;

            //Rotate

            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            transform.eulerAngles += new Vector3(0, mouseX, 0);
            transform.eulerAngles -= new Vector3(mouseY, 0, 0);

            //Calculate positions based on cameras back angle
            //farPos = headPos + new Vector3(0, 0, zoomFurthest);
            farPos = lookatPos + (-transform.forward * zoomFurthest);
            nearPos = Vector3.Lerp(farPos, lookatPos, zoomClosest);
            zoomPos = Vector3.Lerp(farPos, nearPos, zoomCurrent);

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, rayWidth, -transform.forward, out hit, Vector3.Distance(transform.position, zoomPos), colMask))
            {
                //Move to hit Positon
                transform.position = Vector3.Lerp(transform.position, hit.point + hit.normal * rayWidth, .9f);
            }
            else
            {
                //Move to zoomPosition
                transform.position = Vector3.Lerp(transform.position, zoomPos, .9f);
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        if (followTarget || lookAtTarget != null)
        {
            #region setup
            if (followTarget == null) followTarget = lookAtTarget;
            if (lookAtTarget == null) lookAtTarget = followTarget;
            #endregion

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(nearPos, .5f);
            Gizmos.DrawWireSphere(farPos, .5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(zoomPos, rayWidth);
            Gizmos.DrawWireSphere(lookatPos, .25f);

            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(headPos, .25f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(farPos, lookatPos);
        }
    }
}
