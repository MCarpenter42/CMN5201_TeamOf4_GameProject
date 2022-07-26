using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;

public class DeveloperConsole : Core
{
    #region [ PROPERTIES ]

    public enum CmdDataType
    {
        None,
        Bool,   BoolArray,
        Char,   CharArray,
        Float,  FloatArray,
        Int,    IntArray,
        String, StringArray
    };

    [Header("Components")]
    [SerializeField] RectTransform overlayRect;
    [HideInInspector] public UIElement console;
    [SerializeField] TMP_Text logReadout;
    [SerializeField] TMP_InputField inputBox;
    [SerializeField] GameObject showHideButton;

    [Header("Console Properties")]
    [SerializeField] float fixedShowHideDelay = 0.0f;
    [SerializeField] UIElement.ShowHide showHideType = UIElement.ShowHide.Slide;
    [SerializeField] float transitionTime = 0.1f;
    [SerializeField] InterpDelta.InterpTypes slideMovementStyle = InterpDelta.InterpTypes.Linear;
    [SerializeField] Vector2 visiblePos = Vector2.zero;
    [SerializeField] Vector2 hiddenOffset = Vector2.zero;

    [SerializeField] UnityEvent onShow = new UnityEvent();
    [SerializeField] UnityEvent onHide = new UnityEvent();

    private string[] logLines = new string[24];
    private bool logConsoleState = false;

