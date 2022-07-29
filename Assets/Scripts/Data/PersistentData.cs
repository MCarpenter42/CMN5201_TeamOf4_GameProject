using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
[ExecuteInEditMode]
public class PersistentData : Core
{
	[SerializeField] public string dataFolder { get { return Application.dataPath + "/Resources/Data/"; } }

	[SerializeField] public string encryption_KeysFileName = "EncryptionKeys.json";
	[SerializeField] public string encryption_AES_Key;
	[SerializeField] public string encryption_AES_IV;
	[SerializeField] public string encryption_DES_Key;
	[SerializeField] public int encryption_XOR_Key;

	[SerializeField] public string[] buildScenePaths;
	[SerializeField] public int scenes_MainMenu;
	[SerializeField] public int scenes_LoadScreen;
	[SerializeField] public int[] scenes_Levels;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

#if UNITY_EDITOR
    void Update()
    {
        GetPersistentData();
    }
#endif

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void GetPersistentData()
    {
        bool isInEditor = false;
#if UNITY_EDITOR
        isInEditor = true;

        GameManager gmMngr = gameObject.GetComponent<GameManager>();
        if (gmMngr == null)
        {
            Debug.LogError("Persistent data object should be attached to the Game Manager prefab!");
        }
        else
        {
            if (!AssetDatabase.IsValidFolder($"Assets/Resources/Data"))
            {
                AssetDatabase.CreateFolder("Asset/Resources", "Data");
            }

            // ENCRYPTION KEYS
            {
                DataEncryptionKeys keys = GameManager.EncryptionKeys;
                string encKeysPath = dataFolder + encryption_KeysFileName;
                if (File.Exists(encKeysPath))
                {
                    keys = JsonUtility.FromJson<DataEncryptionKeys>(File.ReadAllText(encKeysPath));
                    EncryptionHandler.SetAllDefaults(keys.aesKey, keys.aesIV, keys.desKey, keys.xorKey);
                }
                else
                {
                    EncryptionHandler.RandomiseAllDefaults();
                    keys.SetKeys(EncryptionHandler.AES.GetDefaults()[0], EncryptionHandler.AES.GetDefaults()[1], EncryptionHandler.DES.GetDefault(), EncryptionHandler.XOR.GetDefault());
                    File.WriteAllText(encKeysPath, JsonUtility.ToJson(keys));
                }
                encryption_AES_Key = keys.aesKey;
                encryption_AES_IV = keys.aesIV;
                encryption_DES_Key = keys.desKey;
                encryption_XOR_Key = keys.xorKey;
            }

            // SCENE INDICES
            {
                string[] pathArray = new string[EditorBuildSettings.scenes.Length];
                for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    pathArray[i] = EditorBuildSettings.scenes[i].path;
                }

                buildScenePaths = pathArray;

                scenes_MainMenu = SceneIndexFromName(gmMngr.MainMenuScene.name);
                scenes_LoadScreen = SceneIndexFromName(gmMngr.LevelTransitionScene.name);
                scenes_Levels = new int[gmMngr.LevelScenes.Count];
                for (int i = 0; i < gmMngr.LevelScenes.Count; i++)
                {
                    scenes_Levels[i] = SceneIndexFromName(gmMngr.LevelScenes[i].name);
                }
            }
        }
#endif
        if (Application.isPlaying || !isInEditor)
        {
            GameManager.EncryptionKeys = new DataEncryptionKeys(encryption_AES_Key, encryption_AES_IV, encryption_DES_Key, encryption_XOR_Key);
        }
    }

    public int SceneIndexFromName(string sceneName)
    {
        for (int i = 0; i < buildScenePaths.Length; i++)
        {
            string buildSceneName = buildScenePaths[i];
            int n1 = buildSceneName.LastIndexOf('/') + 1;
            int n2 = buildSceneName.LastIndexOf('.') - n1;
            if (buildSceneName.Substring(n1, n2) == sceneName)
            {
                return i;
            }
        }
        return -1;
    }
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
