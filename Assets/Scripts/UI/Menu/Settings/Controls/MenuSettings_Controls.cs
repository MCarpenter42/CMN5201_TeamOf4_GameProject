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

    [SerializeField] GameObject controlCategoryPrefab;
    [SerializeField] GameObject controlListItemPrefab;

    private List<ControlListCategory> controlCategories = new List<ControlListCategory>();

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
    }

    protected override void Start()
    {
        base.Start();
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
        for (int i = 0; i < Controls.categoryCount; i++)
        {
            string categoryName = Controls.GetType().GetProperties()[i].Name;
            categoryNames.Add(categoryName);
        }
        controlNames = Controls.GetNamesList();
    }

    private void CreateCategories()
    {
        firstCategory.gameObject.name = "Ctrl_Cat_" + Controls.categoryNames[0];
        firstCategory.SetName(Controls.categoryNames[0]);
        firstCategory.gameObject.transform.SetParent(scrollContent);
        controlCategories.Add(firstCategory);
        for (int i = 1; i < Controls.categoryCount; i++)
        {
            ControlListCategory category = Instantiate(firstCategory, scrollContent);
            category.gameObject.name = "Ctrl_Cat_" + Controls.categoryNames[i];
            category.SetName(Controls.categoryNames[i]);
            controlCategories.Add(category);
        }

        float posY = 0.0f;
        List<string> controls = Controls.GetNamesList();
        for (int i = 0; i < controlCategories.Count; i++)
        {
            controlCategories[i].rTransform.anchoredPosition = new Vector2(0.0f, posY);

            string category = controlCategories[i].categoryName;
            List<KeyValuePair<int, string>> itemProperties = new List<KeyValuePair<int, string>>();

            for (int j = 0; j < controls.Count; i++)
            {
                if (controls[j].Split('.')[1] == category)
                {
                    itemProperties.Add(new KeyValuePair<int, string>(j, controls[j]));
                }
            }

            controlCategories[i].CreateItems(itemProperties);

            posY -= controlCategories[i].totalHeight;
        }

        float scrollContentHeight = 0.0f;
        for (int i = 0; i < controlCategories.Count; i++)
        {
            scrollContentHeight += controlCategories[i].totalHeight;
        }
    }

    #endregion

    public void UpdateControlsList()
    {

    }

    #region [ SET CONTROL ]

    public void SetControl(ControlInput targetInput)
    {
        StartCoroutine(ListenForKeypress(targetInput));
    }

    public IEnumerator ListenForKeypress(ControlInput targetInput)
    {
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

            yield return new WaitForEndOfFrame();
        }

        if (setNewKey)
        {
            for (int i = 0; i < controlNames.Count; i++)
            {
                KeyCode control = Controls.GetControlByName(controlNames[i]);

                if (pressedKey == control)
                {
                    Controls.SetControlByName(controlNames[i], KeyCode.None);
                }
            }

            targetInput.Key = pressedKey;
        }

        UpdateControlsList();
    }

    #endregion

}
