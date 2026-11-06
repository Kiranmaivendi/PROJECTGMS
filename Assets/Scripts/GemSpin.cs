using UnityEngine;

public class GemSpin : MonoBehaviour
{
    public float spinSpeed = 120f;

    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }
}