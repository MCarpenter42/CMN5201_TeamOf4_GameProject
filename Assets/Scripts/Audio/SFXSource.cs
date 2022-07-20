using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class SFXSource : Core
{
    #region [ PROPERTIES ]

    [Header("Audio")]
    public AudioSource source;
    public AudioClip clip;

    [Header("Settings")]
    public float baseVolume = 1.0f;
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
        AudioSourceComponent();
    }

    void Update()
    {
#if UNITY_EDITOR
        AudioSourceComponent();
#endif
        if (volumeFalloff)
        {
            DoVolumeFalloff();
        }
    }

#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private void AudioSourceComponent()
    {
        if (source == null)
        {
            if (gameObject.GetComponent<AudioSource>() == null)
            {
                source = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                source = gameObject.GetComponent<AudioSource>();
            }
        }

        if (minDistance >= maxDistance)
        {
            minDistance = maxDistance - 0.1f;
        }
        distanceRange = minDistance - maxDistance;
        source.volume = baseVolume;
    }

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
        /*if (!alreadyPlaying)
        {
            source.PlayOneShot(clip);
        }*/
        source.PlayOneShot(clip);
        return alreadyPlaying;
    }
    
    public bool PlayAudioClip(AudioClip clip)
    {
        bool alreadyPlaying = source.isPlaying;
        /*if (!alreadyPlaying)
        {
            source.PlayOneShot(clip);
        }*/
        source.PlayOneShot(clip, 1.0f);
        return alreadyPlaying;
    }

    public bool PlayAudioClip(AudioClip clip, float pitch, float volume)
    {
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);

        bool alreadyPlaying = source.isPlaying;
        if (!alreadyPlaying)
        {
            float prePitch = source.pitch;
            float preVolume = source.volume;
            source.pitch = pitch;
            source.volume = volume;

            source.PlayOneShot(clip, volume);

            StartCoroutine(DelayedSetPitch(prePitch, clip.length));
        }
        return alreadyPlaying;
    }

    public void PlayAudioLoop(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    private IEnumerator DelayedSetPitch(float pitch, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.pitch = pitch;
    }
    
    private IEnumerator DelayedSetVolume(float volume, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.volume = volume;
    }
    
    private IEnumerator DelayedSetPV(float pitch, float volume, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.pitch = pitch;
        source.volume = volume;
    }

    public void Stop()
    {
        source.Stop();
    }
}
