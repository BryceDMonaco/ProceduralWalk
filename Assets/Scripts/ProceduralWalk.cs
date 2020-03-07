using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalk : MonoBehaviour
{
    public Transform legTransform;  // Will always look at the foot point
    public Transform footTransform;  // The point where the foot is locked
    public Transform neutralPoint;  // The point where the foot will check its distance from (should just be straight down for a biped)

    public float distanceThreshold = 2f;  // The maximum absolute value distance between the foot point and the neutral point
    public float footForwardAmount = 0.9f;  // When the walker steps forward, the foot should travel footForwardAmount * distanceThreshold units, must be <1
    public float footBackwardAmount = 0.6f;  // When the walker steps backward, the foot should travel footBackwardAmount * distanceThreshold units, must be <1

    private bool footDown = false;
    private Vector3 footDownPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(neutralPoint.position, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100f, ~LayerMask.GetMask(new string[] { "Ignore Raycast" })))
        {

            neutralPoint.position = hit.point;
        }

        if (!footDown)
        {
            // If foot is not down, put it down (set foot point to the current neutral point)
            footDownPos = neutralPoint.position;
            footTransform.position = footDownPos;
            footDown = true;
        }
        else if (Vector3.Distance(footTransform.position, neutralPoint.position) > distanceThreshold)
        {
            // Take a step
            footDown = false;

            if ((neutralPoint.position - footTransform.position).z >= 0)  // Foot is behind (body moving forward)
            {
                Vector3 footForwardPosition = neutralPoint.position + (distanceThreshold *footForwardAmount * neutralPoint.forward);
                footDownPos = footForwardPosition;
                footTransform.position = footDownPos;
            }
            else  // Foot is ahead (body moving backwards)
            {
                Vector3 footBackwardPosition = neutralPoint.position - (distanceThreshold * footBackwardAmount * neutralPoint.forward);
                footDownPos = footBackwardPosition;
                footTransform.position = footDownPos;
            }

            footDown = true;

        }
        else  // Foot is down, and it is still within the distance threshold
        {
            // Set foot's position back to what it should be
            footTransform.position = footDownPos;
        }

        // Used in place of a model for the leg model
        Debug.DrawLine(legTransform.position, footTransform.position, Color.green);
    }
}
