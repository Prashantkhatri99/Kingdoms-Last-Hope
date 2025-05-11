using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public CollectableType type;
        public int count;
        public int maxAllowed;
        public Sprite icon;  // ✅ Added the icon field to represent the item's icon.

        public Slot()
        {
            type = CollectableType.NONE;
            count = 0;
            maxAllowed = 99;
            icon = null;  // You can set a default icon here or when items are added to the inventory.
        }

        public bool CanAddItem()
        {
            return count < maxAllowed;
        }

        public void AddItem(CollectableType type)
        {
            this.type = type;
            count++;
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

    public void Add(CollectableType typeToAdd)
    {
        foreach (Slot slot in slots)
        {
            if (slot.type == typeToAdd && slot.CanAddItem())
            {
                slot.AddItem(typeToAdd);
                return;
            }
        }
        foreach (Slot slot in slots)
        {
            if (slot.type == CollectableType.NONE)
            {
                slot.AddItem(typeToAdd);
                return;
            }
        }
    }
}

public enum CollectableType
{
    NONE,
    CARROT_SEED
}
