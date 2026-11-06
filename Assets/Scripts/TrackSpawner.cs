using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    [Header("Track Settings")]
    public GameObject trackSegmentPrefab;
    public int startingSegments = 6;
    public float segmentLength = 30f;

    [Header("Player")]
    public Transform player;

    [Header("Cleanup")]
    public float deleteDistanceBehind = 40f;

    [Header("Obstacles")]
    public GameObject obstaclePrefab;
    public int obstaclesPerSegment = 2;
    public int safeSegmentsAtStart = 2;
    public float obstacleY = 0.6f;
    public float laneDistance = 1.6f;

    [Header("Overhead Obstacles")]
    public GameObject overheadPrefab;
    public float overheadChance = 0.25f;
    public float overheadY = 1.5f;

    [Header("Wrecked Cars")]
    public GameObject carPrefab;
    public float carChance = 0.25f;
    public float carY = 0.6f;

    [Header("Coins")]
    public GameObject coinPrefab;
    public int coinsPerLine = 5;
    public float coinSpacing = 1.5f;
    public float coinLineStartOffset = 12f;
    public float coinY = 1.2f;

    [Header("Decorations")]
    public GameObject sideDecorPrefab;

    private float nextSpawnZ = 0f;
    private int segmentsSpawned = 0;
    private List<GameObject> activeSegments = new List<GameObject>();
    private List<GameObject> activeDecor = new List<GameObject>();
    private List<GameObject> activeObstacles = new List<GameObject>();
    private List<GameObject> activeCoins = new List<GameObject>();

    void Start()
    {
        if (trackSegmentPrefab == null)
        {
            Debug.LogError("TrackSpawner: Track Segment Prefab is not assigned!");
            return;
        }

        for (int i = 0; i < startingSegments; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (trackSegmentPrefab == null || player == null) return;

        while (player.position.z > nextSpawnZ - (startingSegments - 2) * segmentLength)
        {
            SpawnSegment();
        }

        if (activeSegments.Count > 0)
        {
            GameObject oldest = activeSegments[0];
            if (oldest == null || player.position.z - oldest.transform.position.z > segmentLength + deleteDistanceBehind)
            {
                activeSegments.RemoveAt(0);
                if (oldest != null) Destroy(oldest);
            }
        }

        if (activeDecor.Count > 0)
        {
            GameObject oldestDecor = activeDecor[0];
            if (oldestDecor == null || player.position.z - oldestDecor.transform.position.z > segmentLength + deleteDistanceBehind)
            {
                activeDecor.RemoveAt(0);
                if (oldestDecor != null) Destroy(oldestDecor);
            }
        }

        CleanupList(activeObstacles);
        CleanupList(activeCoins);
    }

    void CleanupList(List<GameObject> list)
    {
        while (list.Count > 0)
        {
            GameObject first = list[0];
            if (first == null)
            {
                list.RemoveAt(0);
            }
            else if (player.position.z - first.transform.position.z > 20f)
            {
                list.RemoveAt(0);
                Destroy(first);
            }
            else
            {
                break;
            }
        }
    }

    void SpawnSegment()
    {
        float segmentStartZ = nextSpawnZ;
        Vector3 spawnPosition = new Vector3(0f, 0f, segmentStartZ + segmentLength / 2f);

        GameObject segment = Instantiate(trackSegmentPrefab, spawnPosition, Quaternion.identity, transform);
        activeSegments.Add(segment);

        if (sideDecorPrefab != null)
        {
            GameObject decor = Instantiate(sideDecorPrefab, spawnPosition, Quaternion.identity, transform);
            activeDecor.Add(decor);
        }

        if (segmentsSpawned >= safeSegmentsAtStart)
        {
            SpawnObstacles(segmentStartZ);
        }

        if (segmentsSpawned >= 1)
        {
            SpawnCoinLine(segmentStartZ);
        }

        segmentsSpawned++;
        nextSpawnZ += segmentLength;
    }

    void SpawnObstacles(float segmentStartZ)
    {
        if (obstaclePrefab == null || obstaclesPerSegment <= 0) return;

        float spacing = segmentLength / (obstaclesPerSegment + 1);

        for (int i = 1; i <= obstaclesPerSegment; i++)
        {
            int lane = Random.Range(0, 3);
            float x = (lane - 1) * laneDistance;
            float z = segmentStartZ + spacing * i;

            GameObject prefabToUse = obstaclePrefab;
            float y = obstacleY;
            float roll = Random.value;

            if (overheadPrefab != null && roll < overheadChance)
            {
                prefabToUse = overheadPrefab;
                y = overheadY;
            }
            else if (carPrefab != null && roll < overheadChance + carChance)
            {
                prefabToUse = carPrefab;
                y = carY;
            }

            GameObject obstacle = Instantiate(prefabToUse, new Vector3(x, y, z), Quaternion.identity, transform);
            activeObstacles.Add(obstacle);
        }
    }

    void SpawnCoinLine(float segmentStartZ)
    {
        if (coinPrefab == null || coinsPerLine <= 0) return;

        int lane = Random.Range(0, 3);
        float x = (lane - 1) * laneDistance;

        for (int i = 0; i < coinsPerLine; i++)
        {
            float z = segmentStartZ + coinLineStartOffset + coinSpacing * i;
            GameObject coin = Instantiate(coinPrefab, new Vector3(x, coinY, z), Quaternion.identity, transform);
            activeCoins.Add(coin);
        }
    }
}