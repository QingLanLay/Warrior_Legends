using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour 
{
    [Header("事件监听")]
    public FadeEventSO fadeEvent;

    public Image fadeImage;

    private void OnEnable(){
        fadeEvent.OnEventRaised += OnFadeEvent;
    }

    private void OnDidApplyAnimationProperties(){
        fadeEvent.OnEventRaised -= OnFadeEvent;
    }

    private void OnFadeEvent(Color color,float duration,bool fadeIn){
        fadeImage.DOBlendableColor(color, duration);
    }
    
    
}
