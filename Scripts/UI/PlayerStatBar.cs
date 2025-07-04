using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour{
    private Character currentCharacter;
    public Image healthImage;
    public Image delayImage;
    public Image powerImage;

    
    private bool isRecovering;
    private void Update(){

        if (delayImage.fillAmount > healthImage.fillAmount){
            delayImage.fillAmount -= Time.deltaTime;
        }

        if (isRecovering){
           float persentage = currentCharacter.currentPower / currentCharacter.maxPower; 
           powerImage.fillAmount = persentage;

           if (persentage >= 1){
               isRecovering = false;
               return;
           }
        }
    }


    /// <summary>
    /// 接受Health的变更百分比
    /// </summary>
    /// <param name="persentage">百分比：Current/Max</param>
    public void OnHealthChanged(float persentage){
        healthImage.fillAmount = persentage;
    }

    public void OnPowerChanged(Character character){
        isRecovering = true;
        
        currentCharacter = character;
    }
}