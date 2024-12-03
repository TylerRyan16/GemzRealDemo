using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OreDrill : MonoBehaviour
{
    private PrefabManager prefabManager;
    private StatsManager statsManager;
    private AudioSource audioSource;

    private float cost = 50f;
    private bool isPlaced = false;
    private bool isMining = false;
    private RawOre currentOre = null;
    private float drillSpeed;

    // fuel
    private float currentFuel = 0f;
    private float fuelUsagePerCycle;

    // Radius for detecting conveyors
    private float conveyorDetectionRadius = 1.0f;


    private void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
        audioSource = GetComponent<AudioSource>();
        drillSpeed = statsManager.GetDrillSpeed();
        fuelUsagePerCycle = statsManager.GetFuelUsagePerCycle();

        SetPlaced(true);
    }

    private void Update()
    {
        if (isPlaced && isMining == false)
        {
            if (currentFuel <= 0)
            {
                StopMining();
                return;
            }
            bool oreDetected = CheckForOreBelow();
            if (oreDetected)  // Start mining if ore is detected and not already mining
            {
                if (!audioSource.isPlaying) audioSource.Play();
                StartCoroutine(MineOreCycle());  // Start the mining process
            }
        }
    }

    private bool CheckForOreBelow()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("ore"))
            {
                RawOre rawOre = collider.GetComponent<RawOre>();
                if (rawOre != null && rawOre.GetGemsRemaining() > 0)
                {
                    currentOre = rawOre; // Set the current ore reference
                    return true;
                }
            }
        }
        currentOre = null; // Clear the current ore if none found
        return false; // No ore detected
    }

    private IEnumerator MineOreCycle()
    {
        isMining = true;

        while (isMining)  // Continuously mine while ore is detected below
        {
            // Wait 7 seconds between mining attempts
            yield return new WaitForSeconds(drillSpeed);

            // Check if there is still ore below
            if (currentOre == null || currentOre.GetGemsRemaining() <= 0)
            {
                StopMining();
                yield break;
            }

            // Find nearby conveyors and try to add ore
            ConveyorBelt selectedConveyor = FindRandomAvailableConveyor();
            if (selectedConveyor != null && selectedConveyor.itemsOnBelt.Count < selectedConveyor.maxItems)
            {
                // Decrement fuel
                currentFuel -= fuelUsagePerCycle;

                GameObject roughOrePrefab = currentOre.GetRoughPrefab(); // Use the rough prefab from RawOre
                if (roughOrePrefab != null)
                {
                    selectedConveyor.AddItem(roughOrePrefab);

                    // decrement ore vein
                    currentOre.DecrementOreCount(statsManager.GetOreAmountPerCycle());
                    if (currentOre.GetGemsRemaining() <= 0)
                    {
                        if (audioSource.isPlaying) audioSource.Stop();
                        Destroy(currentOre.gameObject);
                    }
                    isMining = false;  // Stop mining if no ore is detected
                }
            }
            else
            {
                //Debug.Log("No available conveyor nearby.");
            }
        }
    }


    private void StopMining()
    {
        isMining = false;
        currentOre = null;
        if (audioSource.isPlaying) audioSource.Stop();
    }


    // Finds a random conveyor with available space within the detection radius
    private ConveyorBelt FindRandomAvailableConveyor()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, conveyorDetectionRadius);
        List<ConveyorBelt> availableConveyors = new List<ConveyorBelt>();

        foreach (Collider collider in hitColliders)
        {
            ConveyorBelt conveyor = collider.GetComponentInParent<ConveyorBelt>();

            // Check if the conveyor is valid and the "start" tag is within the detection radius
            if (conveyor != null && conveyor.IsPlaced() && conveyor.itemsOnBelt.Count < conveyor.maxItems)
            {
                Transform startTransform = conveyor.transform.Find("startDisplay");
                if (startTransform != null)
                {
                    // Check if the "start" object is part of the detected colliders
                    if (Array.Exists(hitColliders, hit => hit.transform == startTransform))
                    {
                        availableConveyors.Add(conveyor);
                    }
                }
            }
        }

        // Return a random conveyor from the available list if any are found
        if (availableConveyors.Count > 0)
        {
            return availableConveyors[UnityEngine.Random.Range(0, availableConveyors.Count)];
        }

        return null;
    }

    // Returns the appropriate ore prefab based on ore type
    private GameObject GetOrePrefabByType(string oreType)
    {
        switch (oreType)
        {
            case "T1": return prefabManager.GetDirtyGarnetPrefab();
            case "T2": return prefabManager.GetDirtyEmeraldPrefab() ;
            case "T3": return prefabManager.GetDirtyTanzanitePrefab();
            default: return null;
        }
    }

    public void SetPlaced(bool placed)
    {
        isPlaced = placed;
    }

    private void OnDrawGizmosSelected()
    {
        // Set the gizmo color
        Gizmos.color = Color.cyan;

        // Draw the detection radius as a wire sphere
        Gizmos.DrawWireSphere(transform.position, conveyorDetectionRadius);

        // Optional: Highlight start points of nearby conveyors
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, conveyorDetectionRadius);
        foreach (Collider collider in hitColliders)
        {
            ConveyorBelt conveyor = collider.GetComponentInParent<ConveyorBelt>();
            if (conveyor != null)
            {
                Transform startTransform = conveyor.transform.Find("start");
                if (startTransform != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(startTransform.position, 0.1f); // Draw small spheres for start points
                }
            }
        }
    }

    public float GetCost()
    {
        return cost;
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
    }

    public float GetFuel()
    {
        return currentFuel;
    }



}
