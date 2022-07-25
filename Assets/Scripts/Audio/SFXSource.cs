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

    [Header("Properties")]
    public float baseVolume = 1.0f;
    public bool volumeFalloff;
    [Range(0.0f, 100.0f)]
    public float minDistance = 0.0f;
    [Range(0.1f, 100.1f)]
    public float maxDistance = 20.0f;
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
        if (Application.isPlaying)
        {
            if (volumeFalloff && source.isPlaying)
            {
                DoVolumeFalloff();
            }
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void SetProperties(float baseVolume, bool volumeFalloff)
    {
        this.baseVolume = baseVolume;
        this.volumeFalloff = volumeFalloff;
        if (volumeFalloff)
        {
            this.minDistance = 0.0f;
            this.maxDistance = 20.0f;
        }
    }
    
    public void SetProperties(float baseVolume, bool volumeFalloff, float minDistance, float maxDistance)
    {
        this.baseVolume = baseVolume;
        this.volumeFalloff = volumeFalloff;
        if (volumeFalloff)
        {
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
        }
    }

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
        source.ignoreListenerVolume = true;
    }

    private void DoVolumeFalloff()
    {
        distanceRange = maxDistance - minDistance;

        float distance = (GameManager.Listener.transform.position - transform.position).magnitude;
        float volume;

        if (distance <= minDistance)
        {
            volume = baseVolume;
        }
        else if (distance > minDistance && distance <= maxDistance)
        {
            volume = 1.0f - InterpDelta.CosSlowDown((distance - minDistance) / distanceRange);
        }
        else
        {
            volume = 0.0f;
        }

        source.volume = volume;
    }

    public bool CompareClip(AudioClip clip)
    {
        return this.clip == clip;
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
        source.loop = false;
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
        source.loop = false;
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
            source.loop = false;

            source.PlayOneShot(clip, volume);

            StartCoroutine(DelayedSetPitch(prePitch, clip.length));
        }
        return alreadyPlaying;
    }

    public void PlayAudioLoop(AudioClip clip)
    {
        SetAudioClip(clip);
        source.loop = true;
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

    public virtual void Stop()
    {
        source.Stop();
    }
}
