using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankAndItsUIManager : MonoBehaviour {

    public AudioSource RidiculeSource;

    private GameObject ActiveUICanvas;

    private Button ItemFireButton1;
    private Button ItemFireButton2;
    private Button RidiculeButton;

    private void OnEnable()
    {
        ActiveUICanvas = GameObject.Find("ActiveUICanvas");
        // hereby set Ridicule button.
        Button[] buttons = ActiveUICanvas.GetComponentsInChildren<Button>();
        foreach (Button b in buttons)
        {
            if (b.name == "ItemFire1")
            {
                ItemFireButton1 = b;
            }
            else if (b.name == "ItemFire2")
            {
                ItemFireButton2 = b;
            }
            else if (b.name == "Ridicule")
            {
                RidiculeButton = b;
            }
        }

        if (ItemFireButton1)
        {

        }

        if (ItemFireButton2)
        {

        }

        if (RidiculeButton)
        {
            //RidiculeButton.onClick.AddListener();
        }
    }



    // TODO: implement. set UI accordingly. Usually, it switches first and second Icon, reset which to fire when press item 1 or 2.
    public void OnPickupCollected(string BoxId)
    {

    }
}
