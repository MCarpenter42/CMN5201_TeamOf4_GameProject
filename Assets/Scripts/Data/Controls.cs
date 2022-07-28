using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* >> ADDING A NEW CONTROL <<

    If you need to add a control that doesn't exist yet, and it fits
    an existing category, then the process is simple! Add a new public
    ControlInput property to the relevant category class, and use its
    constructor to assign an appropriate display name and default
    keybind to it!

    If the new control doesn't fit into any of the existing categories,
    follow the instructions for adding a category first, then come back
    and follow the instructions for adding a control!

*/

/* >> ADDING A NEW CATEGORY <<

    If you want to create a new category of controls, then you'll want
    to start by creating a new public class - for the sake of
    consistency and readability, make sure its name starts with
    "Controls_". Once the class is created, make sure to go and add
    an instance of it to the main Controls class, and add it directly
    after the other category instances.

    It's very important that instances of new categories are placed
    directly after the existing ones, and before the main Controls
    class' other properties, as one of the methods it contains
    checks how many categories exist based on a loop that stops
    when it reaches a property of the "int" type, so messing with
    the layout breaks this method.

*/

public class Controls : Core
{
	public Controls_General General = new Controls_General();
    public Controls_Movement Movement = new Controls_Movement();
    public Controls_Interaction Interaction = new Controls_Interaction();
    public Controls_Camera Camera = new Controls_Camera();

    public int categoryCount = 4;
    /*{
        get {
            int count = 0;
            PropertyInfo[] properties = typeof(Controls).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                IEnumerable<System.Attribute> attr = property.GetCustomAttributes();
                Debug.Log(attr.GetEnumerator());
            }

            for (int i = 0; i < properties.Length; i++)
            {
                string typeName = GetType().GetProperties()[i].ToString();
                if (typeName.Contains("Controls_"))
                {
                    count++;
                }
            }
            return count;
        }
    }*/
    public List<string> categoryNames = new List<string>() { "General", "Movement", "Interaction", "Camera" };
    public List<string> controlNames = new List<string>()
    {
        "Controls.General.Pause",
        "Controls.General.ResetLevel",
        "Controls.Movement.Up",
        "Controls.Movement.Down",
        "Controls.Movement.Left",
        "Controls.Movement.Right",
        "Controls.Interaction.Interact",
        "Controls.Interaction.RotateCounterClockwise",
        "Controls.Interaction.RotateClockwise",
        "Controls.Camera.RotCamLeft",
        "Controls.Camera.RotCamRight",
        "Controls.Camera.ZoomCamIn",
        "Controls.Camera.ZoomCamOut"
    };

