using UnityEngine;

public class FollowAndFaceCamera : MonoBehaviour
{
    [Header("camera")]
    public Transform playerCamera; 

    [Header("rotation")]
    public Vector3 positionOffset = new Vector3(0, 0.5f, 0); 

    [Header("round")]
    public bool useCameraForward = true;   
    public bool lockPitchRotation = true;  
    public float rotationSmoothness = 8f;  

    void LateUpdate()
    {
        if (playerCamera == null) return;

        UpdatePosition();
        UpdateRotation();
    }

    void UpdatePosition()
    {
        Vector3 targetPos = playerCamera.position 
            + playerCamera.right * positionOffset.x
            + playerCamera.up * positionOffset.y
            + playerCamera.forward * positionOffset.z;

        transform.position = targetPos;
    }

    void UpdateRotation()
    {
        if (useCameraForward)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                playerCamera.rotation,
                rotationSmoothness * Time.deltaTime
            );
        }
        else
        {
            Vector3 lookDirection = playerCamera.position - transform.position;
            if (lockPitchRotation) lookDirection.y = 0;

            Quaternion targetRot = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRot,
                rotationSmoothness * Time.deltaTime
            );
        }
    }
    
    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, playerCamera.position);
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
}