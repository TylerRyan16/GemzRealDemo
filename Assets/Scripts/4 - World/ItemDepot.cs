using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDepot : MonoBehaviour
{
    public List<GameObject> inputZones = new List<GameObject>(); // Assign small cubes as input zones
    private List<Vector3> zonePositions = new List<Vector3>();  // Store initial positions
    private float checkInterval = 1f; // Check for items every second

    private StatsManager statsManager;

    void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
    }



    public void SendItemToInventory(GameObject item)
    {
        string itemName = item.name.ToLower();
        int value = DetermineItemValue(itemName);

        if (value > 0)
        {
            string category = GetItemCategory(itemName);
            statsManager.AddOreToDepot(category, 1);
            statsManager.AddMoney(value);
            statsManager.UpdateStatsInfo();
            Debug.Log($"Depot collected {category} worth {value} credits.");
        }

        Destroy(item); // Destroy the item after processing
    }

    // Determine the value of an item based on its category
    private int DetermineItemValue(string itemName)
    {
        itemName = itemName.ToLower();

        if (itemName.Contains("dirtygarnet")) return 1;   // Least valuable
        if (itemName.Contains("cleanedgarnet")) return 2;   // Least valuable
        if (itemName.Contains("cutgarnet")) return 10;   // Least valuable
        if (itemName.Contains("polishedgarnet")) return 25;   // Least valuable

        if (itemName.Contains("dirtyemerald")) return 5;   // Least valuable
        if (itemName.Contains("cleanedemerald")) return 15;   // Least valuable
        if (itemName.Contains("cutemerald")) return 45;   // Least valuable
        if (itemName.Contains("polishedemerald")) return 125;   // Least valuable

        if (itemName.Contains("dirtytanzanite")) return 100;   // Least valuable
        if (itemName.Contains("cleanedtanzanite")) return 175;   // Least valuable
        if (itemName.Contains("cuttanzanite")) return 250;   // Least valuable
        if (itemName.Contains("polishedtanzanite")) return 525;   // Least valuable



        if (itemName.Contains("rawCoal")) return 1;
        if (itemName.Contains("refinedCoal")) return 2;


        else
        {
            Debug.Log("unrecognized item in DetermineValue: " + itemName);
            return 0;
        }
    }


    // Determine the category of the item based on its name
    private string GetItemCategory(string itemName)
    {
        itemName = itemName.ToLower();

        if (itemName.Contains("dirtygarnet")) return "Dirty Garnet";
        if (itemName.Contains("cleanedgarnet")) return "Cleaned Garnet";
        if (itemName.Contains("cutgarnet")) return "Cut Garnet";
        if (itemName.Contains("polishedgarnet")) return "Polished Garnet";

        if (itemName.Contains("dirtyemerald")) return "Dirty Emerald";
        if (itemName.Contains("cleanedemerald")) return "Cleaned Emerald";
        if (itemName.Contains("cutemerald")) return "Cut Emerald";
        if (itemName.Contains("polishedemerald")) return "Polished Emerald";

        if (itemName.Contains("dirtytanzanite")) return "Dirty Tanzanite";
        if (itemName.Contains("cleanedtanzanite")) return "Cleaned Tanzanite";
        if (itemName.Contains("cuttanzanite")) return "Cut Tanzanite";
        if (itemName.Contains("polishedtanzanite")) return "Polished Tanzanite";

        if (itemName.Contains("rawcoal")) return "Raw Coal";
        if (itemName.Contains("refinedcoal")) return "Refined Coal";

        Debug.LogWarning($"Unrecognized item: {itemName}");
        return null;
    }


    // Visualize input zones in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (GameObject zone in inputZones)
        {
            if (zone != null)
            {
                Gizmos.DrawWireSphere(zone.transform.position, 0.5f); // Adjust radius as needed
            }
        }
    }
}