	public Dictionary<KeyCode, string> KeyNames = new Dictionary<KeyCode, string>
    {
        // JOYSTICK BUTTONS HAVE BEEN TEMPORARILY EXCLUDED
        // UNITY DOCUMENTATION SUGGESTS HANDLING THEM THROUGH A DIFFERENT MEANS
            // >> SEE https://docs.unity3d.com/ScriptReference/KeyCode.html

        // ALPHANUMERIC
        { KeyCode.A, "A" },
        { KeyCode.B, "B" },
        { KeyCode.C, "C" },
        { KeyCode.D, "D" },
        { KeyCode.E, "E" },
        { KeyCode.F, "F" },
        { KeyCode.G, "G" },
        { KeyCode.H, "H" },
        { KeyCode.I, "I" },
        { KeyCode.J, "J" },
        { KeyCode.K, "K" },
        { KeyCode.L, "L" },
        { KeyCode.M, "M" },
        { KeyCode.N, "N" },
        { KeyCode.O, "O" },
        { KeyCode.P, "P" },
        { KeyCode.Q, "Q" },
        { KeyCode.R, "R" },
        { KeyCode.S, "S" },
        { KeyCode.T, "T" },
        { KeyCode.U, "U" },
        { KeyCode.V, "V" },
        { KeyCode.W, "W" },
        { KeyCode.X, "X" },
        { KeyCode.Y, "Y" },
        { KeyCode.Z, "Z" },
        { KeyCode.Alpha0, "0" },
        { KeyCode.Alpha1, "1" },
        { KeyCode.Alpha2, "2" },
        { KeyCode.Alpha3, "3" },
        { KeyCode.Alpha4, "4" },
        { KeyCode.Alpha5, "5" },
        { KeyCode.Alpha6, "6" },
        { KeyCode.Alpha7, "7" },
        { KeyCode.Alpha8, "8" },
        { KeyCode.Alpha9, "9" },

        // SYMBOLS
        { KeyCode.Ampersand, "&" },
        { KeyCode.Asterisk, "*" },
        { KeyCode.At, "@" },
        { KeyCode.BackQuote, "`" },
        { KeyCode.Backslash, "\\" },
        { KeyCode.Caret, "^" },
        { KeyCode.Colon, ":" },
        { KeyCode.Comma, "," },
        { KeyCode.Dollar, "$" },
        { KeyCode.DoubleQuote, "\"" },
        { KeyCode.Equals, "=" },
        { KeyCode.Exclaim, "!" },
        { KeyCode.Greater, ">" },
        { KeyCode.Hash, "#" },
        { KeyCode.LeftBracket, "[" },
        { KeyCode.LeftCurlyBracket, "{" },
        { KeyCode.LeftParen, "(" },
        { KeyCode.Less, "<" },
        { KeyCode.Minus, "-" },
        { KeyCode.Percent, "%" },
        { KeyCode.Period, "." },
        { KeyCode.Pipe, "|" },
        { KeyCode.Plus, "+" },
        { KeyCode.Question, "?" },
        { KeyCode.Quote, "'" },
        { KeyCode.RightBracket, "]" },
        { KeyCode.RightCurlyBracket, "}" },
        { KeyCode.RightParen, ")" },
        { KeyCode.Semicolon, ";" },
        { KeyCode.Slash, "/" },
        { KeyCode.Tilde, "~" },
        { KeyCode.Underscore, "_" },

        // ARROW KEYS
        { KeyCode.DownArrow, "DOWN" },
        { KeyCode.LeftArrow, "LEFT" },
        { KeyCode.RightArrow, "RIGHT" },
        { KeyCode.UpArrow, "UP" },

        // NUMPAD KEYS
        { KeyCode.Keypad0, "Numpad 0" },
        { KeyCode.Keypad1, "Numpad 1" },
        { KeyCode.Keypad2, "Numpad 2" },
        { KeyCode.Keypad3, "Numpad 3" },
        { KeyCode.Keypad4, "Numpad 4" },
        { KeyCode.Keypad5, "Numpad 5" },
        { KeyCode.Keypad6, "Numpad 6" },
        { KeyCode.Keypad7, "Numpad 7" },
        { KeyCode.Keypad8, "Numpad 8" },
        { KeyCode.Keypad9, "Numpad 9" },
        { KeyCode.KeypadDivide, "Numpad /" },
        { KeyCode.KeypadEnter, "Numpad Enter" },
        { KeyCode.KeypadEquals, "Numpad =" },
        { KeyCode.KeypadMinus, "Numpad -" },
        { KeyCode.KeypadMultiply, "Numpad *" },
        { KeyCode.KeypadPeriod, "Numpad ." },
        { KeyCode.KeypadPlus, "Numpad +" },

        // CONTROL KEYS
        { KeyCode.AltGr, "Alt Gr" },
        { KeyCode.Backspace, "Backspace" },
        { KeyCode.Break, "Break" },
        { KeyCode.Clear, "Clear" },
        { KeyCode.Delete, "Delete" },
        { KeyCode.End, "End" },
        { KeyCode.Escape, "Esc" },
        { KeyCode.Help, "Help" },
        { KeyCode.Home, "Home" },
        { KeyCode.Insert, "Insert" },
        { KeyCode.LeftAlt, "Left Alt" },
            //{ KeyCode.LeftApple, "Left Command" },
        { KeyCode.LeftCommand, "Left Command" },
        { KeyCode.LeftControl, "Left Ctrl" },
        { KeyCode.LeftShift, "Left Shift" },
        { KeyCode.LeftWindows, "Left Windows" },
        { KeyCode.Menu, "Menu" },
        { KeyCode.PageDown, "Page Down" },
        { KeyCode.PageUp, "Page Up" },
        { KeyCode.Pause, "Pause" },
        { KeyCode.Print, "Print Screen" },
        { KeyCode.Return, "Enter" },
        { KeyCode.RightAlt, "Right Alt" },
            //{ KeyCode.RightApple, "Right Command" },
        { KeyCode.RightCommand, "Right Command" },
        { KeyCode.RightControl, "Right Ctrl" },
        { KeyCode.RightShift, "Right Shift" },
        { KeyCode.RightWindows, "Right Windows" },
        { KeyCode.Space, "Space" },
        { KeyCode.SysReq, "Sys Req" },
        { KeyCode.Tab, "Tab" },

        // LOCK KEYS
        { KeyCode.CapsLock, "Caps Lock" },
        { KeyCode.Numlock, "Num Lock" },
        { KeyCode.ScrollLock, "Scroll Lock" },

        // FUNCTION KEYS
        { KeyCode.F1, "F1" },
        { KeyCode.F2, "F2" },
        { KeyCode.F3, "F3" },
        { KeyCode.F4, "F4" },
        { KeyCode.F5, "F5" },
        { KeyCode.F6, "F6" },
        { KeyCode.F7, "F7" },
        { KeyCode.F8, "F8" },
        { KeyCode.F9, "F9" },
        { KeyCode.F10, "F10" },
        { KeyCode.F11, "F11" },
        { KeyCode.F12, "F12" },
        { KeyCode.F13, "F13" },
        { KeyCode.F14, "F14" },
        { KeyCode.F15, "F15" },

        // MOUSE BUTTONS
        { KeyCode.Mouse0, "Left Mouse Button" },
        { KeyCode.Mouse1, "Right Mouse Button" },
        { KeyCode.Mouse2, "Middle Mouse Button" },
        { KeyCode.Mouse3, "Mouse Button 4" },
        { KeyCode.Mouse4, "Mouse Button 5" },
        { KeyCode.Mouse5, "Mouse Button 6" },
        { KeyCode.Mouse6, "Mouse Button 7" },

        // NO KEY
        { KeyCode.None, "[ No Key ]" },
    };

