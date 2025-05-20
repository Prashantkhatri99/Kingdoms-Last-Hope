using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public CollectableType type;
        public int count;
        public int maxAllowed;
        public Sprite icon;

        public Slot()
        {
            type = CollectableType.NONE;
            count = 0;
            maxAllowed = 99;
            icon = null;
        }

        public bool CanAddItem()
        {
            return count < maxAllowed;
        }

        public void AddItem(CollectableType type, Sprite icon = null)
        {
            this.type = type;
            count++;
            if (this.icon == null && icon != null)
                this.icon = icon;
        }
    }

    public List<Slot> slots = new List<Slot>();

    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    public void Add(CollectableType typeToAdd, Sprite icon = null)
    {
        foreach (Slot slot in slots)
        {
            if (slot.type == typeToAdd && slot.CanAddItem())
            {
                slot.AddItem(typeToAdd); // Icon already assigned in slot
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.type == CollectableType.NONE)
            {
                slot.AddItem(typeToAdd, icon); // Assign icon on first insert
                return;
            }
        }
    }
}

public enum CollectableType
{
    NONE,
    CARROT_SEED,
    POTATO_SEED,
    // Add other types as needed
}
