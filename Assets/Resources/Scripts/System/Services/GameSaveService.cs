using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data.User;
using Newtonsoft.Json;
using UnityEngine;

[AutoRegister(typeof(ISaveService), priority: 10)]
public class GameSaveService : ISaveService
{
    private Userdata _currentData = null;
    private int _currentSlot = -1;
    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
    private readonly IPathProvider _pathProvider;
    private const int MAX_SLOT_COUNT = 100;

    public GameSaveService(IPathProvider pathProvider)
    {
        _pathProvider = pathProvider;
    }

    public  Userdata CurrentData { get { return _currentData; } }

    public bool CheckValidSlotIndex(int slotId)
    {
        return slotId >= 0 && slotId < MAX_SLOT_COUNT;
    }

    public bool IsSlotUsed(int slotId)
    {
        if (CheckValidSlotIndex(slotId) == false)
        {
            return false;
        }

        if (Directory.Exists(_pathProvider.GetSaveDirPath(slotId)) && File.Exists(_pathProvider.GetSavePath(slotId)))
        {
            return true;
        }

        return false;
    }

    public bool Save(int slotId, Userdata userData = null)
    {
        if (CheckValidSlotIndex(slotId) == false)
        {
            return false;
        }

        string dirPath = _pathProvider.GetSaveDirPath(slotId);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        try
        {
            Userdata saveData = userData ?? CurrentData;
            if (saveData == null)
            {
                Debug.Log("Data is Null");
                return false;
            }

            saveData.lastSavedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, _settings);
            string path = _pathProvider.GetSavePath(slotId);

            File.WriteAllText(path, json);
            _currentData = saveData;
            _currentSlot = slotId;

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());

            return false;
        }
    }

    public bool Save()
        => Save(_currentSlot, _currentData);

    public Userdata Load(int slotId)
    {
        if (CheckValidSlotIndex(slotId) == false)
        {
            return null;
        }

        try
        {
            string path = _pathProvider.GetSavePath(slotId);
            Userdata loadedData = null;

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                loadedData = JsonConvert.DeserializeObject<Userdata>(json, _settings);
            }

            return loadedData;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());

            return null;
        }
    }

    public bool Delete(int slotId)
    {
        if (CheckValidSlotIndex(slotId) == false)
        {
            return false;
        }

        string dirPath = _pathProvider.GetSaveDirPath(slotId);
        if (!Directory.Exists(dirPath))
        {
            return false;
        }

        try
        {
            Directory.Delete(dirPath, true);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    public Userdata CreateNewData(int characterType)
    {
        int slotId = 0;
        for (int i = 0; i < MAX_SLOT_COUNT; i++)
        {
            if (!IsSlotUsed(i))
            {
                slotId = i;
                break;
            }
        }

        Userdata newPlayerData = new Userdata(characterType)
        {
            playerType = characterType,
            gold = 0,
            level = 1,
            exp = 0,
            maxActivityPoint = 3,
            activityPoint = 3
        };
        newPlayerData.lastSavedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        Save(slotId, newPlayerData);
        _currentData = newPlayerData;

        return newPlayerData;
    }

    public void SetCurrentData(Userdata data, int slotId)
    {
        if (data == null)
        {
            return;
        }
        _currentData = data;
        _currentSlot = slotId;
    }
}
