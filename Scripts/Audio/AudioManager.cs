using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour{
    [Header("事件监听")]
    public PlayAudioEventSO FXEvent;
    public VoidEventSO pasueEvent;
    public FloatEventSO syncVolumeEvent;
    
    public PlayAudioEventSO BGMEvent;
    public FloatEventSO volumeEvent;

    public AudioSource BGMSource;
    public AudioSource FXSource;

    public AudioMixer mixer;

    private void OnEnable(){
        FXEvent.onEventRasied += OnFXEvnet;
        BGMEvent.onEventRasied += OnBGMEvent;
        volumeEvent.OnEventRaised += OnVolumeEvent;
        pasueEvent.OnEventRaised += OnPasueEvent;
    }


    private void OnDisable(){
        FXEvent.onEventRasied -= OnFXEvnet;
        BGMEvent.onEventRasied -= OnBGMEvent;
        volumeEvent.OnEventRaised -= OnVolumeEvent;
        pasueEvent.OnEventRaised -= OnPasueEvent;
    }

    private void OnBGMEvent(AudioClip clip){
        BGMSource.clip = clip;
        BGMSource.Play();
    }

    private void OnFXEvnet(AudioClip clip){
        FXSource.clip = clip;
        FXSource.Play();
    }


    private void OnVolumeEvent(float amount){
        mixer.SetFloat("MasterVolume", amount * 100 - 80);
    }
    private void OnPasueEvent(){
        float amount; 
        mixer.GetFloat("MasterVolume",out amount);
        syncVolumeEvent.Raise( amount);
    }
}
