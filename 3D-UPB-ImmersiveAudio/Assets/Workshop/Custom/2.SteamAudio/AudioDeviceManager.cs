using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioDeviceManager : MonoBehaviour
{
    [Header("Choose Device to play")]
    public bool speaker = true;
    public bool radio = false;

    [Header("Choose Clip to play")]
    [Range(0, 10)] public int clipNumber = 0;
    public List<AudioClip> clips;


    private AudioSource[] audioSources;
    private int lastClipNumber = -1;
    private bool lastSpeakerState;
    private bool lastRadioState;


    void Start()
    {
        // true = include si obiectele disabled
        audioSources = GetComponentsInChildren<AudioSource>(true);
        SetClip();
        UpdateDevicesState();
            
    }

    void Update()
    {
        if (lastClipNumber != clipNumber)
            SetClip();
        if (lastSpeakerState != speaker || lastRadioState != radio)
            UpdateDevicesState();
    }

    public void SetClip()
    {
        if (clips == null || clips.Count == 0)
            return;

        if (clipNumber >= clips.Count)
            clipNumber = clips.Count - 1;

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.clip = clips[clipNumber];

            if (audioSource.enabled && audioSource.gameObject.activeInHierarchy)
                audioSource.Play();
        }

        lastClipNumber = clipNumber;
    }

    private void UpdateDevicesState()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            Transform parentTransform = audioSource.transform.parent;

            string parentName = parentTransform.name.ToLower();

            if (parentName.Contains("speaker"))
                parentTransform.gameObject.SetActive(speaker);

            if (parentName.Contains("radio"))
                parentTransform.gameObject.SetActive(radio);
        }

        lastSpeakerState = speaker;
        lastRadioState = radio;
    }
}
