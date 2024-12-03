using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class ItemCardsDisplay : MonoBehaviour
{
    // references
    private PrefabManager prefabManager;
    private TileHover tileHover;
    private StatsManager statsManager;
    private ShopMenuManager shopMenuManager;
    private AudioSource audioSource;

    // shop bar & tab bar
    public GameObject moneySpeedText;

    // material dictionary
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

    // info card and children
    public GameObject InfoCard;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    // fuel input
    public Button maxFuelButton;
    public TextMeshProUGUI currentFuelToAddText;
    public TMP_InputField fuelInputField;

    public TextMeshProUGUI fuelText;

    public Image itemImageUI;
    public Button deleteButton;

    // item images
    public Sprite conveyorBeltImage;
    public Sprite oreDrillImage;
    public Sprite oreWasherImage;

    // audio
    public AudioClip sellSound;

    // fuel
    public int fuelToAdd = 0;

    // the selected object that we clicked on
    private GameObject selectedObject;
    private bool isInfoCardOpen = false; // To lock adjustment
    private Dictionary<GameObject, Vector2> originalPositions = new Dictionary<GameObject, Vector2>();

    private void Start()
    {
        prefabManager = FindObjectOfType<PrefabManager>();
        tileHover = FindObjectOfType<TileHover>();
        statsManager = FindObjectOfType<StatsManager>();
        shopMenuManager = FindObjectOfType<ShopMenuManager>();
        audioSource = GetComponent<AudioSource>();

        // listeners
        deleteButton.onClick.AddListener(DeleteSelectedObject);
        fuelInputField.onValueChanged.AddListener(OnFuelInputChanged);
        maxFuelButton.onClick.AddListener(AddMaxFuelToTally);


        SaveOriginalPositions();
    }

    private void SaveOriginalPositions()
    {
        SaveOriginalPosition(moneySpeedText);
    }

    private void SaveOriginalPosition(GameObject uiElement)
    {
        if (uiElement != null)
        {
            RectTransform rect = uiElement.GetComponent<RectTransform>();
            if (rect != null && !originalPositions.ContainsKey(uiElement))
            {
                originalPositions[uiElement] = rect.anchoredPosition;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfPlayerClickedObject();

        if (!isInfoCardOpen) return;
       

        if (selectedObject != null && Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.X))
        {
            // input item cost here
            DeleteSelectedObject();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            AddMaxFuelToTally();
        }

        // Rotate counter clockwise
        if (selectedObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            RotateSelectedObject(-90f);
        }

        // rotate clockwise
        if (selectedObject != null && Input.GetKeyDown(KeyCode.E))
        {
            RotateSelectedObject(90f);
        }

    }

    public void CheckIfPlayerClickedObject()
    {
        if (prefabManager.GetCurrentItem() != null) return; 

        if (Input.GetMouseButtonDown(0))
        {

            // If the mouse is over a UI element, don't process object clicks
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Perform a raycast from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check for various object types
                ConveyorBelt conveyorBelt = hit.collider.GetComponent<ConveyorBelt>();
                OreDrill oreDrill = hit.collider.GetComponent<OreDrill>();
                OreWasher oreWasher = hit.collider.GetComponent<OreWasher>();



                // remove highlight from previous object
                if (selectedObject != null)
                {
                    RemoveHighlight(selectedObject);
                }

                // Determine which object was clicked and update UI
                if (conveyorBelt != null)
                {
                    selectedObject = conveyorBelt.gameObject;
                    HighlightSelectedObject(selectedObject);
                    UpdateInfoCard("Conveyor Belt", "Moves items between locations.", conveyorBeltImage, selectedObject);
                    return;
                }
                else if (oreDrill != null)
                {
                    selectedObject = oreDrill.gameObject;
                    HighlightSelectedObject(selectedObject);
                    UpdateInfoCard("Ore Drill", "Extracts resources from the ground.", oreDrillImage, selectedObject);
                    return;
                }
                else if (oreWasher != null)
                {
                    selectedObject = oreWasher.gameObject;
                    HighlightSelectedObject(selectedObject);
                    UpdateInfoCard("Ore Washer", "Washes the dirt off of rough gems.", oreWasherImage, selectedObject);
                    return;
                }
                /*else if (storageContainer != null)
                {
                    UpdateInfoCard("Storage Container", "Holds resources and items.", storageContainerImage);
                    return;
                }
                else if (refinery != null)
                {
                    UpdateInfoCard("Refinery", "Processes raw materials into refined goods.", refineryImage);
                    return;
                }*/
            }

            // If no relevant object was clicked, hide the info card

            if (selectedObject != null)
            {
                RemoveHighlight(selectedObject);
                HideInfoCard();
            }
        }
    }

    // Helper function to update the info card
    private void UpdateInfoCard(string itemName, string itemDescription, Sprite itemImage, GameObject selectedObject)
    {
        // set card active
        InfoCard.SetActive(true);

        // get item variables
        itemNameText.text = itemName;
        itemImageUI.sprite = itemImage;
        itemDescriptionText.text = itemDescription;

        // reset fuel variables
        fuelToAdd = 0;
        currentFuelToAddText.text = "0";
        UpdateFuelText();

        // push bars over
        AdjustUIElementPositions();

        // set info card to open
        isInfoCardOpen = true;
    }

    public void IncreaseFuelTally(int amountToAdd)
    {
        int amountTotal = fuelToAdd + amountToAdd;
        Debug.Log("increasing fuel tally");

        // only allow increase if we have enough
        if (amountTotal <= statsManager.GetCoalCount())
        {
            fuelToAdd += amountToAdd;

            string formattedText = FormatNumber(fuelToAdd);
            currentFuelToAddText.text = formattedText;

            AdjustFuelTextSize(formattedText); // Adjust font size
        }
    }

    private string FormatNumber(float number)
    {
        if (number >= 1_000_000_000_000)
            return (number / 1_000_000_000_000f).ToString("0.##") + "T"; // Trillion
        else if (number >= 1_000_000_000)
            return (number / 1_000_000_000f).ToString("0.##") + "B"; // Billion
        else if (number >= 1_000_000)
            return (number / 1_000_000f).ToString("0.##") + "M"; // Million
        else if (number >= 1_000)
            return (number / 1_000f).ToString("0.##") + "K"; // Thousand
        else
            return number.ToString("0"); // Less than 1,000
    }

    // Helper to adjust font size dynamically
    private void AdjustFuelTextSize(string formattedText)
    {
        int digitCount = formattedText.Length;

        if (digitCount <= 2)
        {
            currentFuelToAddText.fontSize = 28; 
        }
        else if (digitCount <= 4)
        {
            currentFuelToAddText.fontSize = 24; 
        } else if (digitCount <= 5)
        {
            currentFuelToAddText.fontSize = 18;
        } else if (digitCount <= 7)
        {
            currentFuelToAddText.fontSize = 14;
        }
    }



    public void AddMaxFuelToTally()
    {
        fuelToAdd += statsManager.GetCoalCount();

        currentFuelToAddText.text = FormatNumber(fuelToAdd);
    }


    public void ClearFuelTally()
    {
        fuelToAdd = 0;

        currentFuelToAddText.text = FormatNumber(fuelToAdd);
    }


    public void OnFuelInputChanged(string inputValue)
    {
        if (int.TryParse(inputValue, out int fuelAmount))
        {
            fuelToAdd = Mathf.Max(fuelAmount, 0); // Ensure fuelToAdd is not negative

            string formattedText = FormatNumber(fuelToAdd);
            currentFuelToAddText.text = formattedText;

            AdjustFuelTextSize(formattedText); // Adjust font size
        }
        else
        {
            Debug.LogWarning("Invalid input in fuel input field. Please enter a number.");
        }
    }

    public void AddFuel()
    {
        if (statsManager.GetCoalCount() <= 0) return;

        // stop if we're adding more than we have
        if (fuelToAdd > statsManager.GetCoalCount()) return;

        if (selectedObject.TryGetComponent<OreDrill>(out OreDrill oreDrill))
        {
            oreDrill.AddFuel(fuelToAdd); 
            statsManager.RemoveCoal(fuelToAdd);
            statsManager.UpdateStatsInfo();
            UpdateFuelText();
        }
        else if (selectedObject.TryGetComponent<OreWasher>(out OreWasher oreWasher))
        {
            oreWasher.AddFuel(fuelToAdd); 
            statsManager.RemoveCoal(fuelToAdd);
            statsManager.UpdateStatsInfo();
            UpdateFuelText();
        }
        else
        {
            Debug.LogWarning("Selected object does not support fuel addition.");
        }
    }

    public void UpdateFuelText()
    {
        // Try to get the `GetFuel` method from the specific component
        if (selectedObject.TryGetComponent<OreDrill>(out OreDrill oreDrill))
        {
            fuelText.text = $"Fuel: {oreDrill.GetFuel():F2}";

        }
        else if (selectedObject.TryGetComponent<OreWasher>(out OreWasher oreWasher))
        {
            fuelText.text = $"Fuel: {oreWasher.GetFuel():F2}";

        }
        else
        {
            fuelText.text = "Fuel: N/A"; // Default text if no fuel method is available
        }
    }


    private void HighlightSelectedObject(GameObject obj)
    {
        if (obj != null)
        {
            // Save the original materials if not already saved
            if (!originalMaterials.ContainsKey(obj))
            {
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                Material[] originalMats = new Material[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    originalMats[i] = renderers[i].material;
                }
                originalMaterials[obj] = originalMats;
            }

            // Apply the transparent material
            Renderer[] highlightRenderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in highlightRenderers)
            {
                renderer.material = tileHover.transparentMaterial; // Apply transparent material
            }
        }
    }

    

    private void AdjustUIElementPositions()
    {
        RectTransform infoCardRect = InfoCard.GetComponent<RectTransform>();
        if (infoCardRect == null) return;

        // Calculate the width of the info card
        float infoCardWidth = infoCardRect.rect.width + 10;

        // Adjust positions for each UI element
        AdjustUIElementPosition(moneySpeedText, infoCardWidth);
    }

    private void AdjustUIElementPosition(GameObject uiElement, float offset)
    {
        if (uiElement != null)
        {
            RectTransform elementRect = uiElement.GetComponent<RectTransform>();
            if (elementRect != null)
            {
                // Save the current position
                Vector2 originalPosition = elementRect.anchoredPosition;

                // Add the info card's width to push it to the right
                elementRect.anchoredPosition = new Vector2(originalPosition.x + offset, originalPosition.y);
            }
        }
    }


    private void DeleteSelectedObject()
    {
        if (selectedObject != null)
        {
            // retrieve item cost
            float cost = 0f;
            ConveyorBelt conveyorBelt = selectedObject.GetComponent<ConveyorBelt>();
            OreDrill oreDrill = selectedObject.GetComponent<OreDrill>();
            OreWasher oreWasher = selectedObject.GetComponent<OreWasher>();

            if (conveyorBelt != null)
            {
                cost = conveyorBelt.GetCost();
            }
            if (oreDrill != null)
            {
                cost = oreDrill.GetCost();
            }
            if (oreWasher != null)
            {
                cost = oreWasher.GetCost();
                // Remove all items inside the washer
                foreach (var item in oreWasher.itemsOnWasher)
                {
                    Destroy(item); // Destroy all items currently in the washer
                }
                oreWasher.itemsOnWasher.Clear(); // Clear the washer's list
            }

            // add money back
            statsManager.AddMoney(cost * 0.75f);
            statsManager.UpdateMoneyText();


            // Get grid position of the object
            Vector3Int gridPosition = tileHover.tilemap.WorldToCell(selectedObject.transform.position);
            gridPosition.z = 0;

            // Remove the grid position from occupiedGridPositions
            if (tileHover.occupiedGridPositions.Contains(gridPosition))
            {
                tileHover.occupiedGridPositions.Remove(gridPosition);
                Debug.Log($"removing object at grid position {gridPosition}");
            }
            else
            {
                tileHover.PrintOccupiedGridPositions();
                Debug.LogWarning($"Grid position {gridPosition} not found in occupiedGridPositions.");
            }

            // Destroy the object
            Destroy(selectedObject);

            audioSource.PlayOneShot(sellSound);


            // Hide the info card after deletion
            HideInfoCard();
        }
        else
        {
            Debug.Log("No object selected to delete.");
        }
    }

    private void HideInfoCard()
    {
        if (InfoCard.activeSelf)
        {
            InfoCard.SetActive(false);

            // Restore UI positions
            RestoreOriginalPositions();

            isInfoCardOpen = false;
            selectedObject = null;
        }
    }

    private void RotateSelectedObject(float angle)
    {
        if (selectedObject != null)
        {
            // Rotate the object around its Y-axis
            selectedObject.transform.Rotate(0f, angle, 0f, Space.Self);
            Debug.Log($"Rotated {selectedObject.name} by {angle} degrees.");
        }
        else
        {
            Debug.LogWarning("No object selected to rotate.");
        }
    }


    private void RemoveHighlight(GameObject obj)
    {
        if (obj != null && originalMaterials.ContainsKey(obj))
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            Material[] originalMats = originalMaterials[obj];

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = originalMats[i]; // Restore original material
            }

            originalMaterials.Remove(obj); // Remove from dictionary
        }
    }

    private void RestoreOriginalPositions()
    {
        RestoreOriginalPosition(moneySpeedText);
    }

    private void RestoreOriginalPosition(GameObject uiElement)
    {
        if (uiElement != null && originalPositions.ContainsKey(uiElement))
        {
            RectTransform rect = uiElement.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = originalPositions[uiElement];
            }
        }
    }

    public bool IsInfoCardOpen()
    {
        return isInfoCardOpen;
    }

}
