using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class Menu : UIElement
{
    #region [ PROPERTIES ]

    [Header("Components")]
    public List<GameObject> menuFrames;

    [HideInInspector] public int activeFrame = -1;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        SetActiveFrame(0);
    }

    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void SetActiveFrame(int index)
    {
        if (InBounds(index, menuFrames) && index != activeFrame)
        {
            for (int i = 0; i < menuFrames.Count; i++)
            {
                menuFrames[i].SetActive(i == index);
            }
            activeFrame = index;
        }
    }
    
    public void SetActiveFrame(GameObject menuFrame)
    {
        if (menuFrames.Contains(menuFrame))
        {
            int index = menuFrames.IndexOf(menuFrame);
            if (InBounds(index, menuFrames) && index != activeFrame)
            {
                for (int i = 0; i < menuFrames.Count; i++)
                {
                    menuFrames[i].SetActive(i == index);
                }
                activeFrame = index;
            }
        }
        else
        {
            throw new Exception("ERROR: Object \"" + menuFrame.name + "\" is not a valid frame of menu \"" + gameObject.name + "\"!");
        }
    }
    
}
