using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class Prompt : UI
{
    #region [ PROPERTIES ]

    [SerializeField] float minTextPadding = 30.0f;
    [SerializeField] bool twoParts = false;
    private GameObject[] parts = new GameObject[2] { null, null };
    private TextMeshProUGUI[] labels = new TextMeshProUGUI[2];
    private float[] defaultWidths = new float[3];
	
    [HideInInspector] public bool visible;

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void GetComponents()
    {
        if (twoParts)
        {
            
            if (transform.childCount < 1)
            {
                throw new Exception("Prompt \"" + gameObject.name + "\" has fewer than two child objects, but two parts are required when the \"twoParts\" attribute is set to true!");
            }
            else
            {
                if (transform.childCount > 2)
                {
                    Debug.LogWarning("Prompt \"" + gameObject.name + "\" has more than two child objects, but prompts can handle at most two parts.");
                }

                parts[0] = transform.GetChild(0).gameObject;
                defaultWidths[0] = parts[0].GetComponent<RectTransform>().rect.width;
                if (GetChildrenWithTag(parts[0], "Text").Count > 0)
                {
                    labels[0] = GetChildrenWithTag(parts[0], "Text")[0].GetComponent<TextMeshProUGUI>();
                }

                parts[1] = transform.GetChild(1).gameObject;
                defaultWidths[1] = parts[1].GetComponent<RectTransform>().rect.width;
                if (GetChildrenWithTag(parts[1], "Text").Count > 0)
                {
                    labels[1] = GetChildrenWithTag(parts[1], "Text")[0].GetComponent<TextMeshProUGUI>();
                }
            }
        }
        else
        {
            defaultWidths[0] = gameObject.GetComponent<RectTransform>().rect.width;
            if (GetChildrenWithTag(gameObject, "Text").Count > 0)
            {
                labels[0] = GetChildrenWithTag(gameObject, "Text")[0].GetComponent<TextMeshProUGUI>();
            }
        }
        defaultWidths[2] = gameObject.GetComponent<RectTransform>().rect.width;
    }

    public void SetText(string text, AdjustCondition adjustWidth)
    {
        SetText(0, text, adjustWidth);
    }

    public void SetText(int labelIndex, string text, AdjustCondition adjustWidth)
    {
        Vector2 mainSize = gameObject.GetComponent<RectTransform>().sizeDelta;
        Vector2 partSize = new Vector2();
        if (twoParts)
        {
            partSize = parts[labelIndex].GetComponent<RectTransform>().sizeDelta;
        }

        labels[labelIndex].text = text;

        if (adjustWidth != AdjustCondition.Never)
        {
            float newTextWidth = labels[labelIndex].GetPreferredValues().x;

            if (adjustWidth == AdjustCondition.Always)
            {
                if (twoParts)
                {
                    float newPartWidth = newTextWidth + minTextPadding;
                    float wDiff = newPartWidth - partSize.x;
                    mainSize.x += wDiff;
                    partSize.x = newPartWidth;
                }
                else
                {
                    mainSize.x = newTextWidth + minTextPadding;
                }
            }
            else if (adjustWidth == AdjustCondition.GreaterThan)
            {
                if (newTextWidth + minTextPadding > defaultWidths[0])
                {
                    float wDiff = newTextWidth + minTextPadding - defaultWidths[0];
                    if (twoParts)
                    {
                        partSize.x += wDiff;
                        mainSize.x = (defaultWidths[2] - defaultWidths[1] - defaultWidths[0]) + partSize.x + parts[ToInt(!ToBool(labelIndex))].GetComponent<RectTransform>().sizeDelta.x;
                    }
                    else
                    {
                        mainSize.x += wDiff;
                    }
                }
                else
                {
                    if (twoParts)
                    {
                        partSize.x = defaultWidths[labelIndex];
                        mainSize.x = (defaultWidths[2] - defaultWidths[1] - defaultWidths[0]) + partSize.x + parts[ToInt(!ToBool(labelIndex))].GetComponent<RectTransform>().sizeDelta.x;
                    }
                    else
                    {
                        mainSize.x = defaultWidths[2];
                    }
                }
            }

            gameObject.GetComponent<RectTransform>().sizeDelta = mainSize;
            if (twoParts)
            {
                parts[labelIndex].GetComponent<RectTransform>().sizeDelta = partSize;
            }
        }
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
        visible = show;
    }
}
