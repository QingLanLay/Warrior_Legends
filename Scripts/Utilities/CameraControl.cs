using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour{
    
    [Header("事件监听")]
    public VoidEventSO afterLoadEvent;
    
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;
    private void Awake(){
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable(){
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
        afterLoadEvent.OnEventRaised += OnAfterLoadEvent;
    }


    private void OnDisable(){
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        afterLoadEvent.OnEventRaised -= OnAfterLoadEvent;
        
    }

    private void OnAfterLoadEvent(){
        GetNewCameraBounds();
    }

    private void OnCameraShakeEvent(){
        // 播放震动
        impulseSource.GenerateImpulse();
    }

    // 场景切换后修改
    // private void Start(){
    //     GetNewCameraBounds();
    // }

    private void GetNewCameraBounds(){
        var obj = GameObject.FindGameObjectWithTag("Bounds");

        if (obj == null){
            return;
        }

        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        // 切换场景清理缓存
        confiner2D.InvalidateCache();
    }
}