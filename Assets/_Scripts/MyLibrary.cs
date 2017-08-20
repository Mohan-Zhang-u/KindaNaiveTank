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



		




}
