using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin(coinValue);
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCoin();
        }
        Destroy(gameObject);
    }
}