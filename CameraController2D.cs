
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraController2D : MonoBehaviour
{
    public Transform followTarget;

    [SerializeField] float headHeight;
    [SerializeField] Vector2 lookAtOffset;
    [SerializeField] float zoomClosest, zoomFurthest;
    [Range(0, 1)]
    [SerializeField] float zoomCurrent;

    Vector3 headPos, lookatPos;
    Camera cam;

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            cam = GetComponent<Camera>();

            zoomCurrent = Mathf.Clamp(zoomCurrent += (Input.mouseScrollDelta.y * .1f), 0, 1);

            //position of head
            headPos = followTarget.transform.position + new Vector3(0, headHeight, 0);
            lookatPos = headPos + new Vector3(0, lookAtOffset.y, 0) + (transform.right * lookAtOffset.x);

            //Move to headposition and back
            transform.position = lookatPos + new Vector3(0, 0, -10);

            //Calculate cam orto sizes based on zoom
            cam.orthographicSize = Mathf.Lerp(zoomClosest, zoomFurthest, zoomCurrent);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (followTarget != null)
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lookatPos, .25f);

            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(headPos, .25f);
        }
    }
}
