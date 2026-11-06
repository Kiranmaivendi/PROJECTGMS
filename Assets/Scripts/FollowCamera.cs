using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                         // The Player

    [Header("Camera Position")]
    public Vector3 offset = new Vector3(0f, 4f, -8f); // Behind and above the player
    public float followSpeed = 8f;                    // How smoothly the camera follows
    public float lookAheadDistance = 6f;              // How far ahead of the player the camera looks

    void LateUpdate()
    {
        if (target == null) return;

        // Follow forward and up/down smoothly, but sideways only a little
        Vector3 desired = new Vector3(
            target.position.x * 0.5f + offset.x,
            offset.y,
            target.position.z + offset.z
        );

        transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);

        Vector3 lookPoint = new Vector3(target.position.x * 0.5f, 1f, target.position.z + lookAheadDistance);
        transform.LookAt(lookPoint);
    }
}