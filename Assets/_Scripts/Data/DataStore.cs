using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/*
/// <summary>
/// Decoration data serializable class
/// </summary>
[Serializable]
public class DecorationData
{
    public bool unlocked;
    public List<int> availableColours;
    public int selectedColourIndex;

    public DecorationData()
    {
        availableColours = new List<int>();
    }
}
*/

[Serializable]
public class LevelData
{
    public string Name;
    public bool Unlocked;
    public int MaxScore;
}

[Serializable]
public class SinglePlayerLevelData : LevelData
{
    public SinglePlayerLevelData(string name)
    {
        this.Name = name;
        this.Unlocked = false;
        this.MaxScore = 0;
    }
}

[Serializable]
public class MultiPlayerLevelData : LevelData
{
    public int Maxkill;
    public int PlayedTimes;
    public int WinTimes;
    public MultiPlayerLevelData(string name)
    {
        this.Name = name;
        this.Unlocked = false;
        this.MaxScore = 0;
        this.Maxkill = 0;
        PlayedTimes = 0;
        WinTimes = 0;
    }
}

[Serializable]
public class SettingsData
{
    public float musicVolume = 0.0f;
    public float sfxVolume = 0.0f;
    public float masterVolume = 0.0f;
    public float thumbstickSize = 0.6f;
}

[Serializable]
public class DataStore : ISerializationCallbackReceiver
{

    private static readonly string s_DefaultName = "Player";

    public int selectedTank;
    //public int selectedDecoration;
    public int currency;
    public bool[] unlockedTanks;
    public string playerName;

    public Dictionary<string, SinglePlayerLevelData> SinglePlayerLevelsDictionary;
    public Dictionary<string, MultiPlayerLevelData> MultiPlayerLevelsDictionary;
    public List<SinglePlayerLevelData> SinglePlayerlevels;
    public List<MultiPlayerLevelData> MultiPlayerlevels;
    public SettingsData settingsData;

    // 为什么这些东西要init在这里而不是Libraryfile里面呢?因为这些东西是开关游戏不变的,所以需要存在一个serialize的文件里. 而且, 不需要SceneLibrary是因为已经有了built-in
    // SceneManager.
    public DataStore()
    {
        /*
        // Init decoration size
        TankDecorationLibrary decorationLib = TankDecorationLibrary.s_Instance;

        if (decorationLib != null)
        {
            int numDecorations = decorationLib.GetNumberOfDefinitions();
            decorations = new DecorationData[numDecorations];
            for (int i = 0; i < numDecorations; ++i)
            {
                decorations[i] = new DecorationData();
            }
        }
        else
        {
            Debug.LogError("No decoration library. Failed to init decoration size");
        }
        */

        // Init unlocked tanks
        TankLibrary tankLib = TankLibrary.s_Instance;
        if (tankLib != null)
        {
            unlockedTanks = new bool[100];
        }
        else
        {
            Debug.LogError("No tank library. Failed to init unlocked tanks");
        }

        SinglePlayerLevelsDictionary = new Dictionary<string, SinglePlayerLevelData>();
        MultiPlayerLevelsDictionary = new Dictionary<string, MultiPlayerLevelData>();
        SinglePlayerlevels = new List<SinglePlayerLevelData>();
        MultiPlayerlevels = new List<MultiPlayerLevelData>();
        settingsData = new SettingsData();
        playerName = s_DefaultName;
    }

    /// <summary>
    /// Gets the singleplayer level data.
    /// </summary>
    /// <returns>The level data.</returns>
    /// <param name="name">Identifier.</param>
    public LevelData GetSPLevelData(string name)
    {
        SinglePlayerLevelData result;
        if (!SinglePlayerLevelsDictionary.TryGetValue(name, out result))
        {
            SinglePlayerLevelData newLevelData = new SinglePlayerLevelData(name);
            SinglePlayerLevelsDictionary.Add(name, newLevelData);
            return newLevelData;
        }

        return result;
    }

    /// <summary>
    /// Gets the MultiPlayer level data.
    /// </summary>
    /// <returns>The level data.</returns>
    /// <param name="name">Identifier.</param>
    public LevelData GetMPLevelData(string name)
    {
        MultiPlayerLevelData result;
        if (!MultiPlayerLevelsDictionary.TryGetValue(name, out result))
        {
            MultiPlayerLevelData newLevelData = new MultiPlayerLevelData(name);
            MultiPlayerLevelsDictionary.Add(name, newLevelData);
            return newLevelData;
        }

        return result;
    }

    /// <summary>
    /// Gets all SP level data.
    /// </summary>
    /// <returns>The all level data.</returns>
    public List<SinglePlayerLevelData> GetAllSPLevelData()
    {
        return SinglePlayerLevelsDictionary.Values.ToList();
    }

    /// <summary>
    /// Gets all MP level data.
    /// </summary>
    /// <returns>The all level data.</returns>
    public List<MultiPlayerLevelData> GetAllMPLevelData()
    {
        return MultiPlayerLevelsDictionary.Values.ToList();
    }

    /// <summary>
    /// Serialization implementation from ISerializationCallbackReceiver
    /// </summary>
    public void OnBeforeSerialize()
    {
        LevelDataSerialize();
    }

    /// <summary>
    /// Deserialization implementation from ISerializationCallbackReceiver
    /// </summary>
    public void OnAfterDeserialize()
    {
        LevelDataDeserialize();
    }

    /// <summary>
    /// Converts dictionary to list by getting the values for serialization
    /// </summary>
    private void LevelDataSerialize()
    {
        SinglePlayerlevels = SinglePlayerLevelsDictionary.Values.ToList();
        MultiPlayerlevels = MultiPlayerLevelsDictionary.Values.ToList();
    }

    /// <summary>
    /// Converts list to dictionary on deserialization for optimal accessing
    /// </summary>
    private void LevelDataDeserialize()
    {
        SinglePlayerLevelsDictionary = SinglePlayerlevels.ToDictionary(l => l.Name);
        MultiPlayerLevelsDictionary = MultiPlayerlevels.ToDictionary(l => l.Name);
        SinglePlayerlevels.Clear();
        MultiPlayerlevels.Clear();
    }
}
