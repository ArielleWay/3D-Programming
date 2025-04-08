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

    public void AddItem(string itemName)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName].quantity++;
        }
        else
        {
            inventory.Add(itemName, new InventoryItem { itemName = itemName, quantity = 1 });
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
}
