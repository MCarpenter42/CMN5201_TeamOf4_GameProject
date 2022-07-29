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

    public static GameData DataMaster = new GameData();

    #endregion

    #region [ PROPERTIES ]

    public static DataEncryptionKeys keys = new DataEncryptionKeys();

    private static string keysFilepath { get { return Application.dataPath + "/Resources/Data/EncryptionKeys.json"; } }
    /*private string saveDataFilepath { get { return Application.dataPath + "/SaveData/GameSaveData.json"; } }
#if UNITY_EDITOR
    private string saveDataFilepath_UNENC { get { return Application.dataPath + "/SaveData/GameSaveData_UNENC.json"; } }
#endif*/
    private static string saveDataFileName = "SaveData.dat";

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

    /*public static void LoadEncryptionKeys()
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
    }*/

    #region [ GAME STATE <-> LOCAL DATA ]

    public static void GameStateToData()
    {
        DataMaster.levelsUnlocked = GameManager.levelsUnlocked;
    }

    public static void GameStateFromData()
    {
        GameManager.levelsUnlocked = DataMaster.levelsUnlocked;
    }

    #endregion
    
    #region [ LOCAL DATA <-> SAVE FILES ]

    public static void DataToDisk()
    {
#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder($"Assets/SaveData"))
        {
            AssetDatabase.CreateFolder("Assets", "SaveData");
        }
#endif
        EncryptedObject encData = EncryptionHandler.AES.Encrypt(DataMaster, GameManager.EncryptionKeys.aesKey, GameManager.EncryptionKeys.aesIV);
        FileHandler.SaveData(encData, saveDataFileName);

        /*File.WriteAllText(saveDataFilepath, JsonUtility.ToJson(EncryptionHandler.AES.Encrypt(DataMaster, keys.aesKey, keys.aesIV)));
#if UNITY_EDITOR
        File.WriteAllText(saveDataFilepath_UNENC, JsonUtility.ToJson(DataMaster));
#endif*/
    }

    public static void DataFromDisk()
    {
        EncryptedObject encData = FileHandler.LoadData(saveDataFileName) as EncryptedObject;
        if (encData == null)
        {
            DataMaster = new GameData();
        }
        else
        {
            DataMaster = EncryptionHandler.AES.Decrypt<GameData>(encData, GameManager.EncryptionKeys.aesKey, GameManager.EncryptionKeys.aesIV);
        }
        if (DataMaster.levelsUnlocked.Length == 0)
        {
            SetupUnlockArray();
        }

        /*if (File.Exists(saveDataFilepath))
        {
            string encString = File.ReadAllText(saveDataFilepath);
            EncryptedObject encData = JsonUtility.FromJson<EncryptedObject>(encString);
            DataMaster = EncryptionHandler.AES.Decrypt<GameData>(encData, keys.aesKey, keys.aesIV);
            if (DataMaster.levelsUnlocked.Length == 0)
            {
                Debug.Log("Length of loaded level unlock array is 0!");
                SetupUnlockArray();
                DataToDisk();
            }
        }
        else
        {
            SetupUnlockArray();
            
            DataToDisk();
        }*/
    }

    #endregion
    
    #region [ OTHER ]

    private static void SetupUnlockArray()
    {
        int n = GameManager.Instance.scenes_Levels.Length;
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
    }

    #endregion

}

[Serializable]
public class GameData
{
    public bool firstTimePlaying = true;
    public bool[] levelsUnlocked = new bool[0];
}
