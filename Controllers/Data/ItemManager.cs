using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemManager : MonoBehaviour
{
    public Transform gridParent; // The parent of the grid items (a GridLayoutGroup)
    public GameObject gridItemPrefab;     // The prefab for the grid items
    public GameObject reportsIcon; 
    public GameObject detailViewPrefab;
    public CSVItemLoader csvLoader;

    private List<Item> items;
    private Item currentItem;
    private int curDay;

    void Start()
    {
        items = csvLoader.LoadItems();
        reportsIcon.SetActive(false);
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        curDay = levelManager.currentDay;

          detailViewPrefab.SetActive(false);

        // Sort items by itemDay
        items.Sort((x, y) => x.itemDay.CompareTo(y.itemDay));

        PopulateInventory();
    }

    void PopulateInventory()
    {
        // Clear existing inventory items
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // Add items available up to the current day
        foreach (Item item in items)
        {
            if (item.itemDay <= curDay)
            {
                GameObject gridItem = Instantiate(gridItemPrefab, gridParent);
                gridItem.transform.Find("ItemImage").GetComponent<Image>().sprite = item.itemImage;

                Button itemButton = gridItem.GetComponent<Button>();
                if (itemButton == null)
                {
                    Debug.LogError("Button component not found in item grid prefab.");
                    continue;
                }

                itemButton.onClick.AddListener(() => ShowItemDetails(item));
            }
        }
    }

    // Show details of the selected item
    public void ShowItemDetails(Item item)
    {
        currentItem = item;
        detailViewPrefab.SetActive(true);

        detailViewPrefab.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
        detailViewPrefab.transform.Find("ItemImage").GetComponent<Image>().sprite = item.itemImage;
        detailViewPrefab.transform.Find("ItemDescription").GetComponent<TMP_Text>().text = item.itemText;

        Button equipButton = detailViewPrefab.transform.Find("Panel/Equip").GetComponent<Button>();
        equipButton.onClick.RemoveAllListeners();
        equipButton.gameObject.SetActive(!string.IsNullOrEmpty(item.itemEvent));

        if (!string.IsNullOrEmpty(item.itemEvent))
        {
            equipButton.onClick.AddListener(() => EquipItem(item));
        }
    }

    // write this function out
    private void EquipItem(Item item)
    {
        // Handle item-specific events
        switch (item.itemEvent)
        {
            case "KeyEvent":
                EnableSpriteWithTag("KeyTag");
                break;

            case "USBEvent":
                EnableSpriteWithTag("USBTag");
                ActivatePanelsWithTag();
                break;

            default:
                Debug.LogWarning("Unrecognized item event: " + item.itemEvent);
                return;
        }

        // Remove the item from the list
        items.Remove(item);

        // Update the inventory UI
        PopulateInventory();
    }

    private void EnableSpriteWithTag(string tag)
{
    // Find the GameObject in the scene by tag
    GameObject spriteObject = GameObject.FindWithTag(tag);
    if (spriteObject != null)
    {
        // Find and enable the SpriteRenderer component
        SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true; // Enable the SpriteRenderer component
        }
        else
        {
            Debug.LogWarning($"No SpriteRenderer component found on GameObject with tag {tag}.");
        }

        // Find and enable the Collider2D component
        Collider2D collider2D = spriteObject.GetComponent<Collider2D>();
        if (collider2D != null)
        {
            collider2D.enabled = true; // Enable the Collider2D component
        }
        else
        {
            Debug.LogWarning($"No Collider2D component found on GameObject with tag {tag}.");
        }

        // Optionally set the GameObject active if it was deactivated
        // This line is generally not needed unless the entire GameObject was deactivated.
        spriteObject.SetActive(true); // Ensure the GameObject itself is active
    }
    else
    {
        Debug.LogError($"GameObject with tag {tag} not found in the scene.");
    }
}


    private void ActivatePanelsWithTag()
    {
      
                reportsIcon.SetActive(true);
        
    }


    // Method to update the current day number
    public void UpdateDayNumber(int newDayNumber)
    {
        curDay = newDayNumber;
        PopulateInventory(); // Refresh the inventory based on the new day
        Debug.Log("Inventory updated for day: " + curDay);
    }
}

