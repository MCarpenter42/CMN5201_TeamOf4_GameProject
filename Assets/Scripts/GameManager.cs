using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class GameManager : Core
{
    #region [ OBJECTS ]

    public static GameDataHandler GameDataHandler;

    private static GameManager instance = null;
    public static Controls controlsInstance;
    private VideoSettings vidSettingsInstance;

    public static EventSystem EventSystem;
    public static Player Player;
    public static LevelController LevelController;

    public static UIController UIController;
    public static PauseMenu PauseMenu;

    public static AudioController AudioController;
    public static GameObject Listener;

#if UNITY_EDITOR
    //private EditorSceneHandler editorSceneHandler;

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

    public static string pathListLocalPath = "/Resources/Data/ScenePathList.json";
    public static string pathListFilepath { get { return Application.dataPath + "/Resources/Data/ScenePathList.json"; } }
    [SerializeField] public ScenePathList scenePaths { get { return GetOrAddComponent<ScenePathList>(gameObject); } }

    public static bool[] levelsUnlocked = new bool[0];

    public Color blackoutColour;

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
/*#if UNITY_EDITOR
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
#endif*/
        }
#if UNITY_EDITOR
        GetScenePaths();
#endif
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
        UIController.blackoutColour = blackoutColour;

        AudioController = FindObjectOfType<AudioController>();
    }

    private void Setup()
    {
        GameDataHandler = GetOrAddComponent<GameDataHandler>(gameObject);

        EventSystem = GetOrAddComponent<EventSystem>(gameObject);
        controlsInstance = GetOrAddComponent<Controls>(gameObject);

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
        if (LevelController.isGameplayLevel && !UIController.devConsole.console.visible)
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
        OnSceneLoad(SceneIndexFromName(SceneManager.GetActiveScene().name));
    }
    
    public void OnSceneLoad(int sceneIndex)
    {
        if (LevelController.isGameplayLevel)
        {
            Listener.transform.SetParent(Player.gameObject.transform, false);
            Listener.transform.localPosition = new Vector3(0.0f, 0.4f, 0.0f);
        }
        else
        {
            Listener.transform.localPosition = new Vector3(0.0f, 200.0f, 0.0f);
        }

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

    /*private void GetScenePaths()
    {
        string jsonData = File.ReadAllText(pathListFilepath);
        scenePaths = JsonUtility.FromJson<ScenePathList>(jsonData);
    }*/
    
    private void GetScenePaths()
    {
#if UNITY_EDITOR
        string[] pathArray = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            pathArray[i] = EditorBuildSettings.scenes[i].path;
        }

        scenePaths.paths = pathArray;

        scenePaths.mainMenu = SceneIndexFromName(MainMenuScene.name);
        scenePaths.levelTransition = SceneIndexFromName(LevelTransitionScene.name);
        scenePaths.levels = new int[LevelScenes.Count];
        for (int i = 0; i < LevelScenes.Count; i++)
        {
            scenePaths.levels[i] = SceneIndexFromName(LevelScenes[i].name);
        }

        string jsonData = JsonUtility.ToJson(scenePaths);
        File.WriteAllText(pathListFilepath, jsonData);
#endif
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
                ChangeScene(scenePaths.mainMenu, true, true, 1.0f);
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
                ChangeScene(scenePaths.levels[index], true, true, 1.0f);
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
        int n = LevelController.levelIndex + 1;
        UnlockLevel(n);
        LoadLevel(n);
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
        Listener.transform.SetParent(gameObject.transform);
        int currentSceneIndex = LevelController.sceneIndex;

        if (fadeOut)
        {
            UIController.BlackScreenFade(true, LevelController.levelFadeTime);
            AudioController.AudioFade(true, LevelController.levelFadeTime);
            yield return new WaitForSecondsRealtime(LevelController.levelFadeTime);
        }

        Resume();

        AsyncOperation loading = SceneManager.LoadSceneAsync(scenePaths.levelTransition, LoadSceneMode.Additive);
        while (!loading.isDone)
        {
            yield return null;
        }
        loading = null;

        Scene sceneToSetActive = SceneManager.GetSceneByPath(scenePaths.paths[scenePaths.levelTransition]);
        SceneManager.SetActiveScene(sceneToSetActive);

        AsyncOperation unloading = SceneManager.UnloadSceneAsync(scenePaths.paths[currentSceneIndex]);
        while (!unloading.isDone)
        {
            yield return null;
        }
        unloading = null;

        FindObjectOfType<Camera>().backgroundColor = UIController.blackoutColour;
        yield return new WaitForSecondsRealtime(loadScreenDelay);

        loading = SceneManager.LoadSceneAsync(targetSceneIndex, LoadSceneMode.Additive);
        while (!loading.isDone)
        {
            yield return null;
        }
        loading = null;

        sceneToSetActive = SceneManager.GetSceneByPath(scenePaths.paths[targetSceneIndex]);
        SceneManager.SetActiveScene(sceneToSetActive);

        unloading = SceneManager.UnloadSceneAsync(scenePaths.paths[scenePaths.levelTransition]);
        while (!unloading.isDone)
        {
            yield return null;
        }
        unloading = null;

        /*AsyncOperation loading = SceneManager.LoadSceneAsync(targetSceneIndex);
        while (!loading.isDone)
        {
            yield return null;
        }*/

        OnAwake();
        OnSceneLoad(targetSceneIndex);
        GameDataHandler.GameStateToData();

        sceneChangeComplete = true;

        if (fadeIn)
        {
            UIController.BlackScreenFade(false, LevelController.levelFadeTime);
            AudioController.AudioFade(false, LevelController.levelFadeTime);
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
        //GameEndProcess();
        StartCoroutine(IDelayedQuit(delay));
    }

    private IEnumerator IDelayedQuit(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
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
