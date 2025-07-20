using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraController2D : MonoBehaviour
{
    public Transform followTarget;
    public bool noZoom = false;

    [SerializeField] float headHeight;
    [SerializeField] Vector2 lookAtOffset;
    [SerializeField] float zoomClosest, zoomFurthest;
    [Range(0, 1)]
    [SerializeField] float zoomCurrent;

    [SerializeField] public Vector4[] cameraBounds;

    [SerializeField] bool insideBounds;
    [SerializeField] Vector4 currentBounds;

    Vector3 headPos, lookatPos;
    Camera cam;

    private void LateUpdate()
    {
        if (followTarget == null) return;

        #region camera tracking
        cam = GetComponent<Camera>();

        //position of head
        headPos = followTarget.transform.position + new Vector3(0, headHeight, 0);
        lookatPos = headPos + new Vector3(0, lookAtOffset.y, 0) + (transform.right * lookAtOffset.x);

        //Move to headposition and back
        transform.position = lookatPos + new Vector3(0, 0, -10);

        if (noZoom) return;

        zoomCurrent = Mathf.Clamp(zoomCurrent -= (Input.mouseScrollDelta.y * .1f), 0, 1);

        //Calculate cam orto sizes based on zoom
        cam.orthographicSize = Mathf.Lerp(zoomClosest, zoomFurthest, zoomCurrent);
        #endregion

        #region bounds clipping
        if (cameraBounds.Length == 0) return; // If no bounds, return

        insideBounds = false; 
        currentBounds = Vector4.zero;


        //Check for bounds
        for (int i = 0; i < cameraBounds.Length; i++)
        {
            Vector3 boundCenter = GetBoundCenter(cameraBounds[i]);
            Vector3 boundSize = GetBoundSize(cameraBounds[i]);

            float sizeXH = boundSize.x / 2;
            float sizeYH = boundSize.y / 2;


            float valueX = Mathf.Clamp(transform.position.x,
                                        boundCenter.x - sizeXH,
                                            boundCenter.x + sizeXH);

            float valueY = Mathf.Clamp(transform.position.y,
                                        boundCenter.y - sizeYH,
                                            boundCenter.y + sizeYH);


            if (valueX == transform.position.x && valueY == transform.position.y)
            {
                insideBounds = true;
                currentBounds = cameraBounds[i];
                break;
            }
        }

        if (insideBounds)
        {
            //Clip if needed
            Vector3 boundCenter = GetBoundCenter(currentBounds);
            Vector3 boundSize = GetBoundSize(currentBounds);

            float camH = Mathf.Lerp(zoomClosest, zoomFurthest, zoomCurrent) * (Camera.main.aspect);
            float camV = Mathf.Lerp(zoomClosest, zoomFurthest, zoomCurrent);

            float sizeXH = boundSize.x / 2;
            float sizeYH = boundSize.y / 2;

            // Check each edges and compare distances
            // trdl
            Vector4 borderE = new(boundCenter.y + sizeYH,
                                    boundCenter.x + sizeXH,
                                        boundCenter.y - sizeYH,
                                            boundCenter.x - sizeXH);

            float topOff = transform.position.y + camV - borderE.x;
            float rightOff = transform.position.x + camH - borderE.y;
            float downOff = transform.position.y - camV - borderE.z;
            float leftOff = transform.position.x - camH - borderE.w;

            if (topOff > 0) transform.position -= new Vector3(0, topOff);
            if (rightOff > 0) transform.position -= new Vector3(rightOff, 0);
            if (downOff < 0) transform.position -= new Vector3(0, downOff);
            if (leftOff < 0) transform.position -= new Vector3(leftOff, 0);

        }

        #endregion

    }

    private void OnDrawGizmosSelected()
    {
        if (followTarget != null)
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lookatPos, .15f);

            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(headPos, .25f);
        }

        if (cameraBounds.Length == 0) return;

        Gizmos.color = Color.red;
        float camH = Mathf.Lerp(zoomClosest, zoomFurthest, zoomCurrent) * (Camera.main.aspect);
        float camV = Mathf.Lerp(zoomClosest, zoomFurthest, zoomCurrent);

        Gizmos.DrawRay(new(transform.position.x, transform.position.y, 0), Vector3.left * camH);
        Gizmos.DrawRay(new(transform.position.x, transform.position.y, 0), Vector3.down * camV);
        for (int i = 0; i < cameraBounds.Length; i++)
        {
            Gizmos.color = cameraBounds[i] == currentBounds ? Color.yellow : Color.white;

            Vector3 boundCenter = GetBoundCenter(cameraBounds[i]);
            Vector3 boundSize = GetBoundSize(cameraBounds[i]);

            // Draw bounds
            Gizmos.DrawWireCube(boundCenter, boundSize);
        }
    }

    public Vector3 GetBoundCenter( Vector4 bound)
    {
        return new((bound.y + bound.w) / 2, (bound.x + bound.z) / 2, 0);
    }

    public Vector3 GetBoundSize(Vector4 bound)
    {
        return new((bound.y - bound.w), (bound.x - bound.z), 0);
    }
}
