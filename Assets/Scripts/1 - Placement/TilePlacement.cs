using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class TilePlacement : MonoBehaviour
{

    public ConveyorPlacement conveyor;

    // references
    public TileHover tileHover;
    public PrefabManager prefabManager;
    private GameObject player;
    private PlayerController playerController;
    private ShopMenuManager shopMenuManager;
    private StatsManager statsManager;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        shopMenuManager = FindObjectOfType<ShopMenuManager>();
        statsManager = FindObjectOfType<StatsManager>();
    }

    void Update()
    {

        if (IsMouseOverUI())
        {
            return;
        }

        // DESELECT ITEMS
        if (Input.GetMouseButtonDown(2))
        {
            prefabManager.DeselectItems();
            tileHover.DestroyPreview();
            return;
        }

        // ONLY ALLOW PLACEMENT IF ITEM IS SELECTED
        if (prefabManager.GetCurrentItem() != null)
        {

            if (prefabManager.GetCurrentItem().itemName == "Conveyer Belt")
            {
                // START LEFT CLICK = START CONVEYER PLACEMENT
                if (Input.GetMouseButtonDown(0))
                {
                    conveyor.StartPlacement();
                    shopMenuManager.CloseAllSubmenus();
                }

                // HOLD LEFT CLICK = UPDATE CONVEYOR PLACEMENT
                else if (Input.GetMouseButton(0))
                {
                    conveyor.UpdatePlacement();
                }

                // LET GO OF LEFT CLICK = PLACE ALL
                else if (Input.GetMouseButtonUp(0))
                {
                    conveyor.FinalizePlacement();
                }
            } 
            else 
            {
                // LEFT CLICK = PLACE ITEM
                if (Input.GetMouseButtonDown(0))
                {
                    PlaceItem();
                    shopMenuManager.CloseAllSubmenus();
                }
            }
           
        }


        // keep below funcitons in update
        // ROTATE CLOCKWISE
        if (Input.GetKeyDown(KeyCode.E))
        {
            tileHover.RotatePreviewClockwise();
            conveyor.UpdateAllPreviews();
        }

        // ROTATE COUNTERCLOCKWISE
        if (Input.GetKeyDown(KeyCode.Q))
        {
            tileHover.RotatePreviewCounterClockwise();
            conveyor.UpdateAllPreviews();
        }
    }

    private void PlaceItem()
    {
        if (prefabManager.GetCurrentItem() == null || prefabManager.GetCurrentItem().prefab == null) return;

        // get current grid pos
        Vector3Int gridPosition = tileHover.GetGridPositionUnderCursor();

        if (tileHover.IsPositionOccupiedOrInvalid(gridPosition))
        {
            //Debug.Log("Cannot place item here. Position is occupied or invalid.");
            return;
        }

        // Place the item
        Vector3 placePosition = tileHover.tilemap.GetCellCenterWorld(gridPosition);
        GameObject itemPrefab = prefabManager.GetCurrentItem().prefab;

        if (itemPrefab == null)
        {
           // Debug.LogError("No prefab assigned to the selected item.");
            return;
        }

        GameObject placedItem = Instantiate(itemPrefab, placePosition, Quaternion.Euler(0, tileHover.GetCurrentRotation(), 0));

        // Mark the position as occupied
        tileHover.occupiedGridPositions.Add(gridPosition);

        // Check if the placed item is an OreWasher and set it as placed
        OreWasher washer = placedItem.GetComponent<OreWasher>();
        if (washer != null)
        {
            washer.SetPlaced(true);
        }

        // Deduct money for placement
        Item currentItem = prefabManager.GetCurrentItem();
        statsManager.SubtractMoney(currentItem.cost);
        statsManager.UpdateMoneyText();

    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}





