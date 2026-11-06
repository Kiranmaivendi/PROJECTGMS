using UnityEngine;

public class ZombieFollower : MonoBehaviour
{
    [Header("Target")]
    public Transform player;              // The Player

    [Header("Distance Behind Player")]
    public float followDistance = 5f;     // Normal gap behind the player
    public float closeDistance = 2f;      // Gap when the zombie has nearly caught the player
    public float catchUpSpeed = 4f;       // How fast the zombie closes in
    public float recoverSpeed = 1f;       // How fast the gap returns to normal
    public float stumbleRecoverTime = 3f; // Seconds before the gap starts returning to normal

    [Header("Movement Feel")]
    public float laneFollowSpeed = 6f;    // How quickly the zombie follows the player sideways
    public float groundY = 1f;            // Height of the zombie's centre
    public float bobHeight = 0.2f;        // Small up-and-down running bounce
    public float bobSpeed = 10f;

    private float currentDistance;
    private float targetDistance;

    // True when the zombie is right behind the player
    public bool HasCaughtPlayer
    {
        get { return currentDistance <= closeDistance + 0.1f; }
    }

    void Start()
    {
        currentDistance = followDistance;
        targetDistance = followDistance;
    }

    void Update()
    {
        if (player == null) return;

        float speed = targetDistance < currentDistance ? catchUpSpeed : recoverSpeed;
        currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, speed * Time.deltaTime);

        float x = Mathf.Lerp(transform.position.x, player.position.x, laneFollowSpeed * Time.deltaTime);
        float y = groundY + Mathf.Abs(Mathf.Sin(Time.time * bobSpeed)) * bobHeight;
        float z = player.position.z - currentDistance;

        transform.position = new Vector3(x, y, z);
    }

    // Called later when the player hits an obstacle
    public void Stumble()
    {
        targetDistance = closeDistance;
        CancelInvoke(nameof(Recover));
        Invoke(nameof(Recover), stumbleRecoverTime);
    }

    void Recover()
    {
        targetDistance = followDistance;
    }
}