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

    private static GameManager instance = null;
    public static Controls controlsInstance;
    private VideoSettings vidSettingsInstance;

    public static EventSystem EventSystem;
    public static Player Player;
    public static LevelController LevelController;

    public static UIController UIController;
    public static PauseMenu PauseMenu;

    public static AudioController AudioController;
    public GameObject Listener;

#if UNITY_EDITOR
    //private EditorSceneHandler editorSceneHandler;

    public SceneAsset MainMenuScene;
    public SceneAsset LevelTransitionScene;
    public List<SceneAsset> LevelScenes = new List<SceneAsset>();
#endif

    public static DebugLogging DebugLogging;

    #endregion

    #region [ PROPERTIES ]

    #region [ PERSISTENT DATA ]

    public string encryption_AES_Key;
    public string encryption_AES_IV;
    public string encryption_DES_Key;
    public int encryption_XOR_Key;

    public string[] buildScenePaths;
    public int scenes_MainMenu;
    public int scenes_LoadScreen;
    public int[] scenes_Levels;

    public static DataEncryptionKeys EncryptionKeys = new DataEncryptionKeys();

    #endregion

    public PersistentData PersistentData { get { return GetOrAddComponent<PersistentData>(gameObject); } }

    public static bool onGameLoad = true;

    private Controls defaultControls = new Controls();

    public static float FPS;
    private List<float> frameTimes = new List<float>();

    public static bool isPaused = false;
    public static bool showHints = true;

    public static bool sceneChangeComplete = true;

    public static bool[] levelsUnlocked = new bool[16];

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
        //Setup();
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

    private void Setup()
    {
        OnAwake();
        LoadSettings();

        onGameLoad = false;

        DebugLogging = GetOrAddComponent<DebugLogging>(gameObject);
        DebugLogging.StartLogging();

        PersistentData.GetPersistentData();

        GameDataHandler.DataFromDisk();
        GameDataHandler.GameStateFromData();

        EventSystem = GetOrAddComponent<EventSystem>(gameObject);

        vidSettingsInstance = GetOrAddComponent<VideoSettings>(gameObject);
        VideoSettings = vidSettingsInstance;

        Listener = GetChildrenWithComponent<AudioListener>(gameObject)[0];
        OnSceneLoad();
    }

    public void OnAwake()
    {
        LevelController = FindObjectOfType<LevelController>();
        if (LevelController.isGameplayLevel)
        {
            Player = FindObjectOfType<Player>();
        }

        UIController = FindObjectOfType<UIController>();
        AudioController = FindObjectOfType<AudioController>();

        if (UIController.pauseMenu != null)
        {
            UIController.pauseMenu.Show(false);
        }
        UIController.blackoutColour = blackoutColour;
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
            if (GetInputDown(Controls.General.ResetLevel))
            {
                LevelController.RestartLevel();
            }
        }

        UIController.UIInputs();
    }

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
        foreach (string name in Controls.controlNames)
        {
            if (PlayerPrefs.HasKey(name))
            {
                int keyIndex = PlayerPrefs.GetInt(name);
                Debug.Log("1: " + name + ", " + keyIndex + ", " + (KeyCode)keyIndex);
                Controls.SetControlByName(name, (KeyCode)keyIndex);
                Debug.Log("2: " + name + ", " + keyIndex + ", " + (KeyCode)keyIndex);
            }
            else
            {
                int keyIndex = (int)Controls.GetControlByName(name).Key;
                PlayerPrefs.SetInt(name, keyIndex);
            }
        }
    }

    public void SaveControls()
    {
        Debug.Log("Saving controls...");
        foreach (string name in Controls.controlNames)
        {
            int keyIndex = (int)Controls.GetControlByName(name).Key;
            Debug.Log("3: " + name + ", " + keyIndex + ", " + (KeyCode)keyIndex);
            PlayerPrefs.SetInt(name, keyIndex);
        }
    }

    public void ResetControls()
    {
        for (int i = 0; i < Controls.controlNames.Count; i++)
        {
            string ctrl = Controls.controlNames[i];
            Controls.SetControlByName(ctrl, defaultControls.GetControlByName(ctrl).Key);
        }
        SaveControls();
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
        LevelController.sceneIndex = sceneIndex;

        if (LevelController.isGameplayLevel)
        {
            for (int i = 0; i < scenes_Levels.Length; i++)
            {
                if (sceneIndex == scenes_Levels[i])
                {
                    LevelController.levelIndex = i;
                }
            }
        }
    }

    public bool IsLevelScene(int sceneIndex)
    {
        bool isLevelScene = false;
        foreach (int index in PersistentData.scenes_Levels)
        {
            if (sceneIndex == index)
            {
                isLevelScene = true;
                break;
            }
        }
        return isLevelScene;
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

    #endregion

    #region [ SCENE CATEGORIES ]

    public bool GoToMainMenu()
    {   
        bool successful = false;

        if (scenes_MainMenu != -1)
        {
            try
            {
                ChangeScene(scenes_MainMenu, true, true, 1.0f);
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

        if (InBounds(index, scenes_Levels))
        {
            try
            {
                ChangeScene(scenes_Levels[index], true, true, 1.0f);
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
        if (fadeOut)
        {
            UIController.BlackScreenFade(true, LevelController.levelFadeTime);
            AudioController.AudioFade(true, LevelController.levelFadeTime);
            yield return new WaitForSecondsRealtime(LevelController.levelFadeTime);
        }

        Resume();

        AsyncOperation loading = SceneManager.LoadSceneAsync(targetSceneIndex, LoadSceneMode.Single);
        while (!loading.isDone)
        {
            yield return null;
        }
        loading = null;

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
    
    /*private IEnumerator IChangeScene(int targetSceneIndex, bool fadeOut, bool fadeIn, float loadScreenDelay)
    {
        Listener.transform.SetParent(gameObject.transform);
        int currentSceneIndex = LevelController.sceneIndex;
        if (currentSceneIndex == -1)
        {
            currentSceneIndex = PersistentData.SceneIndexFromName(SceneManager.GetActiveScene().name);
        }
        Debug.Log("Current scene index: " + currentSceneIndex);
        if (fadeOut)
        {
            UIController.BlackScreenFade(true, LevelController.levelFadeTime);
            AudioController.AudioFade(true, LevelController.levelFadeTime);
            yield return new WaitForSecondsRealtime(LevelController.levelFadeTime);
        }

        Resume();

        AsyncOperation loading = SceneManager.LoadSceneAsync(PersistentData.scenes_LoadScreen, LoadSceneMode.Additive);
        while (!loading.isDone)
        {
            yield return null;
        }
        loading = null;

        Scene sceneToSetActive = SceneManager.GetSceneByPath(PersistentData.buildScenePaths[PersistentData.scenes_LoadScreen]);
        SceneManager.SetActiveScene(sceneToSetActive);

        AsyncOperation unloading = SceneManager.UnloadSceneAsync(PersistentData.buildScenePaths[currentSceneIndex]);
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

        sceneToSetActive = SceneManager.GetSceneByPath(PersistentData.buildScenePaths[targetSceneIndex]);
        SceneManager.SetActiveScene(sceneToSetActive);

        unloading = SceneManager.UnloadSceneAsync(PersistentData.buildScenePaths[PersistentData.scenes_LoadScreen]);
        while (!unloading.isDone)
        {
            yield return null;
        }
        unloading = null;

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
    }*/

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
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }

    private void GameEndProcess()
    {
        SaveSettings();
        GameDataHandler.DataToDisk();
#if UNITY_EDITOR
        DebugLogging.LogToFile();
#endif

        gameEndProcessComplete = true;
    }

    #endregion
}
