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

    [SerializeField] GameObject controlCategoryPrefab;
    [SerializeField] GameObject controlListItemPrefab;

    private List<GameObject> controlCategories = new List<GameObject>();
    private List<GameObject> controlListItems = new List<GameObject>();

    #endregion

    #region [ PROPERTIES ]

    private List<string> categoryNames = new List<string>();
    private List<string> controlNames = new List<string>();

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {

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

    private void CreateListItems()
    {
        for (int i = 0; i < categoryNames.Count; i++)
        {
            GameObject category = Instantiate(controlCategoryPrefab, transform);
            category.name = categoryNames[i];
            controlCategories.Add(category);
        }

        for (int i = 0; i < controlNames.Count; i++)
        {
            string[] itemName = controlNames[i].Split(new char['.']);
            string itemCategory = itemName[itemName.Length - 2];
            int categoryIndex = categoryNames.IndexOf(itemCategory);
            GameObject listItem = Instantiate(controlListItemPrefab, controlCategories[categoryIndex].transform);
            controlListItems.Add(listItem);
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
                if (kvp.Key == KeyCode.Escape)
                {
                    setNewKey = false;
                    break;
                }
                else
                {
                    pressedKey = kvp.Key;
                }
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
