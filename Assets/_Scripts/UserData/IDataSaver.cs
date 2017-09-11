using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for saving data
/// </summary>
public interface IDataSaver
{
    void Save(DataStore data);

    bool Load(DataStore data);

    void Delete();
}
