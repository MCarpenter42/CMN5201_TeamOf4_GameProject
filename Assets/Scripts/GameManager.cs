using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class GameManager : Core
{
    #region [ OBJECTS ]

    public static GameDataHandler GameDataHandler;

    private static GameManager instance = null;
    private Controls controlsInstance;
    private VideoSettings vidSettingsInstance;

    public static Player Player;
    public static LevelController LevelController;

    public static UIController UIController;
    public static PauseMenu PauseMenu;

    public static AudioController AudioController;
    public static GameObject Listener;

#if UNITY_EDITOR
    private EditorSceneHandler editorSceneHandler;

    public SceneAsset MainMenuScene;
    public SceneAsset LevelTransitionScene;
    public List<SceneAsset> LevelScenes = new List<SceneAsset>();
#endif

    public static DebugLogging DebugLogging;

    #endregion

    #region [ PROPERTIES ]

    public static bool onGameLoad = true;

    public static float FPS;
    private List<float> frameTimes = new List<float>();

    public static bool isPaused = false;
    public static bool showHints = true;

    public static bool sceneChangeComplete = true;

    public static string pathListLocalPath = "/Scripts/Data/ScenePathList.json";
    public static string pathListFilepath { get { return Application.dataPath + "/Scripts/Data/ScenePathList.json"; } }
    public static ScenePathList scenePaths = new ScenePathList();

    public static bool[] levelsUnlocked = new bool[0];

    public static bool gameEndProcessComplete = false;

    #endregion

    #region [ COROUTINES ]

    Coroutine sceneChange = null;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ SINGLETON CONTROL ]

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameManager inst = FindObjectOfType<GameManager>();
                if (inst == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();

                    instance.Init();

                    // Prevents game manager from being destroyed on loading of a new scene
                    DontDestroyOnLoad(obj);

                    Debug.Log(obj.name);
                }
            }
            return instance;
        }
    }

    // Initialiser function, serves a similar purpose to a constructor
    private void Init()
    {
        Setup();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        if (Application.isPlaying)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        
            if (onGameLoad)
            {
                OnAwake();
                Setup();
            }
            else
            {
                OnAwake();
            }
        }
    }

    void Start()
    {
        DebugOnStart();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            UIController.fps = CalcFPS();
            HandleInputs();

            DebugOnUpdate();
        }
        else
        {
#if UNITY_EDITOR
            if (editorSceneHandler == null)
            {
                if (gameObject.GetComponent<EditorSceneHandler>() == null)
                {
                    editorSceneHandler = gameObject.AddComponent<EditorSceneHandler>();
                    editorSceneHandler.gameManager = this;
                }
                else
                {
                    editorSceneHandler = gameObject.GetComponent<EditorSceneHandler>();
                    editorSceneHandler.gameManager = this;
                }
            }
            else if (editorSceneHandler.gameManager == null)
            {
                editorSceneHandler.gameManager = this;
            }
#endif
            if (scenePaths.paths.Length == 0)
            {
                GetScenePaths();
            }
        }
    }

    void OnApplicationQuit()
    {
        if (!gameEndProcessComplete)
        {
            GameEndProcess();
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void OnAwake()
    {
        LevelController = FindObjectOfType<LevelController>();
        if (LevelController.isGameplayLevel)
        {
            Player = FindObjectOfType<Player>();
        }

        UIController = FindObjectOfType<UIController>();
        if (UIController.pauseMenu != null)
        {
            UIController.pauseMenu.Show(false);
        }

        AudioController = FindObjectOfType<AudioController>();
    }

    private void Setup()
    {
        GameDataHandler = GetOrAddComponent<GameDataHandler>(gameObject);

        controlsInstance = GetOrAddComponent<Controls>(gameObject);
        Controls = controlsInstance;

        vidSettingsInstance = GetOrAddComponent<VideoSettings>(gameObject);
        VideoSettings = vidSettingsInstance;

        DebugLogging = GetOrAddComponent<DebugLogging>(gameObject);

        GetScenePaths();

        if (onGameLoad)
        {
            DebugLogging.StartLogging();

            GameDataHandler.LoadEncryptionKeys();
            GameDataHandler.DataFromDisk();
            GameDataHandler.GameStateFromData();
            onGameLoad = false;
        }

        Listener = GetChildrenWithComponent<AudioListener>(gameObject)[0];
        DontDestroyOnLoad(Listener);
        OnSceneLoad();
    }

    public void OnPause()
    {
        GameDataHandler.GameStateToData();
    }

    public void OnResume()
    {

    }

    public void OnLog()
    {
        if (UIController != null && UIController.devConsole != null)
        {
            if (UIController.devConsole.console.visible)
            {
                UIController.devConsole.UpdateLog();
            }
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private float CalcFPS()
    {
        if (frameTimes.Count >= 60)
        {
            frameTimes.RemoveAt(0);
        }
        frameTimes.Add(Time.deltaTime);
        float total = 0.0f;
        foreach (float f in frameTimes)
        {
            total += f;
        }
        float fps = (float)frameTimes.Count / total;
        fps -= (fps % 0.01f);
        return FPS = fps;
    }

    private void HandleInputs()
    {
        if (LevelController.isGameplayLevel)
        {
            LevelController.PlayerInputs();
            LevelController.CameraInputs();
        }

        UIController.UIInputs();
    }

    #region [ GAME DATA ]



    #endregion

    #region [ SETTINGS ]

    public void LoadSettings()
    {
        LoadControls();
    }

    public void SaveSettings()
    {
        SaveControls();
        PlayerPrefs.Save();
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void LoadControls()
    {
        List<string> controlNames = Controls.GetNamesList();
        foreach (string name in controlNames)
        {
            if (PlayerPrefs.HasKey(name))
            {
                int keyIndex = PlayerPrefs.GetInt(name);
                Controls.SetControlByName(name, (KeyCode)keyIndex);
            }
            else
            {
                int keyIndex = (int)Controls.GetControlByName(name);
                PlayerPrefs.SetInt(name, keyIndex);
            }
        }
    }

    public void SaveControls()
    {
        List<string> controlNames = Controls.GetNamesList();
        foreach (string name in controlNames)
        {
            int keyIndex = (int)Controls.GetControlByName(name);
            PlayerPrefs.SetInt(name, keyIndex);
        }
    }

    #endregion

    #region [ SCENE HANDLING ]

    #region [ SETUP & QUERY ]

    public void OnSceneLoad()
    {
        if (LevelController.isGameplayLevel)
        {
            Debug.Log("Gameplay level loaded");
            Listener.transform.SetParent(Player.gameObject.transform, false);
            Listener.transform.localPosition = new Vector3(0.0f, 0.4f, 0.0f);
        }
        else
        {
            Listener.transform.localPosition = new Vector3(0.0f, 200.0f, 0.0f);
        }
    }
    
    public void OnSceneLoad(int sceneIndex)
    {
        OnSceneLoad();

        LevelController.sceneIndex = sceneIndex;
        if (LevelController.isGameplayLevel)
        {
            for (int i = 0; i < scenePaths.levels.Length; i++)
            {
                if (sceneIndex == scenePaths.levels[i])
                {
                    LevelController.levelIndex = i;
                }
            }
        }
    }

    private void GetScenePaths()
    {
        string jsonData = File.ReadAllText(pathListFilepath);
        scenePaths = JsonUtility.FromJson<ScenePathList>(jsonData);
    }
    
    public int SceneIndexFromName(string sceneName)
    {
        for (int i = 0; i < scenePaths.paths.Length; i++)
        {
            string buildSceneName = scenePaths.paths[i];
            int n1 = buildSceneName.LastIndexOf('/') + 1;
            int n2 = buildSceneName.LastIndexOf('.') - n1;
            if (buildSceneName.Substring(n1, n2) == sceneName)
            {
                return i;
            }
        }
        return -1;
    }
    
    public bool IsLevelScene(int sceneIndex)
    {
        bool isLevelScene = false;
        foreach (int index in scenePaths.levels)
        {
            if (sceneIndex == index)
            {
                isLevelScene = true;
                break;
            }
        }
        return isLevelScene;
    }

    #endregion

    #region [ SCENE CATEGORIES ]

    public bool GoToMainMenu()
    {
        bool successful = false;

        if (scenePaths.mainMenu != -1)
        {
            try
            {
                ChangeScene(scenePaths.mainMenu, true, true, 2.0f);
                successful = true;
            }
            catch
            {
                throw new System.Exception("ERROR: Could not load scene, as it was not found in the build index!");
            }
        }

        return successful;
    }

    public bool LoadLevel(int index)
    {
        bool successful = false;

        if (InBounds(index, scenePaths.levels))
        {
            try
            {
                ChangeScene(scenePaths.levels[index], true, true, 2.0f);
                successful = true;
            }
            catch
            {
                throw new System.Exception("ERROR: Could not load scene, as it was not found in the build index!");
            }
        }

        return successful;
    }
    
    public bool LoadUnlockedLevel(int index)
    {
        if (InBounds(index, levelsUnlocked) && levelsUnlocked[index])
        {
            return LoadLevel(index);
        }
        else
        {
            return false;
        }
    }

    public void NextLevel()
    {
        UnlockLevel(LevelController.levelIndex + 1);

        ChangeScene(LevelController.sceneIndex + 1, true, true);
    }

    public void UnlockLevel(int index)
    {
        if (InBounds(index, levelsUnlocked) && levelsUnlocked[index] == false)
        {
            levelsUnlocked[index] = true;
        }
    }

    #endregion

    #region [ SCENE CHANGES ]

    public void ChangeScene(int targetSceneIndex)
    {
        ChangeScene(targetSceneIndex, true, true);
    }

    public void ChangeScene(int targetSceneIndex, float loadScreenDelay)
    {
        ChangeScene(targetSceneIndex, true, true, loadScreenDelay);
    }
    
    public void ChangeScene(int targetSceneIndex, bool fadeOut, bool fadeIn)
    {
        if (sceneChangeComplete)
        {
            sceneChangeComplete = false;
            StartCoroutine(IChangeScene(targetSceneIndex, fadeOut, fadeIn, 0.5f));
        }
    }
    
    public void ChangeScene(int targetSceneIndex, bool fadeOut, bool fadeIn, float loadScreenDelay)
    {
        if (sceneChangeComplete)
        {
            sceneChangeComplete = false;
            StartCoroutine(IChangeScene(targetSceneIndex, fadeOut, fadeIn, loadScreenDelay));
        }
    }
    
    private IEnumerator IChangeScene(int targetSceneIndex, bool fadeOut, bool fadeIn, float loadScreenDelay)
    {
        Listener.transform.parent = gameObject.transform;

        if (fadeOut)
        {
            UIController.BlackScreenFade(true, LevelController.levelFadeTime);
            AudioController.MusicFade(true, LevelController.levelFadeTime);
            yield return new WaitForSecondsRealtime(LevelController.levelFadeTime);
        }

        Resume();

        /*AudioListener oldestListener = FindObjectOfType<AudioListener>();

        AsyncOperation loading = SceneManager.LoadSceneAsync(scenePaths.levelTransition, LoadSceneMode.Additive);
        while (!loading.isDone)
        {
            yield return null;
            oldestListener.enabled = (FindObjectsOfType<AudioListener>().Length == 1);
        }

        AsyncOperation unloading = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        while (!unloading.isDone)
        {
            yield return null;
        }
        oldestListener = FindObjectOfType<AudioListener>();

        FindObjectOfType<Camera>().backgroundColor = UIController.blackoutColour;
        yield return new WaitForSecondsRealtime(loadScreenDelay);

        loading = SceneManager.LoadSceneAsync(targetSceneIndex, LoadSceneMode.Additive);
        while (!loading.isDone)
        {
            yield return null;
            oldestListener.enabled = (FindObjectsOfType<AudioListener>().Length == 1);
        }

        unloading = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        while (!unloading.isDone)
        {
            yield return null;
        }*/

        AsyncOperation loading = SceneManager.LoadSceneAsync(targetSceneIndex);
        while (!loading.isDone)
        {
            yield return null;
        }

        OnSceneLoad(targetSceneIndex);
        GameDataHandler.GameStateToData();

        sceneChangeComplete = true;

        if (fadeIn)
        {
            UIController.BlackScreenFade(false, LevelController.levelFadeTime);
            AudioController.MusicFade(false, LevelController.levelFadeTime);
            yield return new WaitForSecondsRealtime(LevelController.levelFadeTime);
        }
    }

    private IEnumerator DelayedChangeScene(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        GoToScene('+');
    }
    
    #endregion

    #endregion

    #region [ DEBUG ]

    private void DebugOnStart()
    {
        /*for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            string sceneName = EditorBuildSettings.scenes[i].path;
            int n1 = sceneName.LastIndexOf('/') + 1;
            int n2 = sceneName.LastIndexOf('.') - n1;
            Debug.Log(sceneName.Substring(n1, n2));
        }*/
    }

    private void DebugOnUpdate()
    {
        //Debug.Log(FPS);
    }

    #endregion

    #region [ ON QUIT ]

    public void DelayedQuit(float delay)
    {
        GameEndProcess();
        StartCoroutine(IDelayedQuit(delay));
    }

    private IEnumerator IDelayedQuit(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

    private void GameEndProcess()
    {
        GameDataHandler.DataToDisk();
#if UNITY_EDITOR
        DebugLogging.LogToFile();
#endif

        gameEndProcessComplete = true;
    }

    #endregion
}
