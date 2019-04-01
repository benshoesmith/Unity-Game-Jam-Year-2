using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IALootTable : MonoBehaviour
{
    [System.Serializable]
    public class DropeableItem
    {
        public int ItemID = -1;
        public int MaxAmount = 1;
        public float Rarity= 0.01f;
    }

    [SerializeField]
    private List<DropeableItem> Dropeable_items = new List<DropeableItem>();
    [SerializeField]
    private int MinItemsToDrop = 1;
    [SerializeField]
    private int MaxItemsToDrop = 1;

    public void AddItem(DropeableItem item)
    {
        if (!Dropeable_items.Contains(item))
            return;

        Dropeable_items.Add(item);
    }

    public Dictionary<int, int> GetLoot()
    {
        Dictionary<int, int> DroppedItems = new Dictionary<int, int>();
        int AmounOfItemsToDrop = Random.Range(MinItemsToDrop, MaxItemsToDrop + 1);
        for (int i = 0; i < AmounOfItemsToDrop; i++)
        {
            float roll = Random.value;
            float sumWeight = 0.0f;
            foreach (DropeableItem dItem in Dropeable_items)
            {
                sumWeight += dItem.Rarity;
                if (roll < sumWeight)
                {
                    if (DroppedItems.ContainsKey(dItem.ItemID))
                    {
                        if (DroppedItems[dItem.ItemID] >= dItem.MaxAmount)
                            continue;

                        DroppedItems[dItem.ItemID] = DroppedItems[dItem.ItemID] + 1;
                    } else
                    {
                        DroppedItems.Add(dItem.ItemID, 1);
                    }
                }
            }
        }
        return DroppedItems;
    }
}
