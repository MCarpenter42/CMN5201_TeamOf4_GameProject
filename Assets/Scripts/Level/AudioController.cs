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

    [Header("General Settings")]
    [SerializeField] Vector3 levelCentre;
    private List<GameObject> reverbZones = new List<GameObject>();

    [Header("Music")]
    [SerializeField] AudioClip levelMusic;
    private AudioSource musicPlayer;

    [Header("SFX")]
    [SerializeField] SFXSource sfxSourcePrefab;
    private List<SFXSource> sfxSources = new List<SFXSource>();

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        transform.position = Vector3.zero;
        GetComponents();
        foreach(GameObject zone in reverbZones)
        {
            zone.transform.position = levelCentre;
        }
        GetExistingSFX();
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

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.GetComponent<AudioReverbZone>() != null)
            {
                reverbZones.Add(child);
            }
        }
    }

    private void GetExistingSFX()
    {
        List<SFXSource> sources = ArrayToList(FindObjectsOfType<SFXSource>());
        CopyListData(sources, sfxSources);
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public int CreateSFXSource(Vector3 pos)
    {
        int n = sfxSources.Count;

        SFXSource sfxSource = Instantiate(sfxSourcePrefab, pos, Quaternion.identity);
        sfxSources.Add(sfxSource);

        return n;
    }

    public int CreateSFXSource(Vector3 pos, AudioClip clip)
    {
        int n = sfxSources.Count;

        SFXSource sfxSource = Instantiate(sfxSourcePrefab, pos, Quaternion.identity);
        sfxSource.SetAudioClip(clip);
        sfxSources.Add(sfxSource);

        return n;
    }

    public bool DestroySFXSource(int index)
    {
        bool indexInBounds = InBounds(index, sfxSources);
        if (indexInBounds)
        {
            Destroy(sfxSources[index], 0.05f);
            sfxSources.RemoveAt(index);
        }
        return indexInBounds;
    }

    public int AddSFXSource(SFXSource source)
    {
        int n = sfxSources.Count;

        sfxSources.Add(source);

        return n;
    }

    public bool RemoveSFXSource(int index)
    {
        bool indexInBounds = InBounds(index, sfxSources);
        if (indexInBounds)
        {
            sfxSources.RemoveAt(index);
        }
        return indexInBounds;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void PlayerMoveAudio()
    {

    }
}
