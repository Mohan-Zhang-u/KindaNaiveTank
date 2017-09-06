using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Complete;

public class TankAndItsUIManager : MonoBehaviour {
    [HideInInspector]
    public int _PlayerNumber = 1;
    private GameObject DynamicObjectLibrary;
    private ItemLibrary ItemLibraryScript;
    private Dictionary<string, Sprite> ItemIconDict;

    public AudioSource RidiculeSource;

    private GameObject ActiveUICanvas;
    private Button ItemFireButton1;
    private Button ItemFireButton2;
    private Button RidiculeButton;
    private Image FirstItemImg;
    private Image SecondItemImg;
    private VirtualJoyStickScript JoystickScript;

    private int RidiculeButtonColddownCount = 3;
    private WaitForSeconds ColdDownTime;
    private bool IsColdingDown = false;

    private string ItemName1 = "";
    private string ItemName2 = "";

    private void OnEnable()
    {
        DynamicObjectLibrary = GameObject.Find("DynamicObjectLibrary");
        ItemLibraryScript = DynamicObjectLibrary.GetComponent<ItemLibrary>();
        ItemIconDict = ItemLibraryScript.ItemIconDict;

        ColdDownTime = new WaitForSeconds(3);

        // set Canvas
        ActiveUICanvas = GameObject.Find("ActiveUICanvas");
        JoystickScript = ActiveUICanvas.GetComponentInChildren<VirtualJoyStickScript>();
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
            ItemFireButton1.onClick.AddListener(UseItemNum1);
        }

        if (ItemFireButton2)
        {
            ItemFireButton2.onClick.AddListener(UseItemNum2);
        }

        if (RidiculeButton)
        {
            RidiculeButton.onClick.AddListener(PlayRidicule);
        }

        Image[] images = ActiveUICanvas.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            if (img.name == "FirstItemImg")
            {
                FirstItemImg = img;
            }
            else if (img.name == "SecondItemImg")
            {
                SecondItemImg = img;
            }
        }
        if (FirstItemImg)
        {

        }
        if (SecondItemImg)
        {

        }

        SetItemsIconDisplay();
    }

    private void SetItemsIconDisplay()
    {
        if(ItemName2 == "")
        {
            SecondItemImg.sprite = null;
            if (ItemName1 == "")
            {
                FirstItemImg.sprite = null;
            }
            else
            {
                FirstItemImg.sprite = ItemIconDict[ItemName1];
                if (!FirstItemImg.sprite)
                {
                    Debug.Log("<color=red>Missing Item Icon! </color>");
                }
            }
        }
        else
        {
            if (ItemName1 == "")
            {
                ItemName1 = ItemName2;
                ItemName2 = "";
                FirstItemImg.sprite = ItemIconDict[ItemName1];
                if (!FirstItemImg.sprite)
                {
                    Debug.Log("<color=red>Missing Item Icon! </color>");
                }
                SecondItemImg.sprite = null;
            }
            else
            {
                FirstItemImg.sprite = ItemIconDict[ItemName1];
                if (!FirstItemImg.sprite)
                {
                    Debug.Log("<color=red>Missing Item Icon! </color>");
                }
                SecondItemImg.sprite = ItemIconDict[ItemName2];
                if (!SecondItemImg.sprite)
                {
                    Debug.Log("<color=red>Missing Item Icon! </color>");
                }

            }
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
        if (ItemName1 == "")
        {
            ItemName1 = ItemName;
            SetItemsIconDisplay();
            return;
        }
        else if (ItemName2 == "")
        {
            ItemName2 = ItemName;
            SetItemsIconDisplay();
            return;
        }
        else
        {
            ItemName1 = ItemName2;
            ItemName2 = ItemName;
            SetItemsIconDisplay();
            return;
        }
    }

    // TODO: implement. set UI accordingly. Usually, it switches first and second Icon, reset which to fire when press item 1 or 2.
    public void OnPickupCollected(string BoxId)
    {

    }

    private void UseItemNum1()
    {
        UseItemWithName(ItemName1);
        ItemName1 = "";
        SetItemsIconDisplay();
        
    }

    private void UseItemNum2()
    {
        UseItemWithName(ItemName2);
        ItemName2 = "";
        SetItemsIconDisplay();
    }

    // !!!!!pay attention to DeployByTankId = _PlayerNumber!!!!!!!
    private void UseItemWithName(string ItemName)
    {
        if (ItemName == "")
        {
            return;
        }

        if (ItemName == "Invincible")
        {
            float invincibleSeconds = ItemLibraryScript.InvincibleTime;
            Light Invinciblelight = ItemLibraryScript.InvincibleLight;
            if(gameObject.GetComponent<TankHealth>().SetInvincibleForSeconds(true, invincibleSeconds))
            {
                Light l = Object.Instantiate(Invinciblelight, gameObject.transform.position + new Vector3(0, 3f, 0), gameObject.transform.rotation, gameObject.transform);
                Destroy(l, invincibleSeconds);
            }
        }
        else if (ItemName == "Trap")
        {
            GameObject TrapItem = Object.Instantiate(ItemLibraryScript.GetItemDataForName(ItemName).displayPrefab, gameObject.transform.position + new Vector3(0, -1f, 0) - 5 * gameObject.transform.forward, gameObject.transform.rotation);
            TrapItem.GetComponent<TrapHandler>().DeployByTankId = _PlayerNumber;
        }
        else if (ItemName == "TimeBomb")
        {
            GameObject TimeBombItem = Instantiate(ItemLibraryScript.GetItemDataForName(ItemName).displayPrefab, gameObject.transform.position - 5 * gameObject.transform.forward, gameObject.transform.rotation);
            TimeBombItem.GetComponent<TimeBombHandler>().DeployByTankId = _PlayerNumber;
        }
        else if (ItemName == "FirePath")
        {
            GameObject FirePathItem = Instantiate(ItemLibraryScript.GetItemDataForName(ItemName).displayPrefab, gameObject.transform.position - 2 * gameObject.transform.forward, gameObject.transform.rotation,gameObject.transform);
            FirePathItem.GetComponent<FlameThrowerHandler>().DeployByTankId = _PlayerNumber;
        }
        else if (ItemName == "FirstAidKit")
        {
            GameObject FirstAidKitParticleSystem = Object.Instantiate(ItemLibraryScript.GetItemDataForName(ItemName).displayPrefab, gameObject.transform.position, gameObject.transform.rotation);
            StartCoroutine(FirstAidKitHeal(FirstAidKitParticleSystem));
        }
        else if (ItemName == "FireThrower")
        {
            GameObject FireThrowerItem = Object.Instantiate(ItemLibraryScript.GetItemDataForName(ItemName).displayPrefab, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
            
        }

    }

    private IEnumerator FirstAidKitHeal(GameObject FirstAidKitParticleSystem)
    {
        float t=0;
        while (t<2.5f && JoystickScript.JoyStickInputVectors.magnitude < 0.05f)
        {
            t += Time.deltaTime;
            gameObject.GetComponent<TankHealth>().AddHealth(15f * Time.deltaTime, _PlayerNumber); // in total 15*2.5 hp added
            yield return null;
        }
        Destroy(FirstAidKitParticleSystem);
    }

}
