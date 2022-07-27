using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class DropdownMenu : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    #region [ PROPERTIES ]
    
    [Header("Components")] 
    [SerializeField] RectTransform scrollViewContent;
    [SerializeField] IntButton firstMenuButton;
    [HideInInspector] public List<IntButton> menuButtons;
    [HideInInspector] public DropdownButton mainButton;

    [Header("Menu Properties")]
    public List<string> menuOptions;
    [HideInInspector] public int selOption;
    [SerializeField] Vector2 buttonSpacing = Vector2.zero;
    [SerializeField] int maxButtonsDisplayed = 0;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        if (Application.isPlaying)
        {
            base.Awake();
            Setup();
        }
    }

#if UNITY_EDITOR
    protected override void Update()
    {
        if (!Application.isPlaying)
        {
            if (menuOptions.Count == 0)
            {
                menuOptions.Add("<option text>");
            }
            firstMenuButton.SetLabel(menuOptions[0]);
        }
    }
#endif

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (Application.isPlaying)
        {
            StartCoroutine(IDelayedShow(false));
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    private void Setup()
    {
        RectTransform rTran = firstMenuButton.gameObject.GetComponent<RectTransform>();
        float btnH = rTran.sizeDelta.y;
        float posY = -buttonSpacing.y;

        rTran.SetParent(scrollViewContent, false);
        firstMenuButton.gameObject.name = "DDMenuBtn_0";
        rTran.anchoredPosition = new Vector2(0.0f, posY);
        firstMenuButton.invokeWith = 0;
        firstMenuButton.intEvent.AddListener(OnButtonClicked);
        firstMenuButton.SetLabel(menuOptions[0]);
        menuButtons.Add(firstMenuButton);

        for (int i = 1; i < menuOptions.Count; i++)
        {
            posY -= (btnH + 2.0f * buttonSpacing.y);

            IntButton btn = Instantiate(firstMenuButton, scrollViewContent);
            btn.gameObject.name = "DDMenuBtn_" + i;
            btn.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, posY);
            btn.invokeWith = i;
            btn.intEvent.AddListener(OnButtonClicked);
            btn.SetLabel(menuOptions[i]);
            menuButtons.Add(btn);
        }

        float h = Mathf.Abs(posY) + btnH + buttonSpacing.y;
        scrollViewContent.sizeDelta = new Vector2(scrollViewContent.sizeDelta.x, h);

        int n = menuOptions.Count;
        if (maxButtonsDisplayed > 0)
        {
            n = maxButtonsDisplayed;
        }
        h = (float)n * (btnH + 2.0f * buttonSpacing.y) + 4.0f;
        rTransform.sizeDelta = new Vector2(rTransform.sizeDelta.x, h);
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void OnButtonClicked(int btnInt)
    {
        if (InBounds(btnInt, menuOptions))
        {
            selOption = btnInt;
            mainButton.UpdateSelection(btnInt);
        }
    }

    private IEnumerator IDelayedShow(bool show)
    {
        yield return null;
        if (mainButton != null)
        {
            if (!mainButton.mouseOver)
            {
                SetShown(show);
            }
        }
        else
        {
            SetShown(show);
        }
    }

    private IEnumerator IDelayedShow(bool show, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (mainButton != null)
        {
            if (!mainButton.mouseOver)
            {
                SetShown(show);
            }
        }
        else
        {
            SetShown(show);
        }
    }
}
