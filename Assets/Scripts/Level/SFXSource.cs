using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class SFXSource : Core
{
    #region [ PROPERTIES ]

    [Header("Audio")]
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip clip;

    [Header("Settings")]
    [SerializeField] float baseVolume;
    [SerializeField] bool volumeFalloff;
    [Range(0.0f, 100.0f)]
    [SerializeField] float minDistance = 0.0f;
    [Range(0.1f, 100.1f)]
    [SerializeField] float maxDistance = 20.0f;
    private float distanceRange = 20.0f;

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        if (minDistance >= maxDistance)
        {
            minDistance = maxDistance - 0.1f;
        }
        distanceRange = minDistance - maxDistance;
        source.volume = baseVolume;
    }

    void Update()
    {
        if (volumeFalloff)
        {
            DoVolumeFalloff();
        }
    }

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private void DoVolumeFalloff()
    {
        float distance = (GameManager.Listener.transform.position - transform.position).magnitude;
        if (distance <= minDistance)
        {
            source.volume = baseVolume;
        }
        else if (distance > minDistance && distance <= maxDistance)
        {
            source.volume = 1.0f - ((distance - minDistance) / distanceRange);
        }
        else
        {
            source.volume = 0.0f;
        }
    }

    public void SetAudioClip()
    {
        source.clip = this.clip;
    }
	
    public void SetAudioClip(AudioClip clip)
    {
        this.clip = clip;
        source.clip = clip;
    }

    public bool PlayAudioClip()
    {
        bool alreadyPlaying = source.isPlaying;
        if (!alreadyPlaying)
        {
            source.PlayOneShot(clip);
        }
        return alreadyPlaying;
    }
    
    public bool PlayAudioClip(AudioClip clip)
    {
        bool alreadyPlaying = source.isPlaying;
        if (!alreadyPlaying)
        {
            source.PlayOneShot(clip);
        }
        return alreadyPlaying;
    }

    public bool PlayAudioClip(AudioClip clip, float pitch, float volume)
    {
        bool alreadyPlaying = source.isPlaying;
        if (!alreadyPlaying)
        {
            float prePitch = source.pitch;
            float preVolume = source.volume;
            source.pitch = pitch;
            source.volume = volume;

            source.PlayOneShot(clip);

            StartCoroutine(DelayedSetPV(prePitch, preVolume, clip.length));
        }
        return alreadyPlaying;
    }

    private IEnumerator DelayedSetPV(float pitch, float volume, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.pitch = pitch;
        source.volume = volume;
    }
}