    private List<ConsoleCommand> commands = new List<ConsoleCommand>();
    private List<string> commandPath = new List<string>();
    private ConsoleCommand targetCommand = null;
    private string[] allArgs = new string[0];
    private CmdDataType[] allArgTypes = new CmdDataType[0];
    private string[] cmdArgs = new string[0];
    private CmdDataType[] cmdArgTypes = new CmdDataType[0];

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
        DefineCommands();
    }

    void Start()
    {
        console.SetShown(false, UIElement.ShowHide.Instant);

        showHideButton.SetActive(false);
#if UNITY_EDITOR
        showHideButton.SetActive(true);
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (ParseCommandInput())
            {
                RunCommand();
                inputBox.text = "";
            }
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void GetComponents()
    {
        console = GetOrAddComponent<UIElement>(overlayRect.gameObject);
        console.SetShowHideTransition(fixedShowHideDelay, showHideType, transitionTime, slideMovementStyle);
        if (visiblePos != Vector2.zero)
        {
            console.SetShowHidePositions(hiddenOffset, visiblePos);
        }
        else
        {
            console.SetHiddenOffset(hiddenOffset);
        }
        if (onShow != null)
        {
            console.SetOnShowEvent(onShow);
        }
        if (onHide != null)
        {
            console.SetOnHideEvent(onHide);
        }
        console.GetGenericComponents();
    }

    private void DefineCommands()
    {
        ConsoleCommand echo = new ConsoleCommand("echo", new UnityEvent(EchoRaw));
        {
            ConsoleCommand echo_types = new ConsoleCommand("types", new UnityEvent(EchoTypes));
            echo.AddSubCommand(echo_types);
        }
        commands.Add(echo);

        ConsoleCommand time = new ConsoleCommand("time");
        {
            ConsoleCommand time_scale = new ConsoleCommand("scale");
            {
                ConsoleCommand time_scale_set = new ConsoleCommand("set", new UnityEvent_float(TimeScaleSet));
                time_scale.AddSubCommand(time_scale_set);
                ConsoleCommand time_scale_reset = new ConsoleCommand("reset", new UnityEvent(TimeScaleReset));
                time_scale.AddSubCommand(time_scale_reset);
            }
            time.AddSubCommand(time_scale);
        }
        commands.Add(time);
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    public void ToggleConsole()
    {
        if (!console.gameObject.activeSelf)
        {
            console.gameObject.SetActive(true);
        }
        console.SetShown(!console.visible, showHideType);
    }

    public void UpdateLog()
    {
        ClearArray(logLines);

        int c = DebugLogging.debugLog.Count;
        int l = logLines.Length;
        int n;
        if (c < l)
        {
            n = c;
        }
        else
        {
            n = l;
        }

        for (int i = 1; i <= n; i++)
        {
            logLines[l - i] = DebugLogging.debugLog[c - i];
        }

        string readout = "";
        for (int i = 0; i < l; i++)
        {
            if (i > 0)
            {
                readout += "\n";
            }
            readout += logLines[i];
        }
        logReadout.text = readout;
    }

    public void LogShowState(bool show)
    {
        if (logConsoleState)
        {
            if (show)
            {
                Debug.Log("Showing dev console...");
            }
            else
            {
                Debug.Log("Hiding dev console...");
            }
        }
        else
        {
            logConsoleState = true;
        }
    }

    public bool ParseCommandInput()
    {
        bool parseSuccessful = false;
        string input = inputBox.text;
        commandPath.Clear();

        if (char.Parse(input.Substring(0, 1)) == '/')
        {
            allArgs = input.Substring(1).Split(' ');
        }
        else
        {
            allArgs = input.Split(' ');
        }
        allArgTypes = new CmdDataType[allArgs.Length];

        for (int i = 0; i < allArgs.Length; i++)
        {
            try
            {
                float.Parse(allArgs[i]);
                if (allArgs[i].Contains("."))
                {
                    allArgTypes[i] = CmdDataType.Float;
                }
                else
                {
                    allArgTypes[i] = CmdDataType.Int;
                }
            }
            catch
            {
                if (allArgs[i].Length == 1)
                {
                    allArgTypes[i] = CmdDataType.Char;
                }
                else
                {
                    try
                    {
                        bool.Parse(allArgs[i]);
                        allArgTypes[i] = CmdDataType.Bool;
                    }
                    catch
                    {
                        allArgTypes[i] = CmdDataType.String;
                    }
                }
            }
        }

        int n = -1;
        for (int i = 0; i < commands.Count; i++)
        {
            if (allArgs[0] == commands[i].cmdString)
            {
                n = i;
                parseSuccessful = true;
                break;
            }
        }

        if (parseSuccessful)
        {
            int x = 0;
            targetCommand = commands[n];
            commandPath.Add(targetCommand.cmdString);
            bool keepChecking = false;

            for (int i = 1; i < allArgs.Length; i++)
            {
                x = i;
                if (targetCommand.subCommands.Count > 0)
                {
                    n = targetCommand.CheckForSubcommand(allArgs[i]);
                    if (n > -1)
                    {
                        targetCommand = targetCommand.subCommands[n];
                        commandPath.Add(targetCommand.cmdString);
                        keepChecking = true;
                    }
                }  

                if (!keepChecking)
                {
                    break;
                }
            }

            cmdArgs = new string[targetCommand.paramTypes.Length];
            cmdArgTypes = new CmdDataType[cmdArgs.Length];
            bool argsToStringArray = false;
            for (int i = 0; i < cmdArgs.Length; i++)
            {
                cmdArgs[i] = allArgs[i + x];
                cmdArgTypes[i] = allArgTypes[i + x];
                if (cmdArgTypes[i] != targetCommand.paramTypes[i] && targetCommand.paramTypes[i] != CmdDataType.None)
                {
                    /*if ((CmdDataType)((int)cmdArgTypes[i] + 1) != targetCommand.paramTypes[i])
                    {
                        parseSuccessful = false;
                        Debug.LogError("Unable to execute command - provided parameter " + i + " (" + cmdArgs[i] + ") does not match the required type of \"" + targetCommand.paramTypes[i] + "\"!");
                        break;
                    }*/
                    if (cmdArgTypes[i] == CmdDataType.Int && targetCommand.paramTypes[i] == CmdDataType.Float)
                    {
                        parseSuccessful = true;
                    }
                    else if (CmdDataType.StringArray != targetCommand.paramTypes[i])
                    {
                        parseSuccessful = false;
                        Debug.LogError("Unable to execute command - provided parameter " + i + " (" + cmdArgs[i] + ") does not match the required type of \"" + targetCommand.paramTypes[i] + "\"!");
                        break;
                    }
                    else
                    {
                        argsToStringArray = true;
                    }
                }
            }
            if (argsToStringArray)
            {
                cmdArgs = new string[allArgs.Length - x + 1];
                for (int i = 0; i < cmdArgs.Length; i++)
                {
                    cmdArgs[i] = allArgs[i + x];
                }
            }
        }
        else
        {
            cmdArgs = new string[0];
            Debug.LogError("Unable to execute command - text entered does not match a valid command!");
        }

        return parseSuccessful;
    }

    public bool RunCommand()
    {
        string commandPathReadout = "";
        for (int i = 0; i < commandPath.Count; i++)
        {
            if (i > 0)
            {
                commandPathReadout += ".";
            }
            commandPathReadout += commandPath[i];
        }
        GameManager.DebugLogging.LogCommand("Executing command: " + commandPathReadout);

        switch (targetCommand.paramTypes[0])
        {
            case CmdDataType.None:
                return targetCommand.Execute();

            case CmdDataType.Bool:
                return targetCommand.Execute(bool.Parse(cmdArgs[0]));
                
            case CmdDataType.Char:
                return targetCommand.Execute(char.Parse(cmdArgs[0]));
                
            case CmdDataType.Float:
                return targetCommand.Execute(float.Parse(cmdArgs[0]));
                
            case CmdDataType.Int:
                return targetCommand.Execute(int.Parse(cmdArgs[0]));

            default:
            case CmdDataType.String:
                return targetCommand.Execute(cmdArgs[0]);

            case CmdDataType.StringArray:
                return targetCommand.Execute(cmdArgs);
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ ECHO ]

    public void EchoRaw()
    {
        string echo = "";
        for (int i = 1; i < allArgs.Length; i++)
        {
            echo += allArgs[i] + " ";
        }

        GameManager.DebugLogging.SendLogMessage(echo, "InputEcho");
    }
    
    public void EchoTypes()
    {
        string echo = "";
        for (int i = 2; i < allArgTypes.Length; i++)
        {
            echo += allArgTypes[i] + " ";
        }

        GameManager.DebugLogging.SendLogMessage(echo, "InputTypesEcho");
    }

    #endregion

    #region [ TIME ]

    public void TimeScaleSet(float timeScale)
    {
        Time.timeScale = Mathf.Clamp(timeScale, 0.0f, float.MaxValue);
        GameManager.DebugLogging.SendLogMessage(("Setting relative time scale to " + timeScale), "TimeScale");
    }

    public void TimeScaleReset()
    {
        Time.timeScale = 1.0f;
        GameManager.DebugLogging.SendLogMessage(("Setting relative time scale to 1"), "TimeScale");
    }

    #endregion

}

public class ConsoleCommand
{
    public string cmdString;
    public string parentString;

    public DeveloperConsole.CmdDataType[] paramTypes = new DeveloperConsole.CmdDataType[1];

    public UnityEvent voidEvent;

    public UnityEvent_bool boolEvent;
    public UnityEvent_char charEvent;
    public UnityEvent_float floatEvent;
    public UnityEvent_int intEvent;
    public UnityEvent_string stringEvent;

    public UnityEvent_boolArray boolArrayEvent;
    public UnityEvent_charArray charArrayEvent;
    public UnityEvent_floatArray floatArrayEvent;
    public UnityEvent_intArray intArrayEvent;
    public UnityEvent_stringArray stringArrayEvent;

    public bool noEvents
    {
        get {
            bool single = (voidEvent == null && boolEvent == null && charEvent == null && floatEvent == null && intEvent == null && stringEvent == null);
            bool array = (boolArrayEvent == null && charArrayEvent == null && floatArrayEvent == null && intArrayEvent == null && stringArrayEvent == null);
            return (single && array);
        }
    }

    public bool ArrayEvent(int index)
    {
        return (paramTypes[index] == DeveloperConsole.CmdDataType.BoolArray || paramTypes[index] == DeveloperConsole.CmdDataType.Char || paramTypes[index] == DeveloperConsole.CmdDataType.FloatArray || paramTypes[index] == DeveloperConsole.CmdDataType.IntArray || paramTypes[index] == DeveloperConsole.CmdDataType.StringArray);
    }

    public List<ConsoleCommand> subCommands = new List<ConsoleCommand>();

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public ConsoleCommand(string cmdString)
    {
        this.cmdString = cmdString.ToLower();
    }
    
    public ConsoleCommand(string cmdString, UnityEvent executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        voidEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.None;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_bool executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        boolEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.Bool;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_char executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        charEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.Char;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_float executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        floatEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.Float;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_int executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        intEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.Int;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_string executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        stringEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.String;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_boolArray executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        boolArrayEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.BoolArray;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_charArray executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        charArrayEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.CharArray;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_floatArray executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        floatArrayEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.FloatArray;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_intArray executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        intArrayEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.IntArray;
    }
    
    public ConsoleCommand(string cmdString, UnityEvent_stringArray executeEvent)
    {
        this.cmdString = cmdString.ToLower();
        stringArrayEvent = executeEvent;
        paramTypes[0] = DeveloperConsole.CmdDataType.StringArray;
    }
    
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void AddSubCommand(ConsoleCommand subCmd)
    {
        subCommands.Add(subCmd);
        subCmd.parentString = parentString + cmdString + " ";
    }

    public int CheckForSubcommand(string subCmdString)
    {
        if (subCommands.Count > 0)
        {
            for (int i = 0; i < subCommands.Count; i++)
            {
                if (subCmdString == subCommands[i].cmdString)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public bool Execute()
    {
        bool executeSuccessful = false;

        if (voidEvent != null)
        {
            voidEvent.Invoke();
            executeSuccessful = true;
        }
        else
        {
            if (noEvents)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" cannot be used without a subcommand!");
            }
            else
            {
                if (parentString != null && parentString.Length > 0)
                {
                    Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" requires an argument input!");
                }
                else
                {
                    Debug.LogError("ERROR: Command \"" + cmdString + "\" requires an argument input!");
                }
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(bool b)
    {
        bool executeSuccessful = false;

        if (boolEvent != null)
        {
            boolEvent.Invoke(b);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" does not accept a boolean argument!");
            }
            else
            {
                Debug.LogError("ERROR: Command \"" + cmdString + "\" does not accept a boolean argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(char c)
    {
        bool executeSuccessful = false;

        if (charEvent != null)
        {
            charEvent.Invoke(c);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                throw new Exception("ERROR: Command \"" + parentString + cmdString + "\" does not accept a character argument!");
            }
            else
            {
                throw new Exception("ERROR: Command \"" + cmdString + "\" does not accept a character argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(float f)
    {
        bool executeSuccessful = false;

        if (floatEvent != null)
        {
            floatEvent.Invoke(f);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" does not accept a floating point numerical argument!");
            }
            else
            {
                Debug.LogError("ERROR: Command \"" + cmdString + "\" does not accept a floating point numerical argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(int i)
    {
        bool executeSuccessful = false;

        if (intEvent != null)
        {
            intEvent.Invoke(i);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" does not accept an integer numerical argument!");
            }
            else
            {
                Debug.LogError("ERROR: Command \"" + cmdString + "\" does not accept an integer numerical argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(string s)
    {
        bool executeSuccessful = false;

        if (stringEvent != null)
        {
            stringEvent.Invoke(s);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" does not accept a string argument!");
            }
            else
            {
                Debug.LogError("ERROR: Command \"" + cmdString + "\" does not accept a string argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(bool[] b)
    {
        bool executeSuccessful = false;

        if (boolArrayEvent != null)
        {
            boolArrayEvent.Invoke(b);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" does not accept a boolean array argument!");
            }
            else
            {
                Debug.LogError("ERROR: Command \"" + cmdString + "\" does not accept a boolean array argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(char[] c)
    {
        bool executeSuccessful = false;

        if (charArrayEvent != null)
        {
            charArrayEvent.Invoke(c);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                throw new Exception("ERROR: Command \"" + parentString + cmdString + "\" does not accept a character array argument!");
            }
            else
            {
                throw new Exception("ERROR: Command \"" + cmdString + "\" does not accept a character array argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(float[] f)
    {
        bool executeSuccessful = false;

        if (floatArrayEvent != null)
        {
            floatArrayEvent.Invoke(f);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" does not accept a floating point number array argument!");
            }
            else
            {
                Debug.LogError("ERROR: Command \"" + cmdString + "\" does not accept a floating point number array argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(int[] i)
    {
        bool executeSuccessful = false;

        if (intArrayEvent != null)
        {
            intArrayEvent.Invoke(i);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" does not accept an integer array argument!");
            }
            else
            {
                Debug.LogError("ERROR: Command \"" + cmdString + "\" does not accept an integer array argument!");
            }
        }

        return executeSuccessful;
    }
    
    public bool Execute(string[] s)
    {
        bool executeSuccessful = false;

        if (stringArrayEvent != null)
        {
            stringArrayEvent.Invoke(s);
            executeSuccessful = true;
        }
        else
        {
            if (parentString != null && parentString.Length > 0)
            {
                Debug.LogError("ERROR: Command \"" + parentString + cmdString + "\" does not accept a string array argument!");
            }
            else
            {
                Debug.LogError("ERROR: Command \"" + cmdString + "\" does not accept a string array argument!");
            }
        }

        return executeSuccessful;
    }
}
