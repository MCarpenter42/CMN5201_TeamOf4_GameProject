using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class GameManager : Core
{
    #region [ OBJECTS ]

    private static GameManager instance = null;
    private Controls controlsInstance;

    public static Player Player;

    public static LevelController LevelController;

    public static UIController UIController;

    #endregion

    #region [ PROPERTIES ]

    public static bool isPaused = false;

    public static bool firstLoad = true;

    public static float FPS;
    private List<float> frameTimes = new List<float>();

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
        DontDestroyOnLoad(gameObject);

        Setup();
    }

    void Start()
    {
        DebugOnStart();
    }

    void Update()
    {
        CalcFPS();
        HandleInputs();

        DebugOnUpdate();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void Setup()
    {
        controlsInstance = gameObject.AddComponent<Controls>();
        Controls = controlsInstance;

        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        LevelController = FindObjectOfType<LevelController>();
        UIController = FindObjectOfType<UIController>();

        if (firstLoad)
        {
            firstLoad = false;
            OnFirstLoad();
        }
    }

    private void OnFirstLoad()
    {
        
    }

    private void CalcFPS()
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
        FPS = (int)((float)frameTimes.Count / total);
    }

    private void HandleInputs()
    {
        LevelController.LevelInputs();
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

    #region [ DEBUG ]

    private void DebugOnStart()
    {

    }

    private void DebugOnUpdate()
    {
        //Debug.Log(FPS);
    }

    #endregion
}