    public List<string> GetNamesList()
    {
        /*for (int i = 0; i < GetType().GetProperties().Length; i++)
        {
            if (GetType().GetProperties()[i].PropertyType != typeof(int))
            {
                categoryCount++;
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < categoryCount; i++)
        {
            string categoryName = "Controls.";
            categoryName += GetType().GetProperties()[i].Name;
            categoryNames.Add(categoryName);
        }*/

        /*for (int i = 0; i < GetType().GetProperties().Length; i++)
        {
            System.Type propertyType = GetType().GetProperties()[i].PropertyType;
            string typeName = propertyType.ToString();
            if (typeName.Contains("Controls_"))
            {
                string categoryName = "Controls." + GetType().GetProperties()[i].Name;
                categoryNames.Add(categoryName);
            }
        }*/

        /*foreach (string categoryName in categoryNames)
        {
            object category = GetProperty(this, categoryName);
            int n = category.GetType().GetProperties().Length;
            for (int i = 0; i < n; i++)
            {
                string controlName = categoryName + ".";
                controlName += category.GetType().GetProperties()[i].Name;
                controlNames.Add(controlName);
            }
        }*/

        /*for (int i = 0; i < 4; i++)
        {
            object category = GetProperty(this, categoryNames[i]);
            int n = category.GetType().GetProperties().Length;
            for (int j = 0; j < n; j++)
            {
                string controlName = categoryNames[i] + ".";
                controlName += category.GetType().GetProperties()[i].Name;
                controlNames.Add(controlName);
            }
        }*/

        return controlNames;
    }

    /*public KeyCode GetControlByName(string controlName)
    {
        string[] nameParts = controlName.Split('.');
        object category = GetProperty(this, nameParts[1]);
        ControlInput control = (ControlInput)GetProperty(category, nameParts[2]);

        return control.Key;
    }*/
    
    public KeyCode GetControlByName(string controlName)
    {
        string[] nameParts = controlName.Split('.');
        ControlInput control;

        List<string> inCategory = new List<string>();
        for (int i = 0; i < controlNames.Count; i++)
        {
            string cat = controlNames[i].Split('.')[1];
            if (nameParts[1] == cat)
            {
                inCategory.Add(controlNames[i]);
            }
        }

        int index = 0;
        for (int i = 0; i < inCategory.Count; i++)
        {
            string ctrl = controlNames[i].Split('.')[2];
            if (nameParts[2] == ctrl)
            {
                index = i;
                break;
            }
        }

        switch (nameParts[1])
        {
            default:
            case "General":
                control = General.controls[index];
                break;

            case "Movement":
                control = Movement.controls[index];
                break;

            case "Interaction":
                control = Interaction.controls[index];
                break;

            case "Camera":
                control = Camera.controls[index];
                break;
        }

        return control.Key;
    }
    
