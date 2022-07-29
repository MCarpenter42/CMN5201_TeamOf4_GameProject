using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class MenuSettings_Controls : Menu
{
    #region [ OBJECTS ]

    [Header("Components")]
    [SerializeField] RectTransform scrollContent;
    [SerializeField] ControlListCategory firstCategory;

    [HideInInspector] public List<ControlListCategory> controlCategories = new List<ControlListCategory>();
    [HideInInspector] public List<ControlListItem> controlListItems = new List<ControlListItem>();

    #endregion

    #region [ PROPERTIES ]

    private List<string> categoryNames = new List<string>();
    private List<string> controlNames = new List<string>();

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        base.Awake();
        GetComponents();
        CreateCategories();
        UpdateAllLabels();
    }

    protected override void Start()
    {
        base.Start();
        UpdateAllLabels();
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

    #region [ SETUP ]

    private void GetComponents()
    {
        categoryNames = Controls.categoryNames;
        controlNames = Controls.controlNames;
    }

    private void CreateCategories()
    {
        firstCategory.gameObject.name = "Ctrl_Cat_" + categoryNames[0];
        firstCategory.SetName(categoryNames[0]);
        firstCategory.gameObject.transform.SetParent(scrollContent);
        firstCategory.parent = this;
        controlCategories.Add(firstCategory);

        for (int i = 1; i < categoryNames.Count; i++)
        {
            ControlListCategory category = Instantiate(firstCategory, scrollContent);
            category.gameObject.name = "Ctrl_Cat_" + categoryNames[i];
            category.SetName(categoryNames[i]);
            category.parent = this;
            controlCategories.Add(category);
        }

        float posY = 0.0f;

        for (int i = 0; i < controlCategories.Count; i++)
        {
            controlCategories[i].rTransform.anchoredPosition = new Vector2(0.0f, posY);

            string category = controlCategories[i].categoryName;
            List<string> itemTargets = new List<string>();

            for (int j = 0; j < controlNames.Count; j++)
            {
                if (controlNames[j].Split('.')[1] == category)
                {
                    itemTargets.Add(controlNames[j]);
                }
            }

            controlCategories[i].CreateItems(itemTargets);

            posY -= controlCategories[i].totalHeight;
        }

        float scrollContentHeight = 0.0f;
        for (int i = 0; i < controlCategories.Count; i++)
        {
            scrollContentHeight += controlCategories[i].totalHeight;
        }
        scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, scrollContentHeight);
    }

    public void AddListItems(List<ControlListItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].index = controlListItems.Count;
            controlListItems.Add(items[i]);
        }
    }

    #endregion

    #region [ SET CONTROL ]

    public void SetControl(ControlListItem trigger)
    {
        StartCoroutine(ISetControl(trigger));
    }

    public IEnumerator ISetControl(ControlListItem trigger)
    {
        trigger.SetLabel("...");

        KeyCode pressedKey = KeyCode.None;
        bool setNewKey = true;

        while (true)
        {
            foreach (KeyValuePair<KeyCode, string> kvp in Controls.KeyNames)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    pressedKey = kvp.Key;
                    if (kvp.Key == KeyCode.Escape)
                    {
                        setNewKey = false;
                    }
                    break;
                }
            }

            if (pressedKey != KeyCode.None)
            {
                break;
            }

            yield return null;
        }
        
        if (setNewKey)
        {
            int n = Controls.controlNames.IndexOf(trigger.targetInput);

            for (int i = 0; i < Controls.controlNames.Count; i++)
            {
                KeyCode control = Controls.GetControlByName(Controls.controlNames[i]).Key;

                if (pressedKey == control && Controls.controlNames[i] != trigger.targetInput)
                {
                    Controls.SetControlByName(Controls.controlNames[i], KeyCode.None);
                    controlListItems[i].UpdateLabel();
                }
            }

            Controls.SetControlByName(trigger.targetInput, pressedKey);
            Debug.Log(pressedKey + ", " + trigger.targetInput + ", " + Controls.GetControlByName(trigger.targetInput).Key);

            GameManager.Instance.SaveControls();
        }

        trigger.UpdateLabel();
    }

    #endregion

    public void ResetControls()
    {
        GameManager.Instance.ResetControls();
        UpdateAllLabels();
    }

    public void UpdateAllLabels()
    {
        foreach (ControlListItem item in controlListItems)
        {
            item.UpdateLabel();
        }
    }
}
