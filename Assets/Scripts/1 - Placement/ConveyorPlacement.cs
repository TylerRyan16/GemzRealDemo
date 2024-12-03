using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConveyorPlacement : MonoBehaviour
{
    // references
    public Tilemap tilemap;
    private TileHover tileHover;
    public PrefabManager prefabManager;
    private PlayerController playerController; 
    private ShopMenuManager shopMenuManager;
    private StatsManager statsManager;

    // placement variables
    private List<GameObject> previewInstances = new List<GameObject>();
    private Vector3Int lastGridPosition;
    private GameObject lastPlacedConveyer;
    private float lastConveyerRotation;
    private float initialPreviewRotation;
    private AudioSource audioSource;
    private Vector3Int lastDirection;



    // Start is called before the first frame update
    void Start()
    {
        tileHover = GameObject.FindObjectOfType<TileHover>();
        playerController = FindObjectOfType<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        shopMenuManager = FindObjectOfType<ShopMenuManager>();
        statsManager = FindObjectOfType<StatsManager>();
    }



    public void StartPlacement()
    {
        initialPreviewRotation = tileHover.GetCurrentRotation();

        // CLEAR PREVIEWS, SAVE LAST GRID POSITION
        ClearPreviews();
        lastGridPosition = tileHover.GetGridPositionUnderCursor();

        // Determine initial direction based on rotation
        lastDirection = GetDirectionFromRotation(tileHover.GetCurrentRotation());

        if (lastPlacedConveyer == null)
        {
            lastConveyerRotation = tileHover.GetCurrentRotation();

            if (lastGridPosition != null)
            {
                AddPreviewInstance(lastGridPosition, lastConveyerRotation); // Use the global rotation for initial placement

            }


            lastPlacedConveyer = prefabManager.GetCurrentItem().prefab;
        }
        else
        {
            AddPreviewInstance(lastGridPosition, lastConveyerRotation);
        }
    }

    public void UpdatePlacement()
    {
        Vector3Int currentGridPosition = tileHover.GetGridPositionUnderCursor();

        // IF MOVING GRID SQUARES
        if (currentGridPosition != lastGridPosition)
        {
            Vector3Int currentDirection = currentGridPosition - lastGridPosition;



            // replace with corner
            if (currentDirection != lastDirection && previewInstances.Count > 1)
            {
                ReplaceWithCornerConveyor(previewInstances[previewInstances.Count - 1], lastDirection, currentDirection);  
            }


            // Update the current direction and rotation
            float newRotation = CalculateDynamicRotationBasedOnLastConveyer(currentGridPosition);
            AddPreviewInstance(currentGridPosition, newRotation);

            // Update placement state
            lastGridPosition = currentGridPosition;
            lastDirection = currentDirection;
        }
    }


    public void FinalizePlacement()
    {
        if (previewInstances.Count > 0)
        {
            float totalCost = 0f;

            foreach (var preview in previewInstances)
            {
                Vector3Int gridPosition = tilemap.WorldToCell(preview.transform.position);
                if (!tileHover.IsPositionOccupiedOrInvalid(gridPosition))
                {

                    GameObject prefabToInstantiate;

                    if (preview.CompareTag("CornerConveyor"))
                    {
                        prefabToInstantiate = prefabManager.cornerConveyorPrefab;
                    }
                    else if (preview.CompareTag("StraightConveyor"))
                    {
                        prefabToInstantiate = prefabManager.straightConveyorPrefab;
                    } else if (preview.CompareTag("MirrorCorner"))
                    {
                        prefabToInstantiate = prefabManager.GetMirroredCornerPrefab();
                    }
                    else 
                    {
                        prefabToInstantiate = prefabManager.GetCurrentItem().prefab;
                    }

                    Vector3 placePosition = tilemap.GetCellCenterWorld(gridPosition);
                    GameObject finalizedInstance = Instantiate(prefabToInstantiate, placePosition, preview.transform.rotation);

                    tileHover.occupiedGridPositions.Add(gridPosition);

                    ConveyorBelt conveyorBelt = finalizedInstance.GetComponent<ConveyorBelt>();

                    if (conveyorBelt != null)
                    {
                        if (preview.CompareTag("CornerConveyor"))
                        {
                            // true means corner
                            conveyorBelt.SetAsPlaced(true);
                        }
                        else if (preview.CompareTag("MirrorCorner"))
                        {
                            // true means corner
                            conveyorBelt.SetAsPlaced(true);
                        }
                        else
                        {
                            // false means not corner
                            conveyorBelt.SetAsPlaced(false); 
                        }
                    }

                    // Update the last placed conveyor reference
                    lastPlacedConveyer = finalizedInstance;
                    lastConveyerRotation = preview.transform.eulerAngles.y;

                    // Deduct money for placement
                    Item currentItem = prefabManager.GetCurrentItem();
                    statsManager.SubtractMoney(currentItem.cost);

                    // Update the UI
                    statsManager.UpdateMoneyText();
                }
            }
            ClearPreviews();
        }

        tileHover.SetRotation(lastConveyerRotation);
        lastPlacedConveyer = null;
    }



    float CalculateDynamicRotationBasedOnLastConveyer(Vector3Int currentGridPosition)
    {
        if (lastPlacedConveyer == null) return tileHover.GetCurrentRotation(); // Use the current hover rotation if no last conveyer

        Vector3Int gridDifference = currentGridPosition - lastGridPosition;
        float newRotation = lastConveyerRotation;

        string currentFacingDirection = "";
        if (lastConveyerRotation == 0f) currentFacingDirection = "-x";
        else if (lastConveyerRotation == 90f) currentFacingDirection = "+z";
        else if (lastConveyerRotation == 180f) currentFacingDirection = "+x";
        else if (lastConveyerRotation == 270f) currentFacingDirection = "-z";


        // FACING -X
        if (currentFacingDirection == "-x")
        {
            if (gridDifference.y > 0)
            {
                //Debug.Log("Turning Right");
                newRotation = 90f;
            }
            else if (gridDifference.y < 0)
            {
                // Debug.Log("Turning Left");
                newRotation = 270f;
            }
            else if (gridDifference.x > 0)
            {
                newRotation = 180f;
            }
        }

        // FACING +X
        if (currentFacingDirection == "+x")
        {
            if (gridDifference.y > 0)
            {
                newRotation = 90f;
            }
            else if (gridDifference.y < 0)
            {
                newRotation = 270f;
            }
            else if (gridDifference.x < 0)
            {
                newRotation = 0f;
            }
            else if (gridDifference.x > 0)
            {
                newRotation = 180f;
            }
        }

        // FACING -Z
        if (currentFacingDirection == "-z")
        {
            if (gridDifference.x > 0)
            {
                newRotation = 180f;
            }
            else if (gridDifference.x < 0)
            {
                newRotation = 0f;
            }

            else if (gridDifference.y > 0)
            {
                newRotation = 90f;
            }
        }

        // FACING +Z
        if (currentFacingDirection == "+z")
        {
            if (gridDifference.x > 0)
            {
                newRotation = 180f;
            }
            else if (gridDifference.x < 0)
            {
                newRotation = 0f;
            }

            else if (gridDifference.y < 0)
            {
                newRotation = 270f;
            }
        }

        tileHover.SetRotation(newRotation);
        return newRotation;
    }

    private Vector3Int GetDirectionFromRotation(float rotation)
    {
        // Map the rotation to a grid direction
        if (Mathf.Approximately(rotation, 0f))
            return new Vector3Int(-1, 0, 0); // Facing -X
        else if (Mathf.Approximately(rotation, 90f))
            return new Vector3Int(0, 1, 0); // Facing +Y
        else if (Mathf.Approximately(rotation, 180f))
            return new Vector3Int(1, 0, 0); // Facing +X
        else if (Mathf.Approximately(rotation, 270f))
            return new Vector3Int(0, -1, 0); // Facing -Y
        else
            return Vector3Int.zero; // Default to no direction if undefined
    }

    private void ReplaceWithCornerConveyor(GameObject previewInstance, Vector3Int fromDirection, Vector3Int toDirection)
    {
        if (previewInstance == null) return;

        // Get the position and parent of the preview instance
        Vector3 position = previewInstance.transform.position;
        Transform parent = previewInstance.transform.parent;

        // Calculate rotation for the corner conveyor
        float cornerRotation = CalculateCornerRotation(fromDirection, toDirection);

        // Destroy the old preview instance
        Destroy(previewInstance);

        // Determine whether to use the mirrored corner conveyor
        GameObject cornerConveyorPrefabToUse;
        GameObject cornerConveyor;

        if ((fromDirection == new Vector3Int(0, -1, 0) && toDirection == new Vector3Int(1, 0, 0)) ||  // Down to Right
            (fromDirection == new Vector3Int(0, 1, 0) && toDirection == new Vector3Int(-1, 0, 0)) ||
            (fromDirection == new Vector3Int(-1, 0, 0) && toDirection == new Vector3Int(0, -1, 0))||
            fromDirection == new Vector3Int(1, 0, 0) && toDirection == new Vector3Int(0, 1, 0))   
        {
            cornerConveyorPrefabToUse = prefabManager.GetMirroredCornerPrefab();
            // Instantiate the selected corner conveyor prefab
            cornerConveyor = Instantiate(cornerConveyorPrefabToUse, position, Quaternion.Euler(0, cornerRotation, 0), parent);
            cornerConveyor.tag = "MirrorCorner";
        }
        else
        {

            cornerConveyorPrefabToUse = prefabManager.GetCornerPrefab();
            cornerConveyor = Instantiate(cornerConveyorPrefabToUse, position, Quaternion.Euler(0, cornerRotation, 0), parent);
            cornerConveyor.tag = "CornerConveyor";
        }


        // Replace the preview instance in the list
        previewInstances[previewInstances.Count - 1] = cornerConveyor;
    }


    private float CalculateCornerRotation(Vector3Int fromDirection, Vector3Int toDirection)
    {
        // Map the direction changes to corresponding rotations
        if (fromDirection == new Vector3Int(-1, 0, 0) && toDirection == new Vector3Int(0, 1, 0)) return 0f;   // Left to Up
        if (fromDirection == new Vector3Int(-1, 0, 0) && toDirection == new Vector3Int(0, -1, 0)) return 180f; // Left to Down
        if (fromDirection == new Vector3Int(1, 0, 0) && toDirection == new Vector3Int(0, 1, 0)) return 0f; // Right to Up
        if (fromDirection == new Vector3Int(1, 0, 0) && toDirection == new Vector3Int(0, -1, 0)) return 180f; // Right to Down
        if (fromDirection == new Vector3Int(0, 1, 0) && toDirection == new Vector3Int(-1, 0, 0)) return 270f; // Up to Left
        if (fromDirection == new Vector3Int(0, 1, 0) && toDirection == new Vector3Int(1, 0, 0)) return 90f;   // Up to Right
        if (fromDirection == new Vector3Int(0, -1, 0) && toDirection == new Vector3Int(-1, 0, 0)) return 270f; // Down to Left
        if (fromDirection == new Vector3Int(0, -1, 0) && toDirection == new Vector3Int(1, 0, 0)) return 90f;   // Down to Right

        return 0f; // Default case
    }



    public void AddPreviewInstance(Vector3Int gridPosition, float rotation)
    {
        Vector3 placePosition = tilemap.GetCellCenterWorld(gridPosition);
        var currentItem = prefabManager.GetCurrentItem();

        if (currentItem == null || currentItem.prefab == null)
        {
            Debug.LogWarning("No item selected or prefab missing in PrefabManager. Unable to create preview.");
            return;
        }

        GameObject previewInstance = Instantiate(prefabManager.GetCurrentItem().prefab, placePosition, Quaternion.Euler(0, rotation, 0));
        if (previewInstance.CompareTag("Untagged")) // Optional check to avoid overwriting other tags
        {
            previewInstance.tag = "StraightConveyor";
        }

        // Apply the transparent material to previews
        Renderer[] renderers = previewInstance.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.material = tileHover.transparentMaterial;
        }

        previewInstances.Add(previewInstance);
        audioSource.Play();
    }

    public void UpdateAllPreviews()
    {
        // Update the rotation of all previews based on the current rotation
        for (int i = 0; i < previewInstances.Count; i++)
        {
            Vector3Int gridPosition = tilemap.WorldToCell(previewInstances[i].transform.position);
            previewInstances[i].transform.position = tilemap.GetCellCenterWorld(gridPosition);
            previewInstances[i].transform.rotation = Quaternion.Euler(0, tileHover.GetCurrentRotation(), 0);
        }
    }

    void ClearPreviews()
    {
        foreach (var previewInstance in previewInstances)
        {
            Destroy(previewInstance);
        }
        previewInstances.Clear();
    }


}