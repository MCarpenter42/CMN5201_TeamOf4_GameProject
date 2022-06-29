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

    [Header("Sources")]
    [SerializeField] SFXSource sfxSourcePrefab;
    [HideInInspector] public List<SFXSource> sfxSources = new List<SFXSource>();
    [HideInInspector] public SFXSource playerSFX;
    [HideInInspector] public List<SFXSource> beamSFX = new List<SFXSource>();

    [Header("Player SFX")]
    [SerializeField] public List<AudioClip> walkStone = new List<AudioClip>();
    [SerializeField] public List<AudioClip> walkWood = new List<AudioClip>();
    [SerializeField] public List<AudioClip> walkGrass = new List<AudioClip>();
    [SerializeField] public List<AudioClip> walkFoliage = new List<AudioClip>();
    [SerializeField] public List<AudioClip> walkWater = new List<AudioClip>();
    private Coroutine playerWalkCycle = null;

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
        //GetExistingSFX();
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

        playerSFX = GetChildrenWithComponent<SFXSource>(GameManager.Player.gameObject)[0].GetComponent<SFXSource>();
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

    public int CreateSFXSource(Vector3 pos, string listName)
    {
        int n = sfxSources.Count;

        SFXSource sfxSource;
        switch (listName)
        {
            case "beamSFX":
                n = beamSFX.Count;
                sfxSource = Instantiate(sfxSourcePrefab, pos, Quaternion.identity);
                beamSFX.Add(sfxSource);
                break;

            case "sfxSources":
            default:
                n = sfxSources.Count;
                sfxSource = Instantiate(sfxSourcePrefab, pos, Quaternion.identity);
                sfxSources.Add(sfxSource);
                break;
        }

        return n;
    }

    public int CreateSFXSource(Vector3 pos, AudioClip clip, string listName)
    {
        int n = sfxSources.Count;

        SFXSource sfxSource;
        switch (listName)
        {
            case "beamSFX":
                n = beamSFX.Count;
                sfxSource = Instantiate(sfxSourcePrefab, pos, Quaternion.identity);
                sfxSource.SetAudioClip(clip);
                beamSFX.Add(sfxSource);
                break;

            case "sfxSources":
            default:
                n = sfxSources.Count;
                sfxSource = Instantiate(sfxSourcePrefab, pos, Quaternion.identity);
                sfxSource.SetAudioClip(clip);
                sfxSources.Add(sfxSource);
                break;
        }

        return n;
    }

    public bool DestroySFXSource(int index, string listName)
    {
        bool indexInBounds = false;
        switch (listName)
        {
            case "beamSFX":
                if (indexInBounds)
                {
                    Destroy(beamSFX[index], 0.05f);
                    beamSFX.RemoveAt(index);
                }
                break;

            case "sfxSources":
            default:
                if (indexInBounds)
                {
                    Destroy(sfxSources[index], 0.05f);
                    sfxSources.RemoveAt(index);
                }
                break;
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

    public void PlayerWalk(float time, int stepCount)
    {
        if (playerWalkCycle != null)
        {
            StopCoroutine(playerWalkCycle);
        }
        playerWalkCycle = StartCoroutine(IPlayerWalk(time, stepCount));
    }

    private IEnumerator IPlayerWalk(float time, int stepCount)
    {
        float stepTime = time / (float)stepCount;

        float targetTime = stepTime / 2.0f;
        for (int i = 0; i < stepCount; i++)
        {
            float timePassed = 0.0f;
            while (timePassed <= targetTime)
            {
                yield return null;
                timePassed += Time.deltaTime;
            }
            
            switch (GameManager.Player.GetFloorType())
            {
                case FloorTypes.Empty:
                case FloorTypes.Stone:
                default:
                    playerSFX.PlayAudioClip(PickFromList(walkStone));
                    break;
            }

            targetTime = stepTime;
        }
    }
}
