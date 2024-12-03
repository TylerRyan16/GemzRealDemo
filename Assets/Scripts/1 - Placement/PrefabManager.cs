using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public List<Item> availableItems;  
    private Item currentItem = null;
    public GameObject displayPrefab;

    public GameObject straightConveyorPrefab; 
    public GameObject cornerConveyorPrefab;
    public GameObject mirroredCornerConveyorPrefab;

    //garnet prefabs
    public GameObject dirtyGarnetPrefab;
    public GameObject cleanedGarnetPilePrefab;
    public GameObject cutGarnetPrefab;
    public GameObject polishedGarnetPrefab;

    // emerald prefabs
    public GameObject dirtyEmeraldPrefab;
    public GameObject cleanedEmeraldPilePrefab;
    public GameObject cutEmeraldPrefab;
    public GameObject polishedEmeraldPrefab;

    // tanzanite prfabs
    public GameObject dirtyTanzanitePrefab;
    public GameObject cleanedTanzanitePilePrefab;
    public GameObject cutTanzanitePrefab;
    public GameObject polishedTanzanitePrefab;

    public GameObject cleanedGemPrefab;

    public Texture2D cursorTexture;

    private void Start()
    {
        currentItem = null;
    }

    public Item GetCurrentItem()
    {
        return currentItem;
    }

    public void SetCurrentItem(int index)
    {
        if (index >= 0 && index < availableItems.Count)
        {
            currentItem = availableItems[index];
        } else
        {
            Debug.LogError("invalid prefab index");
        }
    }

    public void DeselectItems()
    {
        currentItem = null;
    }

    public GameObject GetDisplayPrefab()
    {
        return displayPrefab;
    }

    // GARNET GETTERS

    public GameObject GetDirtyGarnetPrefab()
    {
        return dirtyGarnetPrefab;
    }

    public GameObject GetCleanedGarnetPilePrefab()
    {
        return cleanedGarnetPilePrefab;
    }


    public GameObject GetCutGarnetPrefab()
    {
        return cutGarnetPrefab;
    }

    // EMERALD GETTERS

    public GameObject GetDirtyEmeraldPrefab()
    {
        return dirtyEmeraldPrefab;
    }

    public GameObject GetCleanedEmeraldPilePrefab()
    {
        return cleanedEmeraldPilePrefab;
    }


    public GameObject GetCutEmeraldPrefab()
    {
        return cutEmeraldPrefab; 
    }

    // TANZANITE GETTERS

    public GameObject GetDirtyTanzanitePrefab()
    {
        return dirtyTanzanitePrefab;
    }

    public GameObject GetCleanedTanzanitePilePrefab()
    {
        return cleanedTanzanitePilePrefab;
    }

    public GameObject GetCutTanzanitePrefab()
    {
        return cutTanzanitePrefab;
    }




    // CONVEYORS

    public GameObject GetCornerPrefab()
    {
        return cornerConveyorPrefab;
    }

    public GameObject GetMirroredCornerPrefab()
    {
        return mirroredCornerConveyorPrefab;
    }


    public Texture2D GetCursorTexture()
    {
        return cursorTexture;
    }


}



