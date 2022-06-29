using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class AudioController : Core
{
    #region [ PROPERTIES ]

    [Header("Music")]
    [SerializeField] AudioClip levelMusic;
    private AudioSource musicPlayer;

    [Header("SFX")]
    [SerializeField] GameObject sfxPlayerPrefab;

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
    }

    void Start()
    {
        musicPlayer.Play();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void GetComponents()
    {
        musicPlayer = gameObject.GetComponent<AudioSource>();
        musicPlayer.clip = levelMusic;
    }

}
