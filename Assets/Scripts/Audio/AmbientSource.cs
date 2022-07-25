using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class AmbientSource : SFXSource
{
    #region [ PROPERTIES ]

    private List<AudioClip> clips = new List<AudioClip>();

    public float interval = 0.2f;
    [Range(0.0f, 0.5f)]
    public float intervalRandomness = 0.25f;
    [Range(0.0f, 1.0f)]
    public float triggerChance = 0.1f;
    public float cooldown = 1.0f;

    private float minInclusive;
    private float maxInclusive;

    private bool playing = false;
    private Coroutine ambientLoop = null;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    public void SetClipsList(List<AudioClip> clips)
    {
        this.clips = clips;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void Play()
    {
        minInclusive = interval * (1.0f - intervalRandomness);
        maxInclusive = interval * (1.0f + intervalRandomness);

        if (clips.Count > 0 && !playing)
        {
            playing = true;
            ambientLoop = StartCoroutine(AmbientLoop());
        }
    }

    public override void Stop()
    {
        base.Stop();
        if (ambientLoop != null)
        {
            StopCoroutine(ambientLoop);
        }
        playing = false;
    }

    private IEnumerator AmbientLoop()
    {
        float tickingTriggerChance = triggerChance;
        float randInterval = interval;
        while(playing)
        {
            randInterval = interval * Random.Range(minInclusive, maxInclusive);
            yield return new WaitForSeconds(randInterval);

            if (Random.value <= triggerChance)
            {
                AudioClip clip = PickFromList(clips);
                PlayAudioClip(clip);
                yield return new WaitForSeconds(cooldown + clip.length);
            }
            else
            {
                tickingTriggerChance = Mathf.Clamp(tickingTriggerChance * 1.25f, 0.0f, 1.0f);
            }
        }
    }
}
