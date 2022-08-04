using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class OpeningCutscene : Core
{
    #region [ OBJECTS ]

    [Header("Components")]
    [SerializeField] GameObject container;
    [SerializeField] TMP_Text chagingText;
    [SerializeField] GameObject endImage;
    [SerializeField] GameObject finalText;

	#endregion

	#region [ PROPERTIES ]

    [Header("Text Sequence")]
	[TextArea(3, 5)]
	[SerializeField] string[] textLines;
    [Range(0.0f, 20.0f)]
    [SerializeField] float lineDisplayTime = 6.0f;
    [Range(0.0f, 20.0f)]
    [SerializeField] float finalScreenDisplayTime = 5.0f;

    #endregion

    #region [ COROUTINES ]

    private Coroutine sequence = null;
    private Coroutine fade = null;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        Color clr = chagingText.color;
        clr.a = 0.0f;
        chagingText.color = clr;
        endImage.SetActive(false);
        finalText.SetActive(false);
    }

    void Start()
    {
        if (!GameManager.introPlayed)
        {
            GameManager.introPlayed = true;
            GameManager.AudioController.DelayedPlayMusic(0.25f);
            sequence = StartCoroutine(IIntroSequence());
        }
        else
        {
            container.SetActive(false);
        }
    }
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private IEnumerator IIntroSequence()
    {
        yield return new WaitForSeconds(0.3f);
        GameManager.AudioController.ChangeMusic(AudioController.MusicTrack.Intro);
        float textFadeTime = 1.0f;
        for (int i = 0; i < textLines.Length; i++)
        {
            chagingText.text = textLines[i];
            fade = StartCoroutine(ITextFade(true, textFadeTime));
            yield return new WaitForSeconds(textFadeTime + lineDisplayTime);
            fade = StartCoroutine(ITextFade(false, textFadeTime));
            yield return new WaitForSeconds(textFadeTime);
            if (i < textLines.Length - 1)
            {
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }
        }
        endImage.SetActive(true);
        finalText.SetActive(true);
        yield return new WaitForSeconds(finalScreenDisplayTime);
        Skip();
    }

    private IEnumerator ITextFade(bool fadeIn, float fadeTime)
    {
        Color clrStart = chagingText.color;
        Color clrEnd = chagingText.color;
        if (fadeIn)
        {
            clrStart.a = 0.0f;
            clrEnd.a = 1.0f;
        }
        else
        {
            clrStart.a = 1.0f;
            clrEnd.a = 0.0f;
        }

        float timePassed = 0.0f;
        while (timePassed <= fadeTime)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float delta = timePassed / fadeTime;
            chagingText.color = Color.Lerp(clrStart, clrEnd, delta);
        }
        chagingText.color = clrEnd;
    }

    public void Skip()
    {
        if (fade != null)
        {
            StopCoroutine(fade);
        }
        if (sequence != null)
        {
            StopCoroutine(sequence);
        }
        StartCoroutine(ISkip());
    }

    private IEnumerator ISkip()
    {
        GameManager.UIController.BlackScreenFade(true, 1.0f);
        GameManager.AudioController.MusicFade(true, 1.2f);
        yield return new WaitForSeconds(1.3f);
        container.SetActive(false);
        GameManager.AudioController.ChangeMusic(AudioController.MusicTrack.Menu);
        GameManager.AudioController.MusicFade(false, 1.2f);
        yield return new WaitForSeconds(0.2f);
        GameManager.UIController.BlackScreenFade(false, 1.0f);
    }
}
