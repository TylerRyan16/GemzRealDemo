using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OreSpawner : MonoBehaviour
{
    [System.Serializable]
    public class OreWithRarity
    {
        public GameObject orePrefab;
        public int spawnCount;
    }

    public List<OreWithRarity> ores;
    public Tilemap tilemap;

    public int gridMin = -500;
    public int gridMax = 500;

    void Start()
    {
        SpawnOres();
    }

    void SpawnOres()
    {
        foreach (var ore in ores)
        {
            RawOre oreScript = ore.orePrefab.GetComponent<RawOre>();
            if (oreScript != null)
            {
                // Define ranges based on rarity
                int rarity = oreScript.GetOreRarity();
                (int minDistance, int maxDistance) = GetDistanceRangeForRarity(rarity);

                // Spawn ores within the calculated range
                SpawnOreInRange(ore.orePrefab, ore.spawnCount, minDistance, maxDistance);
            }
            else
            {
                Debug.LogWarning($"Ore prefab {ore.orePrefab.name} is missing a RawOre component!");
            }
        }
    }

    (int minDistance, int maxDistance) GetDistanceRangeForRarity(int rarity)
    {
        switch (rarity)
        {
            case 1:
                return (25, 300);
            case 2:
                return (250, 390);
            case 3:
                return (360, 500);
            default:
                Debug.LogWarning($"Undefined rarity: {rarity}. Defaulting to full range.");
                return (0, 500);
        }
    }

    void SpawnOreInRange(GameObject orePrefab, int oreCount, int minDistance, int maxDistance)
    {
        List<Vector3Int> spawnPositions = new List<Vector3Int>();

        for (int i = 0; i < oreCount; i++)
        {
            bool validPosition = false;
            Vector3Int spawnPosition = Vector3Int.zero;

            for (int attempts = 0; attempts < 10; attempts++)
            {
                // Randomly choose a position within a ring around the center
                float angle = Random.Range(0, Mathf.PI * 2);
                float distance = Random.Range(minDistance, maxDistance);

                int x = Mathf.RoundToInt(distance * Mathf.Cos(angle));
                int y = Mathf.RoundToInt(distance * Mathf.Sin(angle));

                spawnPosition = new Vector3Int(x, y, 1);

                // Ensure position validity
                if (IsPositionValid(spawnPosition, spawnPositions))
                {
                    validPosition = true;
                    break;
                }
            }

            if (validPosition)
            {
                // Convert grid position to world position
                Vector3 placePosition = tilemap.GetCellCenterWorld(spawnPosition);
                placePosition.y = 1.1f; // Adjust height for ore placement

                Instantiate(orePrefab, placePosition, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
                spawnPositions.Add(spawnPosition);
            }
        }
    }

    bool IsPositionValid(Vector3Int position, List<Vector3Int> existingPositions, float minDistance = 2f)
    {
        foreach (Vector3Int existingPosition in existingPositions)
        {
            if (Vector3Int.Distance(position, existingPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}
