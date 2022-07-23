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
            }
            else
            {
                Destroy(this.gameObject);
            }
        
            DontDestroyOnLoad(gameObject);

            Setup();
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
        GameDataHandler.DataToDisk();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void Setup()
    {
        GameDataHandler = GetOrAddComponent<GameDataHandler>(gameObject);
        controlsInstance = GetOrAddComponent<Controls>(gameObject);
        Controls = controlsInstance;
        vidSettingsInstance = GetOrAddComponent<VideoSettings>(gameObject);
        VideoSettings = vidSettingsInstance;

        if (onGameLoad)
        {
            GameDataHandler.LoadEncryptionKeys();
            GameDataHandler.DataFromDisk();
            GameDataHandler.GameStateFromData();
            onGameLoad = false;
        }

        GetScenePaths();
        OnLevelLoad();
    }

    public void OnLevelLoad()
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
        Listener = FindObjectOfType<AudioListener>().gameObject;
    }

    public void OnPause()
    {
        GameDataHandler.GameStateToData();
    }

    public void OnResume()
    {

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
    
    public bool GoToMainMenu()
    {
        bool successful = false;

        if (scenePaths.mainMenu != -1)
        {
            try
            {
                ChangeScene(scenePaths.mainMenu);
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
                ChangeScene(scenePaths.levels[index]);
                successful = true;
            }
            catch
            {
                throw new System.Exception("ERROR: Could not load scene, as it was not found in the build index!");
            }
        }

        return successful;
    }
    
    public bool LoadLevelAdjusted(int index)
    {
        return LoadLevel(index - 1);
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
    
    public bool LoadUnlockedLevelAdjusted(int index)
    {
        if (InBounds(index, levelsUnlocked) && levelsUnlocked[index])
        {
            return LoadLevelAdjusted(index);
        }
        else
        {
            return false;
        }
    }

    public void NextLevel()
    {
        StartCoroutine(DelayedChangeScene(LevelController.levelFadeTime));
        if (GameManager.UIController.blackScreen != null)
        {
            GameManager.UIController.BlackScreenFade(true, LevelController.levelFadeTime);
        }
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
            AudioController.MusicFade(true, LevelController.levelFadeTime);
            yield return new WaitForSeconds(LevelController.levelFadeTime);
        }

        AudioListener oldestListener = FindObjectOfType<AudioListener>();

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
        yield return new WaitForSeconds(loadScreenDelay);

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
        }

        sceneChangeComplete = true;

        if (fadeIn)
        {
            UIController.BlackScreenFade(false, LevelController.levelFadeTime);
            AudioController.MusicFade(false, LevelController.levelFadeTime);
            yield return new WaitForSeconds(LevelController.levelFadeTime);
        }
    }

    private IEnumerator DelayedChangeScene(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        GoToScene('+');
    }

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
}
