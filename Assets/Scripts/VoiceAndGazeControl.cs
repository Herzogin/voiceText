﻿using UnityEngine;
using UnityEngine.EventSystems;
using TextSpeech;
using UnityEngine.Android;

[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(Rigidbody))]
public class VoiceAndGazeControl : MonoBehaviour
{
    bool gazedAt = false;
    const string LANG_CODE = "en-US";
    [SerializeField]
    GameObject affectedGameObject;

    void Start()
    {
        SetupLanguage(LANG_CODE);
        CheckMicPermission();

        EventTrigger trigger = GetComponent<EventTrigger>();

        //ReticlePointer enters trigger
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { ReticlePointerEnters((PointerEventData)data); });

        //ReticlePointer exits trigger
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { ReticlePointerExits((PointerEventData)data); });

        trigger.triggers.Add(enterEntry);
        trigger.triggers.Add(exitEntry);
    }

    void Update()
    {
        if (gazedAt)
        {
            SpeechToText.instance.onResultCallback = OnFinalSpeechResult;
            StartListening();
        }
    }

    void OnFinalSpeechResult(string result)
    {
        if (result.Equals("down"))
        {
            affectedGameObject.GetComponent<Rigidbody>().useGravity = true;
        }
        else if (Equals(result, "small"))
        {
            affectedGameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Equals(result, "big"))
        {
            Vector3 newScale = transform.localScale;
            newScale *= 1.5f;
            affectedGameObject.transform.localScale = newScale;

        }
    }

    private void ReticlePointerEnters(PointerEventData data)
    {
        gazedAt = true;
    }

    private void ReticlePointerExits(PointerEventData data)
    {
        gazedAt = false;
    }

    public void StartListening()
    {
        SpeechToText.instance.StartRecording();
    }

    public void StopListening()
    {
        SpeechToText.instance.StopRecording();
    }

    void CheckMicPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    void SetupLanguage(string languageCode)
    {
        SpeechToText.instance.Setting(languageCode);
    }
}
