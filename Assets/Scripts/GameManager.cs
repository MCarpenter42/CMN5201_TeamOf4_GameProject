using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class GameManager : Core
{
    #region [ OBJECTS ]

    private static GameManager instance = null;
    private Controls controlsInstance;
    private VideoSettings vidSettingsInstance;

    public static Player Player;
    public static LevelController LevelController;

    public static UIController UIController;
    public static PauseMenu PauseMenu;

    public static AudioController AudioController;
    public static GameObject Listener;

    public SceneAsset MainMenuScene;
    private int mainMenuBuildIndex = -1;
    
    public SceneAsset LevelTransitionScene;
    private int levelTransitionBuildIndex = -1;

    public SceneAsset[] LevelScenes;
    private int[] levelSceneIndices;

    #endregion

    #region [ PROPERTIES ]

    public static float FPS;
    private List<float> frameTimes = new List<float>();

    public static bool isPaused = false;
    public static bool showHints = true;

    public static bool sceneChangeComplete = true;

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

    void Start()
    {
        DebugOnStart();
    }

    void Update()
    {
        UIController.fps = CalcFPS();
        HandleInputs();

        DebugOnUpdate();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void Setup()
    {
        controlsInstance = gameObject.AddComponent<Controls>();
        Controls = controlsInstance;
        vidSettingsInstance = gameObject.AddComponent<VideoSettings>();
        VideoSettings = vidSettingsInstance;

        GetSceneIndices();
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

    private void GetSceneIndices()
    {
        mainMenuBuildIndex = SceneIndexFromName(MainMenuScene.name);
        
        levelTransitionBuildIndex = SceneIndexFromName(LevelTransitionScene.name);

        levelSceneIndices = new int[LevelScenes.Length];
        for (int i = 0; i < LevelScenes.Length; i++)
        {
            levelSceneIndices[i] = SceneIndexFromName(LevelScenes[i].name);
        }
    }

    private int SceneIndexFromName(string sceneName)
    {
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            string buildSceneName = EditorBuildSettings.scenes[i].path;
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

        if (MainMenuScene != null)
        {
            try
            {
                ChangeScene(mainMenuBuildIndex);
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

        if (InBounds(index, LevelScenes))
        {
            try
            {
                ChangeScene(levelSceneIndices[index]);
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
        bool successful = false;

        if (InBounds(index - 1, LevelScenes))
        {
            try
            {
                ChangeScene(levelSceneIndices[index - 1]);
                successful = true;
            }
            catch
            {
                throw new System.Exception("ERROR: Could not load scene, as it was not found in the build index!");
            }
        }

        return successful;
    }

    public void NextLevel()
    {
        StartCoroutine(DelayedChangeScene(LevelController.levelFadeTime));
        if (GameManager.UIController.blackScreen != null)
        {
            GameManager.UIController.BlackScreenFade(true, LevelController.levelFadeTime);
        }
    }

    public void ChangeScene(int targetSceneIndex)
    {
        ChangeScene(targetSceneIndex, true, true);
    }
    
    public void ChangeScene(string targetSceneName)
    {
        ChangeScene(targetSceneName, true, true);
    }
    
    public void ChangeScene(int targetSceneIndex, float loadScreenDelay)
    {
        ChangeScene(targetSceneIndex, true, true, loadScreenDelay);
    }
    
    public void ChangeScene(string targetSceneName, float loadScreenDelay)
    {
        ChangeScene(targetSceneName, true, true, loadScreenDelay);
    }
    
    public void ChangeScene(int targetSceneIndex, bool fadeOut, bool fadeIn)
    {
        if (sceneChangeComplete)
        {
            sceneChangeComplete = false;
            StartCoroutine(IChangeScene(targetSceneIndex, fadeOut, fadeIn, 0.5f));
        }
    }
    
    public void ChangeScene(string targetSceneName, bool fadeOut, bool fadeIn)
    {
        if (sceneChangeComplete)
        {
            sceneChangeComplete = false;
            StartCoroutine(IChangeScene(SceneIndexFromName(targetSceneName), fadeOut, fadeIn, 0.5f));
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
    
    public void ChangeScene(string targetSceneName, bool fadeOut, bool fadeIn, float loadScreenDelay)
    {
        if (sceneChangeComplete)
        {
            sceneChangeComplete = false;
            StartCoroutine(IChangeScene(SceneIndexFromName(targetSceneName), fadeOut, fadeIn, loadScreenDelay));
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

        AsyncOperation loading = SceneManager.LoadSceneAsync(levelTransitionBuildIndex, LoadSceneMode.Additive);
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
