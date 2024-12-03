using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // prefab manager
    public PrefabManager prefabManager;
    public ShopMenuManager shopMenuManager;
    public ItemCardsDisplay itemCardsDisplay;
   


    private void Start()
    {
        shopMenuManager = FindObjectOfType<ShopMenuManager>();
    }

    private void Update()
    {
        HandleKeybinds();

        
        
    }

    public void HandleKeybinds()
    {
        // dont accept these keybinds if an item card is open
        if (itemCardsDisplay.IsInfoCardOpen())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            prefabManager.SetCurrentItem(0); // switch to conveyor
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            prefabManager.SetCurrentItem(1); // Switch to ore miner
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            prefabManager.SetCurrentItem(2); // Switch to ore washer
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            prefabManager.SetCurrentItem(3); // Switch to chest
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (shopMenuManager.IsShopOpen())
            {
                shopMenuManager.CloseMainShopMenuAndSubmenus();
            }
            else
            {
                shopMenuManager.OpenMainShop();
            }
        }
    }


}





