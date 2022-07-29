using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class EditorSceneHandler : Core
{
/*    [HideInInspector] public GameManager gameManager;

    [HideInInspector] public ScenePathList scenePaths = new ScenePathList();

    private bool scenesChanged = false;

    *//* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - *//*

#if UNITY_EDITOR
    void Awake()
    {
        GetScenePaths();
        SavePathsToDisk();
    }

    void Update()
    {
        scenesChanged = GetScenePaths();
        if (scenesChanged)
        {
            SavePathsToDisk();
        }
    }

    *//* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - *//*

    public bool GetScenePaths()
    {
        bool scenesChanged = false;

        if (scenePaths.paths.Length != EditorBuildSettings.scenes.Length)
        {
            scenesChanged = true;
        }

        string[] pathArray = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            pathArray[i] = EditorBuildSettings.scenes[i].path;
            if (InBounds(i, EditorBuildSettings.scenes))
            {
                if (pathArray[i] != EditorBuildSettings.scenes[i].path)
                {
                    scenesChanged = true;
                }
            }
            else
            {
                scenesChanged = true;
            }
        }

        if (scenesChanged)
        {
            scenePaths.paths = pathArray;
        }

        return scenesChanged;
    }

    public int GetBuildSceneIndex(SceneAsset scene)
    {
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            string buildSceneName = EditorBuildSettings.scenes[i].path;
            int n1 = buildSceneName.LastIndexOf('/') + 1;
            int n2 = buildSceneName.LastIndexOf('.') - n1;
            if (buildSceneName.Substring(n1, n2) == scene.name)
            {
                return i;
            }
        }
        return -1;
    }

    public void SavePathsToDisk()
    {
        scenePaths.mainMenu = GetBuildSceneIndex(gameManager.MainMenuScene);
        scenePaths.levelTransition = GetBuildSceneIndex(gameManager.LevelTransitionScene);
        int n = gameManager.LevelScenes.Count;
        scenePaths.levels = new int[n];
        for (int i = 0; i < n; i++)
        {
            scenePaths.levels[i] = GetBuildSceneIndex(gameManager.LevelScenes[i]);
        }

        scenePaths.timeModified = "UTC " + System.DateTime.UtcNow;
        string jsonData = JsonUtility.ToJson(scenePaths);
        File.WriteAllText(GameManager.pathListFilepath, jsonData);
    }
#endif*/
}
