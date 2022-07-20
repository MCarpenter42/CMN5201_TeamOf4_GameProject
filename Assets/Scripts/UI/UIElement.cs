using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class UIElement : Core, IPointerEnterHandler, IPointerExitHandler
{
    #region [ PROPERTIES ]

    public enum ShowHide { Instant, Fade, Slide };

    [HideInInspector] public RectTransform rTransform;
    [HideInInspector] public List<Graphic> childGraphicElements = new List<Graphic>();

    [HideInInspector] public bool visible;

    [Header("UI Element Properties")]
    public float fixedShowHideDelay = 0.0f;
    [SerializeField] protected ShowHide showHideType = ShowHide.Instant;
    [SerializeField] protected float transitionTime = 0.1f;
    public Vector2 hiddenOffset = Vector2.zero;
    [SerializeField] protected InterpDelta.InterpTypes slideMovementStyle = InterpDelta.InterpTypes.Linear;
    public Vector2 visiblePos;
    protected bool isMoving = false;
    public float opacity = 1.0f;

    protected UnityEvent_bool onShow = new UnityEvent_bool();

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected virtual void Awake()
    {
        GetGenericComponents();
    }

    protected virtual void Start()
    {

    }
    
    protected virtual void Update()
    {

    }
    
    protected virtual void FixedUpdate()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventOnPointerEnter();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        EventOnPointerExit();
    }
    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public virtual void EventOnPointerEnter()
    {

    }

    public virtual void EventOnPointerExit()
    {

    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    private void GetGenericComponents()
    {
        rTransform = GetComponent<RectTransform>();
        visiblePos = rTransform.anchoredPosition;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void Show(bool show)
    {
        if (showHideType == ShowHide.Instant || showHideType == ShowHide.Fade)
        {
            gameObject.SetActive(show);
            visible = show;
        }
        else if (showHideType == ShowHide.Slide)
        {
            if (show != visible)
            {
                if (show)
                {
                    Move(visiblePos, transitionTime, show);
                }
                else
                {
                    Move(visiblePos + hiddenOffset, transitionTime, show);
                }
                visible = show;
            }
        }
    }
    
    public void Show(bool show, InterpDelta.InterpTypes moveType)
    {
        if (showHideType == ShowHide.Instant || showHideType == ShowHide.Fade)
        {
            gameObject.SetActive(show);
            visible = show;
        }
        else if (showHideType == ShowHide.Slide)
        {
            if (show != visible)
            {
                if (show)
                {
                    Move(moveType, visiblePos, transitionTime, show);
                }
                else
                {
                    Move(moveType, visiblePos + hiddenOffset, transitionTime, show);
                }
                visible = show;
            }
        }
    }

    public void SetShown(bool show)
    {
        SetShown(show, showHideType);
    }
    
    public void SetShown(bool show, ShowHide showHideType)
    {
        if (showHideType == ShowHide.Instant || showHideType == ShowHide.Fade)
        {
            gameObject.SetActive(show);
            visible = show;
        }
        else if (showHideType == ShowHide.Slide)
        {
            if (show)
            {
                rTransform.anchoredPosition = visiblePos;
            }
            else
            {
                rTransform.anchoredPosition = visiblePos + hiddenOffset;
            }
            visible = show;
        }

        onShow.Invoke(show);
    }

    public void DoDelayedShow(bool show, float delayTime)
    {
        StartCoroutine(DelayedShow(show, delayTime));
    }
    
    public void FixedDelayedShow(bool show)
    {
        StartCoroutine(DelayedShow(show, fixedShowHideDelay));
    }

    private IEnumerator DelayedShow(bool show, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        Show(show);
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void Move(Vector3 posEnd)
    {
        Vector3 posStart = rTransform.anchoredPosition;
        float animTime = 0.5f;
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, true));
    }
    
    public void Move(Vector3 posEnd, float animTime)
    {
        Vector3 posStart = rTransform.anchoredPosition;
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, true));
    }
    
    public void Move(Vector3 posStart, Vector3 posEnd, float animTime)
    {
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, true));
    }
    
    public void Move(Vector3 posEnd, bool forward)
    {
        Vector3 posStart = rTransform.anchoredPosition;
        float animTime = 0.5f;
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, forward));
    }
    
    public void Move(Vector3 posEnd, float animTime, bool forward)
    {
        Vector3 posStart = rTransform.anchoredPosition;
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, forward));
    }
    
    public void Move(Vector3 posStart, Vector3 posEnd, float animTime, bool forward)
    {
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, forward));
    }
    
    public void Move(InterpDelta.InterpTypes moveType, Vector3 posEnd, bool forward)
    {
        Vector3 posStart = rTransform.anchoredPosition;
        float animTime = 0.5f;
        StartCoroutine(MoveAnim(moveType, posStart, posEnd, animTime, forward));
    }
    
    public void Move(InterpDelta.InterpTypes moveType, Vector3 posEnd, float animTime, bool forward)
    {
        Vector3 posStart = rTransform.anchoredPosition;
        StartCoroutine(MoveAnim(moveType, posStart, posEnd, animTime, forward));
    }
    
    public void Move(InterpDelta.InterpTypes moveType, Vector3 posStart, Vector3 posEnd, float animTime, bool forward)
    {
        StartCoroutine(MoveAnim(moveType, posStart, posEnd, animTime, forward));
    }

    private IEnumerator MoveAnim(InterpDelta.InterpTypes moveType, Vector3 posStart, Vector3 posEnd, float animTime, bool forward)
    {
        isMoving = true;

        float timeElapsed = 0.0f;
        while (timeElapsed <= animTime)
        {
            timeElapsed += Time.deltaTime;
            float delta = timeElapsed / animTime;

            switch (moveType)
            {
                case InterpDelta.InterpTypes.CosCurve:
                    delta = InterpDelta.CosCurve(delta);
                    break;

                case InterpDelta.InterpTypes.CosSpeedUp:
                    delta = InterpDelta.CosSpeedUp(delta);
                    break;

                case InterpDelta.InterpTypes.CosSlowDown:
                    delta = InterpDelta.CosSlowDown(delta);
                    break;

                case InterpDelta.InterpTypes.CosSpeedUpSlowDown:
                    delta = InterpDelta.CosSpeedUpSlowDown(delta, forward);
                    break;

                case InterpDelta.InterpTypes.CosSlowDownSpeedUp:
                    delta = InterpDelta.CosSlowDownSpeedUp(delta, forward);
                    break;

                case InterpDelta.InterpTypes.SmoothedLinear:
                    delta = InterpDelta.SmoothedLinear(delta, 0.3f);
                    break;

                case InterpDelta.InterpTypes.Linear:
                default:
                    break;
            }

            yield return null;

            rTransform.anchoredPosition = Vector3.Lerp(posStart, posEnd, delta);
        }

        isMoving = false;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

}
