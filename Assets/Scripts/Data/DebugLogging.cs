using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class DebugLogging : Core
{
    public static List<string> debugLog = new List<string>();

    public static string logStart;
    public static string logEnd;

    private static string logLineHeader;
    private static string logLineContent;
    private static string stack;

    private static string directory { get { return Application.dataPath + "/OutputLogs"; } }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    void Start()
    {
        NewLog();
    }

    /*void OnEnable()
    {
        StartLogging();
    }*/

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void StartLogging()
    {
        NewLog();
        Application.logMessageReceived += Log;
    }

    public void NewLog()
    {
        debugLog.Clear();
        logStart = DateTime.Now.ToString();
        logEnd = "";
        debugLog.Add(">> LOGGING STARTED: " + logStart + " <<");
        debugLog.Add("");
        debugLog.Add("");
        //GameManager.Instance.OnLog();
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        stack = stackTrace;
        string typeString = type.ToString();

        SendLogMessage(logString, typeString);
    }

    public void LogCommand(string logString)
    {
        SendLogMessage(logString, "ConsoleCommand");
    }
    
    public void LogSystemMessage(string logString)
    {
        SendLogMessage(logString, "SystemMessage");
    }

    public void SendLogMessage(string logString, string logType)
    {
        string timestamp = DateTime.Now.ToString().Substring(11);

        logLineHeader = timestamp + " | " + logType;
        debugLog.Add(logLineHeader);
        logLineContent = ">>> " + logString;
        debugLog.Add(logLineContent);
        debugLog.Add("");

        GameManager.Instance.OnLog();
    }

#if UNITY_EDITOR
    public void LogToFile()
    {
        if (!AssetDatabase.IsValidFolder($"Assets/OutputLogs"))
        {
            AssetDatabase.CreateFolder("Assets", "OutputLogs");
        }

        logEnd = DateTime.Now.ToString();
        debugLog.Add("");
        debugLog.Add(">>  LOGGING ENDED: " + logEnd + "  <<");
        debugLog.Add("");

        string timestamp = logEnd.Replace(':', '-').Replace(' ', '_').Replace('/', '-');
        string filepath = directory + "/DebugOutput_" + timestamp + ".txt";

        File.WriteAllLines(filepath, debugLog);

        NewLog();
    }
#endif
}
