using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class GameDataHandler : Core
{
    #region [ OBJECTS ]

    public GameData DataMaster { get; private set; }

    #endregion

    #region [ PROPERTIES ]

    public static DataEncryptionKeys keys = new DataEncryptionKeys();

    private string keysFilepath { get { return Application.dataPath + "/Scripts/Data/EncryptionKeys.json"; } }
    private string saveDataFilepath { get { return Application.dataPath + "/SaveData/GameSaveData.json"; } }
#if UNITY_EDITOR
    private string saveDataFilepath_UNENC { get { return Application.dataPath + "/SaveData/GameSaveData_UNENC.json"; } }
#endif

    private bool keysLoaded = false;
    private bool doSave = false;
    private bool startupLoaded = false;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    public GameDataHandler()
    {
        DataMaster = new GameData();
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]



    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void LoadEncryptionKeys()
    {
        if (!keysLoaded)
        {
            if (File.Exists(keysFilepath))
            {
                keys = JsonUtility.FromJson<DataEncryptionKeys>(File.ReadAllText(keysFilepath));
                EncryptionHandler.SetAllDefaults(keys.aesKey, keys.aesIV, keys.desKey, keys.xorKey);
            }
            else
            {
                EncryptionHandler.RandomiseAllDefaults();
                keys.SetKeys(EncryptionHandler.AES.GetDefaults()[0], EncryptionHandler.AES.GetDefaults()[1], EncryptionHandler.DES.GetDefault(), EncryptionHandler.XOR.GetDefault());
                File.WriteAllText(keysFilepath, JsonUtility.ToJson(keys));
            }

            keysLoaded = true;
        }
    }

    #region [ GAME STATE <-> LOCAL DATA ]

    public void GameStateToData()
    {
        if (doSave)
        {

        }
    }

    public void GameStateFromData()
    {
        if (startupLoaded)
        {
            doSave = true;
            GameManager.levelsUnlocked = DataMaster.levelsUnlocked;
        }
    }

    #endregion
    
    #region [ LOCAL DATA <-> SAVE FILES ]

    public void DataToDisk()
    {
#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder($"Assets/SaveData"))
        {
            AssetDatabase.CreateFolder("Assets", "SaveData");
        }
#endif
        if (!keysLoaded)
        {
            LoadEncryptionKeys();
        }

        File.WriteAllText(saveDataFilepath, JsonUtility.ToJson(EncryptionHandler.AES.Encrypt(DataMaster, keys.aesKey, keys.aesIV)));
#if UNITY_EDITOR
        File.WriteAllText(saveDataFilepath_UNENC, JsonUtility.ToJson(DataMaster));
#endif
    }

    public void DataFromDisk()
    {
        if (!keysLoaded)
        {
            LoadEncryptionKeys();
        }

        if (File.Exists(saveDataFilepath))
        {
            string encString = File.ReadAllText(saveDataFilepath);
            EncryptedObject encData = JsonUtility.FromJson<EncryptedObject>(encString);
            /*try
            {*/
                DataMaster = EncryptionHandler.AES.Decrypt<GameData>(encData, keys.aesKey, keys.aesIV);
            /*}
            catch
            {
                throw new Exception("ERROR: Failed to decrypt save file, as it was saved using different encryption keys to those currently in use!");
            }*/
        }
        else
        {
            int n = GameManager.scenePaths.levels.Length;
            DataMaster.levelsUnlocked = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    DataMaster.levelsUnlocked[i] = true;
                }
                else
                {
                    DataMaster.levelsUnlocked[i] = false;
                }
            }

            DataToDisk();
        }
    }

    #endregion
    
    #region [  ]



    #endregion

}

public class GameData
{
    public bool firstTimePlaying = true;
    public bool[] levelsUnlocked = new bool[0];
}

public class DataEncryptionKeys
{
    public string aesKey;
    public string aesIV;
    public string desKey;
    public int xorKey;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public DataEncryptionKeys()
    { }
    
    public DataEncryptionKeys(string aesKey, string aesIV, string desKey, int xorKey)
    {
        this.aesKey = aesKey;
        this.aesIV = aesIV;
        this.desKey = desKey;
        this.xorKey = xorKey;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void SetKeys(string aesKey, string aesIV, string desKey, int xorKey)
    {
        this.aesKey = aesKey;
        this.aesIV = aesIV;
        this.desKey = desKey;
        this.xorKey = xorKey;
    }
}
