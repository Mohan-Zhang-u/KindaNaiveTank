using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    //References to the datastore object, and the saver object responsible for serializing the datastore to file.
    [NonSerialized]
    private DataStore m_Data;
    [NonSerialized]
    private IDataSaver m_Saver;


    //Event fired whenever currency is altered.
    public event Action<int> onCurrencyChanged;

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();

        // In the Unity editor, use a plain text saver so files can be inspected.
#if UNITY_EDITOR || NETFX_CORE
        m_Saver = new JsonSaver();
#else
			m_Saver = new EncryptedJsonSaver();
#endif
        m_Data = new DataStore();


        //TODO:!!!!!!!!!!!!!!!!!!!!!We always want the first tank and the first decoration (a null decoration) to be unlocked by default, and init accordingly.
        if (m_Data.decorations.Length > 0)
        {
            m_Data.decorations[0].unlocked = true;
        }
        if (m_Data.unlockedTanks.Length > 0)
        {
            m_Data.unlockedTanks[0] = true;
        }

        m_Saver.Load(m_Data);
    }

    private void Start()
    {
        //!!!!!!TODO: audioMixer.. Set our saved audio settings
        //if (m_AudioMixer != null)
        //{
        //    m_AudioMixer.SetFloat("MusicVolume", musicVolume);
        //    m_AudioMixer.SetFloat("SFXVolume", sfxVolume);
        //    m_AudioMixer.SetFloat("MasterVolume", masterVolume);
        //}
        //else
        //{
        //    Debug.LogError("Missing AudioMixer");
        //}
    }

    protected override void OnDestroy()
    {
        //We save on exit
        if (s_Instance == this)
        {
            SaveData();
        }

        base.OnDestroy();
    }

    //use the specified saver to serialize our game data.
    public void SaveData()
    {
        m_Saver.Save(m_Data);
    }


    //Returns whether the player has enough currency for a given cost
    public bool CanPlayerAffordPurchase(int cost)
    {
        return m_Data.currency >= cost;
    }

    //Adds currency to the player's currency pool
    public void AddCurrency(int currencyToAdd)
    {
        if (currencyToAdd >= 0)
        {
            m_Data.currency += currencyToAdd;
            SaveData();

            if (onCurrencyChanged != null)
            {
                onCurrencyChanged(m_Data.currency);
            }
        }
        else
        {
            Debug.Log("<color=red>Attempting to add negative currency. Please use RemoveCurrency for this.</color>");
        }
    }


    //Removes currency from the player's currency pool, clamping to zero.
    public void RemoveCurrency(int currencyToRemove)
    {
        if (currencyToRemove >= 0)
        {
            m_Data.currency -= currencyToRemove;
            if (m_Data.currency < 0)
            {
                m_Data.currency = 0;
            }

            if (onCurrencyChanged != null)
            {
                onCurrencyChanged(m_Data.currency);
            }

            SaveData();
        }
        else
        {
            Debug.Log("<color=red>Attempting to remove negative currency. Please use AddCurrency for this.</color>");
        }
    }
}
