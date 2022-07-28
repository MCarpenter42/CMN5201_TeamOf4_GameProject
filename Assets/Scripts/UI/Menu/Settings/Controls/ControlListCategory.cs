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

    #region [ SETUP ]

    public void SetName(string name)
    {
        categoryName = name;
        header.text = categoryName;
    }

    public void CreateItems(List<KeyValuePair<int, string>> itemProperties)
    {
        //string[] itemName = controlNames[i].Split(new char['.']);
        //string itemCategory = itemName[itemName.Length - 2];

        int n = itemProperties.Count;

        firstListItem.rTransform.SetParent(rTransform, false);
        firstListItem.rTransform.anchorMin = new Vector2(0, 0);
        firstListItem.rTransform.anchorMax = new Vector2(1, 0);
        firstListItem.gameObject.name = "Ctrl_Item_" + categoryName + "_0";
        firstListItem.index = itemProperties[0].Key;
        firstListItem.targetInput = itemProperties[0].Value;
        firstListItem.SetName(Controls.GetControlDisplayName(itemProperties[0].Value));
        categoryItems.Add(firstListItem);

        for (int i = 1; i < n; i++)
        {
            ControlListItem item = Instantiate(firstListItem, rTransform);
            item.rTransform.anchorMin = new Vector2(0, 0);
            item.rTransform.anchorMax = new Vector2(1, 0);
            item.gameObject.name = "Ctrl_Item_" + categoryName + "_" + i;
            item.index = itemProperties[i].Key;
            item.targetInput = itemProperties[i].Value;
            item.SetName(Controls.GetControlDisplayName(itemProperties[i].Value));
            categoryItems.Add(item);
        }

        float posY = -rTransform.sizeDelta.y;
        for (int i = 0; i < categoryItems.Count; i++)
        {
            categoryItems[i].rTransform.anchoredPosition = new Vector2(0.0f, posY);
            posY -= categoryItems[i].rTransform.sizeDelta.y;
        }
    }

    #endregion
}
