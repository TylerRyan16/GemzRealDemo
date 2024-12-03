using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class OreInfoDisplay : MonoBehaviour
{
    public string oreName;
    public string oreDescription;
    public int oreSellValue;
    public GameObject ore3DModel;

    [SerializeField] private GameObject orePanel;
    private GameObject currentModelInstance;


    private void OnMouseDown()
    {
        ShowOreInfo();
    }

    public void ShowOreInfo()
    {
        // Activate the info panel
        Debug.Log("Clicked");
        if (orePanel == null)
        {
            Debug.LogError("OrePanel is not assigned in the Inspector!");
            return;
        }
        orePanel.SetActive(true);
    }


    public void HideOreInfo()
    {
        orePanel.SetActive(false);
    }
}
