using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class AudioButton : Button, IPointerClickHandler
{
    [Header("Audio")]
    [SerializeField] public AudioClip clickSound;
    private AudioSource sfx;

    protected override void Awake()
    {
        base.Awake();
        if (gameObject.GetComponent<AudioSource>() == null)
        {
            sfx = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            sfx = gameObject.GetComponent<AudioSource>();
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerClick(eventData);
            sfx.PlayOneShot(clickSound);
        }
    }
}
