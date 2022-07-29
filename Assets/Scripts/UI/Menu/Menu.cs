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
    private List<UIElement> frames = new List<UIElement>();

    [HideInInspector] public int activeFrame = -1;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < menuFrames.Count; i++)
        {
            menuFrames[i].SetActive(true);
            frames.Add(GetOrAddComponent<UIElement>(menuFrames[i]));
        }
    }

    protected override void Start()
    {
        base.Start();
        SetActiveFrame(0);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void SetActiveFrame(int index)
    {
        if (InBounds(index, frames) && index != activeFrame)
        {
            for (int i = 0; i < frames.Count; i++)
            {
                frames[i].SetShown(i == index);
            }
            activeFrame = index;
        }
    }
    
    public void SetActiveFrame(UIElement menuFrame)
    {
        if (frames.Contains(menuFrame))
        {
            int index = frames.IndexOf(menuFrame);
            if (InBounds(index, frames) && index != activeFrame)
            {
                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].SetShown(i == index);
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
