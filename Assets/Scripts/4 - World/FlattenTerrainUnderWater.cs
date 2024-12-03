using UnityEngine;

public class FlattenTerrainUnderWater : MonoBehaviour
{
    public Terrain terrain;
    public GameObject waterBlock;
    public float flattenHeight = 0f; // Desired flat height

    void Flatten()
    {
        TerrainData terrainData = terrain.terrainData;

        // Get terrain dimensions
        Vector3 terrainPosition = terrain.transform.position;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        // Calculate area under water block
        Vector3 waterMin = waterBlock.transform.position - waterBlock.transform.localScale / 2f;
        Vector3 waterMax = waterBlock.transform.position + waterBlock.transform.localScale / 2f;

        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        for (int x = 0; x < heightmapWidth; x++)
        {
            for (int y = 0; y < heightmapHeight; y++)
            {
                // Convert terrain position to world space
                float worldX = terrainPosition.x + x / (float)heightmapWidth * terrainData.size.x;
                float worldZ = terrainPosition.z + y / (float)heightmapHeight * terrainData.size.z;

                if (worldX > waterMin.x && worldX < waterMax.x &&
                    worldZ > waterMin.z && worldZ < waterMax.z)
                {
                    heights[y, x] = flattenHeight / terrainData.size.y;
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    private void Start()
    {
        Flatten();
    }
}