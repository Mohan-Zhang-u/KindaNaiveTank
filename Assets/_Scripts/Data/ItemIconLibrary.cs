using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public struct ItemsIconDefinition
{
    public string name;
    public Image DisplayImg;
}

public class ItemIconLibrary : PersistentSingleton<ItemIconLibrary>
{
    [SerializeField]
    private ItemsIconDefinition[] IconDefinitions;

    protected override void Awake()
    {
        base.Awake();

        if (IconDefinitions.Length == 0)
        {
            Debug.Log("<color=red>WARNING: No icons have been defined in the Icon Library!</color>");
        }
    }

    //Returns the IconTypeDefinition for a given array index. Provides a helpful error if an invalid index is used.
    public Image GetIconForIndex(int index)
    {
        if ((index < 0) || ((index + 1) > IconDefinitions.Length))
        {
            Debug.Log("<color=red>WARNING: Requested icon data index exceeds definition array bounds.</color>");
        }

        return IconDefinitions[index].DisplayImg;
    }

    public Image GetIconDataForName(string iconName)
    {
        foreach (ItemsIconDefinition icon in IconDefinitions)
        {
            if (icon.name == iconName)
            {
                return icon.DisplayImg;
            }
        }

        Debug.Log("<color=red>WARNING: Requested icon name does not exist.</color>");
        return IconDefinitions[0].DisplayImg;

    }

    //Returns how many icon definitions we have in our library array.
    public int GetNumberOfDefinitions()
    {
        return IconDefinitions.Length;
    }
}
