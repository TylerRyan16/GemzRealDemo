using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileHover : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap tilemap;
    public PrefabManager prefabManager;
    public Material transparentMaterial;
    public Material cannotPlaceMaterial;

    private GameObject previewInstance;
    private Vector3Int lastGridPosition;


    public HashSet<Vector3Int> occupiedGridPositions = new HashSet<Vector3Int>(); // Shared between classes
    private float currentRotation = 0f; // The single rotation variable

    private bool noItemSelected = true;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            PrintOccupiedGridPositions();
        }

        if (prefabManager.GetCurrentItem() == null || prefabManager.GetCurrentItem().prefab == null)
        {
            // If no prefab is selected, destroy the current preview (if it exists) and exit early
            DestroyPreview();

            noItemSelected = true;

            return;
        } else
        {
            noItemSelected = false;
        }

        if (!noItemSelected)
        {
            Vector3Int currentGridPosition = GetGridPositionUnderCursor();

            // Create or move the preview instance
            if (previewInstance != null && prefabManager.GetCurrentItem().prefab != previewInstance)
            {
                DestroyPreview();
            }
            if (previewInstance == null)
            {
                CreatePreview(currentGridPosition);
            }
            else
            {
                MovePreview(currentGridPosition);
            }

            // Apply the correct material
            MaterialUtility.ApplyMaterial(previewInstance, currentGridPosition, tilemap, occupiedGridPositions, cannotPlaceMaterial, transparentMaterial);
        }
        
    }

    public Vector3Int GetGridPositionUnderCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane gridPlane = new Plane(Vector3.up, new Vector3(0, 0.86f, 0)); // Set the plane height of the tilemap (0.86)
        float distance;

        if (gridPlane.Raycast(ray, out distance))
        {
            Vector3 hitPosition = ray.GetPoint(distance);
            return tilemap.WorldToCell(hitPosition);
        }
        return Vector3Int.zero;
    }

    void CreatePreview(Vector3Int gridPosition)
    {
        lastGridPosition = gridPosition;
        Vector3 placePosition = tilemap.GetCellCenterWorld(gridPosition);
        previewInstance = Instantiate(prefabManager.GetCurrentItem().prefab, placePosition, Quaternion.Euler(0, currentRotation, 0));

        MaterialUtility.ApplyMaterial(previewInstance, gridPosition, tilemap, occupiedGridPositions, cannotPlaceMaterial, transparentMaterial);
    }

    void MovePreview(Vector3Int gridPosition)
    {
        if (gridPosition != lastGridPosition)
        {
            lastGridPosition = gridPosition;
            Vector3 placePosition = tilemap.GetCellCenterWorld(gridPosition);
            previewInstance.transform.position = placePosition;
            previewInstance.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
    }

    public void RotatePreviewClockwise()
    {
        currentRotation += 90f;
        if (currentRotation >= 360f)
        {
            currentRotation = 0f;
        }
        if (previewInstance != null)
        {
            previewInstance.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
    }

    public void RotatePreviewCounterClockwise()
    {
        currentRotation -= 90f;
        if (currentRotation < 0f)
        {
            currentRotation = 270f; // Wrap from -90 to 270
        }
        if (previewInstance != null)
        {
            previewInstance.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
    }

    public bool IsPositionOccupiedOrInvalid(Vector3Int gridPosition)
    {
        return tilemap.HasTile(gridPosition) || occupiedGridPositions.Contains(gridPosition);
    }

    public void DestroyPreview()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    public float GetCurrentRotation()
    {
        return currentRotation;
    }

    public void SetRotation(float rotation)
    {
        currentRotation = rotation;
        if (previewInstance != null)
        {
            previewInstance.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
    }

    public void PrintOccupiedGridPositions()
    {
        if (occupiedGridPositions.Count == 0)
        {
            Debug.Log("occupiedGridPositions is empty.");
        }
        else
        {
            string positions = "Occupied Grid Positions: ";
            foreach (var position in occupiedGridPositions)
            {
                positions += position.ToString() + ", ";
            }

            // Remove the trailing comma and space for cleaner output
            positions = positions.TrimEnd(',', ' ');

            Debug.Log(positions);
        }
    }
}





