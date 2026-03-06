using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, Iinteractable, ISaveable{
    private SpriteRenderer spriteRenderer;
    public Sprite onpenSprite;
    public Sprite closedSprite;
    public bool isDone;


    private void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable(){
        spriteRenderer.sprite = isDone ? onpenSprite : closedSprite;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
        
    }

    private void OnDestroy(){
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    public void TriggerAction(){
        if (!isDone){
            OpenChest();
        }
    }

    public void OpenChest(){
        spriteRenderer.sprite = onpenSprite;
        isDone = true;
        tag = "Untagged";
    }

    public DataDefination GetDataID(){
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data){
        if (data.boolSaveData.ContainsKey(GetDataID().ID)){
            data.boolSaveData[GetDataID().ID] = isDone;
        }
        else{
            data.boolSaveData.Add(GetDataID().ID, isDone);
        }
        Debug.Log("宝箱存储了");
    }

    public void LoadData(Data data){
        if (data.boolSaveData.ContainsKey(GetDataID().ID)){
            isDone = data.boolSaveData[GetDataID().ID];
            Debug.Log("isDone:"+isDone);
            if (isDone){
                OpenChest();
            }
        }
    }
}