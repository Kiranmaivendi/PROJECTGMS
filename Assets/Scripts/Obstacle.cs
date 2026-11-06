using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        RunnerPlayer player = other.GetComponent<RunnerPlayer>();
        if (player != null && player.isAlive)
        {
            ZombieFollower zombie = Object.FindAnyObjectByType<ZombieFollower>();
            if (zombie != null)
            {
                zombie.Stumble();
            }
            player.Die();
        }
    }
}