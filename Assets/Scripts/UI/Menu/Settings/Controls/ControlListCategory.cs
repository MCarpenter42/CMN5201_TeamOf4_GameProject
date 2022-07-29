using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class ControlListCategory : UIElement
{
    #region [ PROPERTIES ]

    [HideInInspector] public MenuSettings_Controls parent;

    [Header("Components")]
    [SerializeField] TMP_Text header;
    [SerializeField] ControlListItem firstListItem;

    [HideInInspector] public string categoryName;
    [HideInInspector] public List<ControlListItem> categoryItems = new List<ControlListItem>();

    public float totalHeight
    {
        get {
            float h = rTransform.sizeDelta.y;
            for (int i = 0; i < categoryItems.Count; i++)
            {
                h += categoryItems[i].rTransform.sizeDelta.y;
            }
            return h;
        }
    }

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

    public void SetName(string name)
    {
        categoryName = name;
        header.text = categoryName;
    }

    public void CreateItems(List<string> itemTargets)
    {
        int n = itemTargets.Count;

        firstListItem.rTransform.SetParent(rTransform, false);
        categoryItems.Add(firstListItem);

        for (int i = 1; i < n; i++)
        {
            ControlListItem item = Instantiate(firstListItem, rTransform);
            categoryItems.Add(item);
        }

        for (int i = 0; i < categoryItems.Count; i++)
        {
            categoryItems[i].rTransform.anchorMin = new Vector2(0, 0);
            categoryItems[i].rTransform.anchorMax = new Vector2(1, 0);
            categoryItems[i].gameObject.name = "Ctrl_Item_" + categoryName + "_" + i;
            categoryItems[i].targetInput = itemTargets[i];
            categoryItems[i].SetName(Controls.GetControlByName(itemTargets[i]).ControlName);
            categoryItems[i].category = this;

            //Debug.Log(categoryName + ", " + i + ", " + itemTargets[i] + ", " + Controls.GetControlByName(itemTargets[i]).ControlName);
        }

        float posY = 0.0f;
        for (int i = 0; i < categoryItems.Count; i++)
        {
            categoryItems[i].rTransform.anchoredPosition = new Vector2(0.0f, posY);
            posY -= categoryItems[i].rTransform.sizeDelta.y;
        }

        if (parent != null)
        {
            parent.AddListItems(categoryItems);
        }
    }
}
