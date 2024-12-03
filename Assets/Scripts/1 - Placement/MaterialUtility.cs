using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MaterialUtility
{
    public static void ApplyMaterial(GameObject previewObject, Vector3Int gridPosition, Tilemap tilemap, HashSet<Vector3Int> occupiedGridPositions, Material cannotPlaceMaterial, Material transparentMaterial)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (tilemap.HasTile(gridPosition) || occupiedGridPositions.Contains(gridPosition))
            {
                renderer.material = cannotPlaceMaterial;
            }
            else
            {
                renderer.material = transparentMaterial;
            }
        }
    }
}

