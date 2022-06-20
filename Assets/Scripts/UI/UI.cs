using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class UI : Core, IPointerEnterHandler, IPointerExitHandler
{
    #region [ PROPERTIES ]

    protected List<Graphic> childGraphicElements = new List<Graphic>();

    [HideInInspector] public bool visible;

    private enum ShowHide { Instant, Fade, Slide };

    [Header("UI Element Properties")]
    public float fixedShowHideDelay = 0.0f;
    [SerializeField] ShowHide showHideType = ShowHide.Instant;
    [SerializeField] float transitionTime = 0.1f;
    [SerializeField] Vector2 hiddenOffset = Vector2.zero;
    [SerializeField] InterpDelta.InterpTypes slideMovementStyle = InterpDelta.InterpTypes.Linear;
    [HideInInspector] Vector2 visiblePos;
    private bool isMoving = false;
    public float opacity = 1.0f;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        OnAwake();
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {

    }

    void FixedUpdate()
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

    protected void OnAwake()
    {
        GetGenericComponents();
    }

    protected void OnStart()
    {

    }

    public virtual void EventOnPointerEnter()
    {

    }

    public virtual void EventOnPointerExit()
    {

    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    private void GetGenericComponents()
    {
        visiblePos = transform.position;

        /*
        List<GameObject> parentObjects = new List<GameObject>();
        List<GameObject> childObjects = new List<GameObject>();

        parentObjects.Add(gameObject);
        bool keepSearching = true;
        do
        {
            foreach (GameObject obj in parentObjects)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    childObjects.Add(obj.transform.GetChild(i).gameObject);
                }
            }

            if (childObjects.Count == 0)
            {
                keepSearching = false;
                continue;
            }

            foreach (GameObject obj in childObjects)
            {
                if (obj.GetComponent<Graphic>() != null)
                {
                    childGraphicElements.Add(obj.GetComponent<Graphic>());
                }
            }

            CopyListData(childObjects, parentObjects);
            childObjects.Clear();
        }
        while (keepSearching);

        Debug.Log(childGraphicElements.Count);
        */
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
        if (showHideType == ShowHide.Instant || showHideType == ShowHide.Fade)
        {
            gameObject.SetActive(show);
            visible = show;
        }
        else if (showHideType == ShowHide.Slide)
        {
            if (show)
            {
                transform.position = visiblePos;
            }
            else
            {
                transform.position = visiblePos + hiddenOffset;
            }
            visible = show;
        }
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

    public void Move(Vector3 posEnd)
    {
        Vector3 posStart = transform.position;
        float animTime = 0.5f;
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, true));
    }
    
    public void Move(Vector3 posEnd, float animTime)
    {
        Vector3 posStart = transform.position;
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, true));
    }
    
    public void Move(Vector3 posStart, Vector3 posEnd, float animTime)
    {
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, true));
    }
    
    public void Move(Vector3 posEnd, bool forward)
    {
        Vector3 posStart = transform.position;
        float animTime = 0.5f;
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, forward));
    }
    
    public void Move(Vector3 posEnd, float animTime, bool forward)
    {
        Vector3 posStart = transform.position;
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, forward));
    }
    
    public void Move(Vector3 posStart, Vector3 posEnd, float animTime, bool forward)
    {
        StartCoroutine(MoveAnim(slideMovementStyle, posStart, posEnd, animTime, forward));
    }
    
    public void Move(InterpDelta.InterpTypes moveType, Vector3 posEnd, bool forward)
    {
        Vector3 posStart = transform.position;
        float animTime = 0.5f;
        StartCoroutine(MoveAnim(moveType, posStart, posEnd, animTime, forward));
    }
    
    public void Move(InterpDelta.InterpTypes moveType, Vector3 posEnd, float animTime, bool forward)
    {
        Vector3 posStart = transform.position;
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

            transform.position = Vector3.Lerp(posStart, posEnd, delta);
        }

        isMoving = false;
    }
}