    public string GetControlDisplayName(string controlName)
    {
        string[] nameParts = controlName.Split('.');
        ControlInput control;

        List<string> inCategory = new List<string>();
        for (int i = 0; i < controlNames.Count; i++)
        {
            string cat = controlNames[i].Split('.')[1];
            if (nameParts[1] == cat)
            {
                inCategory.Add(controlNames[i]);
            }
        }

        int index = 0;
        for (int i = 0; i < inCategory.Count; i++)
        {
            string ctrl = controlNames[i].Split('.')[2];
            if (nameParts[2] == ctrl)
            {
                index = i;
                break;
            }
        }

        switch (nameParts[1])
        {
            default:
            case "General":
                control = General.controls[index];
                break;

            case "Movement":
                control = Movement.controls[index];
                break;

            case "Interaction":
                control = Interaction.controls[index];
                break;

            case "Camera":
                control = Camera.controls[index];
                break;
        }

        return control.ControlName;
    }

    public void SetControlByName(string controlName, KeyCode key)
    {
        string[] nameParts = controlName.Split('.');
        ControlInput control;

        List<string> inCategory = new List<string>();
        for (int i = 0; i < controlNames.Count; i++)
        {
            string cat = controlNames[i].Split('.')[1];
            if (nameParts[1] == cat)
            {
                inCategory.Add(controlNames[i]);
            }
        }

        int index = 0;
        for (int i = 0; i < inCategory.Count; i++)
        {
            string ctrl = controlNames[i].Split('.')[2];
            if (nameParts[2] == ctrl)
            {
                index = i;
                break;
            }
        }

        switch (nameParts[1])
        {
            default:
            case "General":
                control = General.controls[index];
                break;

            case "Movement":
                control = Movement.controls[index];
                break;

            case "Interaction":
                control = Interaction.controls[index];
                break;

            case "Camera":
                control = Camera.controls[index];
                break;
        }

        control.Key = key;
    }
}

public class Controls_General
{
    public ControlInput Pause = new ControlInput("Pause Game", KeyCode.Escape);
	public ControlInput ResetLevel = new ControlInput("Reset Level", KeyCode.Backspace);
    public ControlInput[] controls;

    public Controls_General()
    {
        controls = new ControlInput[] { Pause, ResetLevel };
    }
}

public class Controls_Movement
{
	public ControlInput Up = new ControlInput("Move Up", KeyCode.W);
	public ControlInput Down = new ControlInput("Move Down", KeyCode.S);
	public ControlInput Left = new ControlInput("Move Left", KeyCode.A);
	public ControlInput Right = new ControlInput("Move Right", KeyCode.D);
    public ControlInput[] controls;

    public Controls_Movement()
    {
        controls = new ControlInput[] { Up, Down, Left, Right };
    }
}

public class Controls_Interaction
{
	public ControlInput Interact = new ControlInput("Interact With Object", KeyCode.Space);
	public ControlInput RotateCounterClockwise = new ControlInput("Activate", KeyCode.Q);
	public ControlInput RotateClockwise = new ControlInput("Activate", KeyCode.E);
    public ControlInput[] controls;

    public Controls_Interaction()
    {
        controls = new ControlInput[] { Interact, RotateCounterClockwise, RotateClockwise };
    }
}

public class Controls_Camera
{
    public ControlInput RotCamLeft = new ControlInput("Rotate Camera Left", KeyCode.LeftArrow);
    public ControlInput RotCamRight = new ControlInput("Rotate Camera Right", KeyCode.RightArrow);
    public ControlInput ZoomCamIn = new ControlInput("Zoom Camera In", KeyCode.R);
    public ControlInput ZoomCamOut = new ControlInput("Zoom Camera Out", KeyCode.F);
    public ControlInput[] controls;

    public Controls_Camera()
    {
        controls = new ControlInput[] { RotCamLeft, RotCamRight, ZoomCamIn, ZoomCamOut };
    }
}



public class ControlInput
{
    public string ControlName;
    public KeyCode Key;

    public ControlInput(string controlName, KeyCode defaultKey)
    {
        ControlName = controlName;
        Key = defaultKey;
    }
}
