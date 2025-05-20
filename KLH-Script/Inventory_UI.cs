using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public PlayerController player;

    public List<Slot_UI> slots = new List<Slot_UI>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            Setup();
        }
        else
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Setup()
{
    if (slots.Count == player.inventory.slots.Count)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var invSlot = player.inventory.slots[i];
            if (invSlot.type != CollectableType.NONE)
            {
                Debug.Log($"Setting slot {i} with item {invSlot.type}, count: {invSlot.count}");
                slots[i].SetItem(invSlot);
            }
            else
            {
                Debug.Log($"Clearing slot {i}");
                slots[i].SetEmpty();
            }
        }
    }
    else
    {
        Debug.LogWarning($"Slots count mismatch: UI slots = {slots.Count}, inventory slots = {player.inventory.slots.Count}");
    }
}

    }

