using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Item
{
    public ItemInfo itemInfo;

    public int Amount;

    public int Durability;

    public Item(ItemInfo ii, int amount, int durability)
    {
        itemInfo = ii;
        Amount = amount;
        Durability = durability;
    }
}

[System.Serializable]
public struct ItemInfo
{
    public string Name;

    public int ItemID;

    public int MaxStackAmount;

    public bool UseDurability;
    public int MaxDurability;

    public ItemInfo(string name, int id, int maxstack, bool usedurability, int maxdurability)
    {
        Name = name;
        ItemID = id;
        MaxStackAmount = maxstack;
        UseDurability = usedurability;
        MaxDurability = maxdurability;
    }
}

public class Inventory : MonoBehaviour
{
    public List<ItemInfo> ItemsInfo = new List<ItemInfo>() 
    { 
        new ItemInfo("Air", 0, 256, false, 0), 
        new ItemInfo("Stone", 1, 256, false, 0), 
        new ItemInfo("Iron", 2, 256, false, 0), 
        new ItemInfo("Gold", 3, 256, false, 0) 
    };


    public List<Item> ItemsInInventory = new List<Item>();


    public void AddToInventory(int ItemId, int amount = 1)
    {
        int itemindex = ItemsInInventory.FindIndex(x => x.itemInfo.ItemID == ItemId);
        if (itemindex >= 0)
        {
            Item i = ItemsInInventory[itemindex];
            i.Amount += amount;

            ItemsInInventory[itemindex] = i;
        }
        else
        {
            ItemInfo iteminfo = GetItemInfo(ItemId);

            ItemsInInventory.Add(new Item(iteminfo, 1, iteminfo.MaxDurability));
        }
    }

    public ItemInfo GetItemInfo(int itemid)
    {
        return ItemsInfo.Find(x => x.ItemID == itemid);
    }
}