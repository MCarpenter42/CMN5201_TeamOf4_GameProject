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

    [Header("Ambience Settings")]
    public float interval = 0.2f;
    [Range(0.0f, 0.5f)]
    public float intervalRandomness = 0.25f;
    [Range(0.0f, 1.0f)]
    public float triggerChance = 0.1f;
    [Range(0.0f, 1.0f)]
    public float adjustPerCheck = 0.1f;
    public float cooldown = 1.0f;

    private float minInclusive;
    private float maxInclusive;

    private bool playing = false;
    private Coroutine ambientLoop = null;

    #endregion

    #region [ DEBUG & DEVELOPMENT ]

    [Header("Debug Readout")]
    [SerializeField] TMP_Text debug_Name;
    [SerializeField] TMP_Text debug_IsPlaying;
    [SerializeField] TMP_Text debug_Cooldown;
    [SerializeField] TMP_Text debug_Interval;
    [SerializeField] TMP_Text debug_TriggerChance;
    [SerializeField] TMP_Text debug_TriggerAttempts;
    private bool doReadout = true;
    private bool onCooldown = false;
    private float cooldownTracker = 0.0f;
    private float triggerTimeTotal = 0.0f;
    private int triggerCount = 0;
    private string timesToTrigger = "Times to trigger: ";

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        minInclusive = 1.0f - intervalRandomness;
        maxInclusive = 1.0f + intervalRandomness;

        if (debug_Name == null || debug_IsPlaying == null || debug_Cooldown == null || debug_Interval == null || debug_TriggerChance == null || debug_TriggerAttempts == null)
        {
            doReadout = false;
        }
    }

    void Start()
    {
        if (doReadout)
        { debug_Name.text = "SFX: " + gameObject.name.ToUpper(); }
    }

    void Update()
    {
        if (onCooldown && cooldownTracker > 0.0f)
        {
            cooldownTracker -= Time.deltaTime;
            if (cooldownTracker < 0.0f)
            { cooldownTracker = 0.0f; }
            if (doReadout)
            { debug_Cooldown.text = "              Cooldown: " + DecimalPlacesString(cooldownTracker, 3); }
        }
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
        int triggerAttempts = 0;
        string percent;
        float timeToTrigger = 0.0f;

        float tickingTriggerChance = triggerChance;
        float randInterval;
        while(playing)
        {
            source.Stop();

            randInterval = interval * Random.Range(minInclusive, maxInclusive);
            if (doReadout)
            { debug_Interval.text = "        Check interval: " + DecimalPlacesString(randInterval, 3) + " (" + DecimalPlacesString(interval, 3) + ") "; }
            yield return new WaitForSeconds(randInterval);
            timeToTrigger += randInterval;

            triggerAttempts++;
            if (doReadout)
            { debug_TriggerAttempts.text = "      Trigger attempts: " + triggerAttempts; }

            if (Random.value <= tickingTriggerChance)
            {
                AudioClip clip = PickFromList(clips);
                PlayAudioClip(clip);

                triggerTimeTotal += timeToTrigger;
                triggerCount++;
                timesToTrigger += DecimalPlacesString(timeToTrigger, 2) + "s, ";
                Debug.Log("Average trigger time over " + triggerCount + " instances: " + (triggerTimeTotal / (float)triggerCount));
                timeToTrigger = 0.0f;

                if (doReadout)
                { debug_IsPlaying.text = "        Playing status: Playing"; }
                yield return new WaitForSeconds(clip.length);
                if (doReadout)
                { debug_IsPlaying.text = "        Playing status: Not playing"; }

                tickingTriggerChance = triggerChance;
                percent = DecimalPlacesString(tickingTriggerChance * 100.0f, 2) + "%";
                
                if (doReadout)
                { debug_TriggerChance.text = "Current trigger chance: " + percent; }

                triggerAttempts = 0;
                if (doReadout)
                { debug_TriggerAttempts.text = "      Trigger attempts: " + triggerAttempts; }

                cooldownTracker = cooldown;
                onCooldown = true;
                yield return new WaitForSeconds(cooldown);
                onCooldown = false;
            }
            else
            {
                percent = DecimalPlacesString(tickingTriggerChance * 100.0f, 2) + "%";
                if (doReadout)
                { debug_TriggerChance.text = "Current trigger chance: " + percent; }
                tickingTriggerChance = Mathf.Clamp(tickingTriggerChance * (1.0f + adjustPerCheck), 0.0f, 1.0f);
            }
        }
    }
}
