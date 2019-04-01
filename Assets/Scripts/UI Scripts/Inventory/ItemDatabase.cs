using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemDatabase : MonoBehaviour {

     private Dictionary<int, ItemObject> itemDatabase = new Dictionary<int, ItemObject>();

    //Make sure the ItemDatabase never gets Destroyed
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        //Load the data
        this.LoadItemData();
    }

    private void LoadItemData()
    {
        //First clear the database
        itemDatabase.Clear();
        //Load inventory
        //Get the database file
        TextAsset itemDatabaseTextAsset = Resources.Load<TextAsset>("itemsDatabase");
        //Create an array of Items
        ItemObject[] allItems = JsonHelperClass.getJsonArray<ItemObject>(itemDatabaseTextAsset.text);
        //Add each item to the dictionary
        foreach (ItemObject item in allItems)
        {
            //Check if the item ID already exits
            if (itemDatabase.ContainsKey(item.ID))
            {
                //already exits so dont add it.
                string descartedItem = JsonUtility.ToJson(item);
                Debug.Log("Item with ID " + item.ID + " already exits in the database descarting item: " + descartedItem);
                continue;
            }
            //Just in case if the item is null
            if (item != null)
            {
                //Build the item as the FromJson does not call the constructor and add it to the database.
                item.BuildItem();
                itemDatabase.Add(item.ID, item);
            }
        }
    }

    public ItemObject getItem(int id)
    {
        ItemObject returnValue;
        //Check if the item exits in the database, if so return null
        if (!itemDatabase.TryGetValue(id, out returnValue))
        {
            returnValue = null;
        }
        return returnValue;
    }
}
