using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, InventoryItem> inventory = new Dictionary<string, InventoryItem>();
    private Dictionary<string, GameObject> itemReferences = new Dictionary<string, GameObject>();

    [Header("Item Prefabs")]
    [SerializeField] private GameObject healthPotionPrefab;

    public void AddItem(string itemName, GameObject pickedUpItem)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName].quantity++;
            Debug.Log("Item Added in Inventory");
        }
        else
        {
            inventory.Add(itemName, new InventoryItem { itemName = itemName, quantity = 1 });
        }

        if (!itemReferences.ContainsKey(itemName))
        {
            itemReferences[itemName] = pickedUpItem;
        }
    }

    public void RemoveItem(string itemName)
    {
        if (inventory.ContainsKey(itemName))
        {
            if (inventory[itemName].quantity > 1)
            {
                inventory[itemName].quantity--;
            }
            else
            {
                inventory.Remove(itemName);
            }
        }
    }

    public bool HasItem(string itemName)
    {
        return inventory.ContainsKey(itemName);
    }

    public void DropItem(string itemName, Vector3 position) 
    {
        if (itemReferences.ContainsKey(itemName))
        {
            GameObject itemToDrop = itemReferences[itemName]; // Get the reference to the actual GameObject
            itemToDrop.transform.position = position; //Move it to the drop position
            itemToDrop.SetActive(true);
            RemoveItem(itemName); // Remove it from the inventory
            Debug.Log($"Dropped {itemName} at {position}");
        }
        else
        {
            Debug.LogWarning($"No reference found for item {itemName}");
        }
    }
}
