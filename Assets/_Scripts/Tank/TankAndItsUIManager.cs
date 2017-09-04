using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Complete;

public class TankAndItsUIManager : MonoBehaviour {
    private GameObject DynamicObjectLibrary;
    private ItemLibrary ItemLibraryScript;
    private Dictionary<string, Image> ItemIconDict;

    public AudioSource RidiculeSource;

    private GameObject ActiveUICanvas;
    private Button ItemFireButton1;
    private Button ItemFireButton2;
    private Button RidiculeButton;

    private int RidiculeButtonColddownCount = 3;
    private WaitForSeconds ColdDownTime;
    private bool IsColdingDown = false;

    private void OnEnable()
    {
        DynamicObjectLibrary = GameObject.Find("DynamicObjectLibrary");
        ItemLibraryScript = DynamicObjectLibrary.GetComponent<ItemLibrary>();
        ItemIconDict = ItemLibraryScript.ItemIconDict;

        ColdDownTime = new WaitForSeconds(3);

        // set Canvas
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
            RidiculeButton.onClick.AddListener(PlayRidicule);
        }
    }

    // there is a build-in colddown counter;
    public void PlayRidicule()
    {
        if (RidiculeButtonColddownCount > 0)
        {
            RidiculeButtonColddownCount -= 1;
            RidiculeSource.Play();
            if (!IsColdingDown && gameObject.activeSelf)
            {
                StartCoroutine(RidiculeButtonColddown());
            }
        }
        else
        {
            return;
        }
    }

    private IEnumerator RidiculeButtonColddown()
    {
        IsColdingDown = true;
        yield return ColdDownTime;
        RidiculeButtonColddownCount = 3;
        IsColdingDown = false;

    }

    public void OnPickUpItems(string ItemName)
    {

    }

    // TODO: implement. set UI accordingly. Usually, it switches first and second Icon, reset which to fire when press item 1 or 2.
    public void OnPickupCollected(string BoxId)
    {

    }

    private void UseItemWithName(string ItemName)
    {
        if (ItemName == "Invincible")
        {
            float invincibleSeconds = ItemLibraryScript.InvincibleTime;
            Light Invinciblelight = ItemLibraryScript.InvincibleLight;
            gameObject.GetComponent<TankHealth>().SetInvincibleForSeconds(true, invincibleSeconds);
            Light l = Object.Instantiate(Invinciblelight, gameObject.transform.position + new Vector3(0, 3f, 0), gameObject.transform.rotation, gameObject.transform);
            Destroy(l, invincibleSeconds);
        }

        SetItemIcon(ItemName);
    }

    private void SetItemIcon(string ItemName)
    {

    }
}
