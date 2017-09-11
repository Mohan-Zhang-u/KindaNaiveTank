using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public int selectedDecoration;
    public int currency;
    public bool[] unlockedTanks;

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
            unlockedTanks = new bool[tankLib.GetNumberOfDefinitions()];
        }
        else
        {
            Debug.LogError("No tank library. Failed to init unlocked tanks");
        }
    }



}
