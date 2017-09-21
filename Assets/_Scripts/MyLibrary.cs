using UnityEngine;
using UnityEngine.UI;
using System;

public class MyLibrary {

	/* this is my personal library of general functions that can be used in different projects. */


	/* try to find a transfrom that is a dscendant of parent. */
	public static Transform FindInDescendantByName(Transform parent, string name, int layer=0){
		if (parent.name == name)
			return parent;
		layer += 1;
		foreach (Transform child in parent){
			Transform r = FindInDescendantByName (child, name, layer);
			if (r != null)
				return r;
		}
		if (layer == 0)
			Debug.Log("<color=red>Error: </color> Descendant not found");
		return null;
	}

    // check whether the certain bit value ( layer in range [0,31]) is in a layermask.value using bitwise
    public static bool LayerInLayerMask(int layer, LayerMask mask)
    {
        if ((mask.value & (1 << layer)) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }







}
