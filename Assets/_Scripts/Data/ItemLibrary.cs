using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

//same as TankLibrary.cs TankTypeDefinition
[Serializable]
public struct ItemTypeDefinition
{
    public GameObject displayPrefab;

    public Sprite displayImage;
    //Unique ID to reference item internally
    public string id;

    //The displayed name of the item
    public string name;

    //A short blurb describing the item
    public string description;

    public bool IsExplosive;
    public bool IsObstacle;


}


public class ItemLibrary : PersistentSingleton<ItemLibrary>
{
    [HeaderAttribute("Below is for item parameters")]
    public float InvincibleTime;
    public Light InvincibleLight;


    //An array of TankTypeDefinitions. These determine which tanks are available in the game and their properties.
    [SerializeField]
    private ItemTypeDefinition[] ItemDefinitions;

    public Dictionary<string, Sprite> ItemIconDict;

    protected override void Awake()
    {
        base.Awake();
        if (ItemDefinitions.Length == 0)
        {
            Debug.Log("<color=red>WARNING: No items have been defined in the item Library!</color>");
        }
    }

    private void OnEnable()
    {
        ItemIconDict = new Dictionary<string, Sprite>();
        foreach (ItemTypeDefinition item in ItemDefinitions)
        {
            ItemIconDict.Add(item.name, item.displayImage);
        }
    }

    //Returns the ItemTypeDefinition for a given array index. Provides a helpful error if an invalid index is used.
    public ItemTypeDefinition GetItemDataForIndex(int index)
    {
        if ((index < 0) || ((index + 1) > ItemDefinitions.Length))
        {
            Debug.Log("<color=red>WARNING: Requested item data index exceeds definition array bounds.</color>");
        }

        return ItemDefinitions[index];
    }

    public ItemTypeDefinition GetItemDataForName(string itemName)
    {
        foreach (ItemTypeDefinition item in ItemDefinitions)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }

        Debug.Log("<color=red>WARNING: Requested item name does not exist.</color>");
        return ItemDefinitions[0];

    }

    public int GetNumberOfDefinitions()
    {
        return ItemDefinitions.Length;
    }

}
