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

    //// TODO: Reference to the primary audiomixer for the game
    //[SerializeField]
    //protected AudioMixer m_AudioMixer;

    #region Properties

    //The in-game currency that the player has accrued.
    public int currency
    {
        get
        {
            return m_Data.currency;
        }
    }

    //public int Exp
    //{
    //    get
    //    {
    //        return (int) m_Data.Exp;
    //    }
    //}

    //The array index of the last tank selected by the player. Corresponds to the definition index in the TankLibrary.
    public int selectedTank
    {
        get { return m_Data.selectedTank; }
        set { m_Data.selectedTank = value; }
    }

    ////The array index of the last decoration selected by the player. Corresponds to the definition index in the TankDecorationLibrary.
    //public int selectedDecoration
    //{
    //    get { return m_Data.selectedDecoration; }
    //    set { m_Data.selectedDecoration = value; }
    //}

    //The player's chosen name.
    public string playerName
    {
        get { return m_Data.playerName; }
        set { m_Data.playerName = value; }
    }

    //The master audio volume level for the game.
    public float masterVolume
    {
        get { return m_Data.settingsData.masterVolume; }
        set { m_Data.settingsData.masterVolume = value; }
    }

    //The music volume level for the game.
    public float musicVolume
    {
        get { return m_Data.settingsData.musicVolume; }
        set { m_Data.settingsData.musicVolume = value; }
    }

    //The sfx volume level for the game.
    public float sfxVolume
    {
        get { return m_Data.settingsData.sfxVolume; }
        set { m_Data.settingsData.sfxVolume = value; }
    }

    //The chosen on-screen thumbstick size for mobile platforms.
    public float thumbstickSize
    {
        get { return m_Data.settingsData.thumbstickSize; }
        set { m_Data.settingsData.thumbstickSize = value; }
    }

    ////Whether the user has chosen to flip the thumbstick from bottom right to bottom left.
    //public bool isLeftyMode
    //{
    //    get { return m_Data.settingsData.isLeftyMode; }
    //    set { m_Data.settingsData.isLeftyMode = value; }
    //}

    ////Whether Everyplay will be active to record multiplayer games.
    //public bool everyplayEnabled
    //{
    //    get { return m_Data.settingsData.everyplayEnabled; }
    //    set { m_Data.settingsData.everyplayEnabled = value; }
    //}

    //Volatile storage for tracking the last SP level the player selected over the app lifetime. NOT TO BE SAVED.
    private int m_LastLevelSelected = -1;

    public int lastLevelSelected
    {
        get { return m_LastLevelSelected; }
        set { m_LastLevelSelected = value; }
    }

    #endregion

    //Event fired whenever currency is altered.
    public event Action<int> onCurrencyChanged;

    //protected override void Awake()
    //{
        
    //}

    private void Start()
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

        /*
        //TODO:!!!!!!!!!!!!!!!!!!!!!We always want the first tank and the first decoration (a null decoration) to be unlocked by default, and init accordingly.
        if (m_Data.decorations.Length > 0)
        {
            m_Data.decorations[0].unlocked = true;
        }
        */
        if (m_Data.unlockedTanks.Length > 0)
        {
            m_Data.unlockedTanks[0] = true;
        }


        m_Saver.Load(m_Data);
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


    //Returns whether a tank for a given index is unlocked.
    public bool IsTankUnlocked(int index)
    {
        if (TankLibrary.s_Instance.GetNumberOfDefinitions() > index && index >= 0)
        {
            return m_Data.unlockedTanks[index];
        }

        return false;
    }


    //Allows a tank index's unlocked status to be set. Defaults to unlocking the tank.
    public void SetTankUnlocked(int index, bool setUnlocked = true)
    {
        if (TankLibrary.s_Instance.GetNumberOfDefinitions() > index && index >= 0)
        {
            m_Data.unlockedTanks[index] = setUnlocked;
        }
        else
        {
            throw new IndexOutOfRangeException("Tank index invalid");
        }

        SaveData();
    }

    public LevelData GetSPLevelData(string Name)
    {
        return m_Data.GetSPLevelData(Name);
    }

    public LevelData GetMPLevelData(string Name)
    {
        return m_Data.GetMPLevelData(Name);
    }

    //Sets whether a singleplayer Level is unlocked or not. Defaults to unlocking.
    public void SetSPLevelUnlocked(string LevelName, bool setUnlocked = true)
    {
        if (m_Data.SinglePlayerLevelsDictionary.ContainsKey(LevelName))
        {
            m_Data.SinglePlayerLevelsDictionary[LevelName].Unlocked = setUnlocked;
            SaveData();
        }
        else
        {
            Debug.Log("<color=red> There's no SPLevel with name </color>" + LevelName);
        }
    }

    //Sets whether a multiplayer Level is unlocked or not. Defaults to unlocking.
    public void SetMPLevelUnlocked(string LevelName, bool setUnlocked = true)
    {
        if(m_Data.MultiPlayerLevelsDictionary.ContainsKey(LevelName))
        {
            m_Data.MultiPlayerLevelsDictionary[LevelName].Unlocked = setUnlocked;
            SaveData();
        }
        else
        {
            Debug.Log("<color=red> There's no MPLevel with name </color>"+ LevelName);
        }
    }

    //Returns whether the SinglePlayer Level with the given Name string has been unlocked.
    public bool IsSPLevelUnlocked(string LevelId)
    {
        if (m_Data.SinglePlayerLevelsDictionary.ContainsKey(LevelId))
        {
            return m_Data.SinglePlayerLevelsDictionary[LevelId].Unlocked;
        }

        return false;
            
    }

    //Returns whether the Multiplayer Level with the given Name string has been unlocked.
    public bool IsMPLevelUnlocked(string LevelId)
    {
        if (m_Data.MultiPlayerLevelsDictionary.ContainsKey(LevelId))
        {
            return m_Data.MultiPlayerLevelsDictionary[LevelId].Unlocked;
        }

        return false;

    }


#if UNITY_EDITOR
    //Menu option to clear save game data for debug purposes.
    [MenuItem("Edit/Clear Save Data")]
    static void ClearData()
    {
        string filename = JsonSaver.GetSaveFilename();
        if (System.IO.File.Exists(filename))
        {
            System.IO.File.Delete(filename);
        }
    }
#endif
}
