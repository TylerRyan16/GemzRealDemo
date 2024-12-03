using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName; 
    public GameObject prefab;  
    public float cost;  

    public Item(string name, GameObject prefab, float cost)
    {
        this.itemName = name;
        this.prefab = prefab;
        this.cost = cost;
    }

    
}
