using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class ButtonAudio : Core
{
    private enum ButtonType { Standard, Heavy };

    [Header("Audio")]
    [SerializeField] ButtonType weight;
    private SFXSource sfx;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    void Awake()
    {
        sfx = gameObject.GetComponent<SFXSource>();
        if (sfx == null)
        {
            sfx = gameObject.AddComponent<SFXSource>();
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void PlayClickSound()
    {
        List<AudioClip> clips = new List<AudioClip>();
        if (weight == ButtonType.Standard)
        {
            clips = GameManager.AudioController.buttonStandard;
        }
        else
        {
            clips = GameManager.AudioController.buttonHeavy;
        }
        GameManager.AudioController.uiSFX.PlayAudioClip(PickFromList(clips));
    }
}
