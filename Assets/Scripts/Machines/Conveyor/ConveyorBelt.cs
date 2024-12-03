using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using static UnityEditor.Progress;

public class ConveyorBelt : MonoBehaviour
{
    private PrefabManager prefabManager;
    private StatsManager stats;

    private float cost = 1f;



    public List<GameObject> itemsOnBelt = new List<GameObject>();
    public int maxItems = 1;
    private GameObject miniatureItemPrefab;

    // line rendering
    private LineRenderer lineRenderer;
    private Transform displayStartPoint;
    private Transform displayMidPoint;
    private Transform displayEndPoint;

    public bool isCornerConveyor = false;
    private bool isPlaced = false;

    // check for transfers in stationary items
    private float transferCheckInterval = 0.2f; // Interval in seconds to re-check transfer
    private float transferCheckTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        // grab references
        prefabManager = FindObjectOfType<PrefabManager>();
        stats = FindObjectOfType<StatsManager>();

        InitializePositions();
    }

    private void InitializePositions()
    {
        displayStartPoint = transform.Find("startDisplay");


        if (isCornerConveyor)
        {

            displayMidPoint = transform.Find("MiddleDisplay");
        }

        displayEndPoint = transform.Find("endDisplay");
    }

    private void Update()
    {
        if (!isPlaced) return;


        if (itemsOnBelt.Count >= maxItems)
        {
            transferCheckTimer += Time.deltaTime;
            if (transferCheckTimer >= transferCheckInterval)
            {
                transferCheckTimer = 0f;
                CheckForAvailableTransfers();
            }
        }

    }

    public bool AddItem(GameObject item)
    {
        if (!isPlaced || itemsOnBelt.Count >= maxItems) return false;

        if (item == null)
        {
            Debug.LogError("Item is null! Check what is being passed into AddItem.");
            return false;
        }

        // Instantiate the miniature item
        GameObject displayItem = Instantiate(item, displayStartPoint.position, Quaternion.identity, transform);
        itemsOnBelt.Add(displayItem);
        StartCoroutine(MoveItemAlongConveyor(displayItem));

        return true;
    }


    // Coroutine to move an item from displayStartPoint to displayEndPoint
    private IEnumerator MoveItemAlongConveyor(GameObject item)
    {
        if (!isPlaced) yield break;

        // movement for corner conveyors
        if (isCornerConveyor)
        {
            if (displayMidPoint != null)
            {
                while (item != null && Vector3.Distance(item.transform.position, displayMidPoint.position) > 0.01f)
                {
                    item.transform.position = Vector3.MoveTowards(
                        item.transform.position,
                        displayMidPoint.position,
                        stats.GetConveyorBeltSpeed() * Time.deltaTime
                    );
                    yield return null;
                }
            }

        }


        // movement for straight conveyors
        while (item != null && Vector3.Distance(item.transform.position, displayEndPoint.position) > 0.01f)
        {
            item.transform.position = Vector3.MoveTowards(
                item.transform.position,
                displayEndPoint.position,
                stats.GetConveyorBeltSpeed() * Time.deltaTime
            );
            yield return null;
        }

        // check for next object
        MonoBehaviour nextObject = GetNextObject();

        if (nextObject != null && item != null)
        {
            if (nextObject is ConveyorBelt nextConveyor && nextConveyor.itemsOnBelt.Count < nextConveyor.maxItems && nextConveyor.IsPlaced())
            {
                itemsOnBelt.Remove(item);
                nextConveyor.AddItem(item);
                Destroy(item);
            }
            else if (nextObject is OreWasher nextWasher && nextWasher.itemsOnWasher.Count < nextWasher.maxItems && nextWasher.IsPlaced())
            {
                itemsOnBelt.Remove(item);
                nextWasher.AddItem(item);
                Destroy(item);
            }
            else if (nextObject is ItemDepot itemDepot)
            {
                itemsOnBelt.Remove(item);
                itemDepot.SendItemToInventory(item);
                Destroy(item);
            } 
            else
            {
                // Stay at end point if next object is full
                item.transform.position = displayEndPoint.position;
            }
        }
        else
        {
            // Stay at end point if no next object
            if (item != null)
            {
                item.transform.position = displayEndPoint.position;
            }
        }

    }

    private MonoBehaviour GetNextObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(displayEndPoint.position, 0.1f);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("start") && collider.transform.parent != transform)
            {
                ConveyorBelt conveyor = collider.GetComponentInParent<ConveyorBelt>();
                if (conveyor != null) return conveyor;

                OreWasher washer = collider.GetComponentInParent<OreWasher>();
                if (washer != null) return washer;

                // Add other machines here (e.g., Smelters, Assemblers)

            }

            if (collider.CompareTag("depotInput"))
            {
                ItemDepot depot = collider.GetComponentInParent<ItemDepot>();
                if (depot != null) return depot;
            }

        }
        return null;
    }

    private ConveyorBelt GetNextConveyor()
    {
        if (!isPlaced) return null;

        Collider[] hitColliders = Physics.OverlapSphere(displayEndPoint.position, 0.3f);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("start") && collider.transform.parent != transform)
            {
                ConveyorBelt nextConveyor = collider.GetComponentInParent<ConveyorBelt>();
                if (nextConveyor != null && nextConveyor.itemsOnBelt.Count < nextConveyor.maxItems)
                {
                    return nextConveyor;
                }
            }
        }
        return null;
    }


    private void CheckForAvailableTransfers()
    {
        if (!isPlaced) return;

        MonoBehaviour nextObject = GetNextObject();

        if (nextObject == null) return;

        List<GameObject> itemsToTransfer = new List<GameObject>();


        foreach (GameObject item in itemsOnBelt)
        {
            // Check if the item is stationary and near the end of the conveyor
            if (Vector3.Distance(item.transform.position, displayEndPoint.position) < 0.1f)
            {
                itemsToTransfer.Add(item); // Mark for transfer
            }
        }

        // Process the transfer after checking
        foreach (GameObject item in itemsToTransfer)
        {
               
            if (nextObject is ConveyorBelt nextConveyor && nextConveyor.itemsOnBelt.Count < nextConveyor.maxItems && nextConveyor.IsPlaced())
            {
                itemsOnBelt.Remove(item); // Remove from the current conveyor
                nextConveyor.AddItem(item); // Add it to the next conveyor
                Destroy(item); // Destroy the item instance here
            }
            else if (nextObject is OreWasher nextWasher && nextWasher.itemsOnWasher.Count < nextWasher.maxItems && nextWasher.IsPlaced())
            {
                itemsOnBelt.Remove(item); // Remove from the current conveyor
                nextWasher.AddItem(item); // Add it to the washer
                Destroy(item); // Destroy the item instance here
            }
            else
            {
                // Stay at end point if the next object is full
                item.transform.position = displayEndPoint.position;
            }            
        }
    }

    public void SetAsPlaced(bool isCorner)
    {
        isPlaced = true;
        isCornerConveyor = isCorner;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }


    public float GetCost()
    {
        return cost;
    }
}