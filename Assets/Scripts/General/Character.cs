using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    [Header("基础属性")]
    public float maxHealth;

    public float currentHealth;
    public float maxPower;
    public float currentPower;

    public float powerRecoverSpeed;

    [Header("受伤无敌")]
    public float invulnerableTime;

    [HideInInspector]
    public float invulnerableCounter;

    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    public void NewGame(){
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void OnEnable(){
        newGameEvent.OnEventRaised += NewGame;
        
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable(){
        newGameEvent.OnEventRaised -= NewGame;
        
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void Update(){
        // 通过计时器判断无敌时间
        if (invulnerable){
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0){
                invulnerable = false;
            }
        }

        if (currentPower < maxPower){
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    public void TakeDamage(Attack attacker){
        if (invulnerable){
            return;
        }

        // Debug.Log(attacker.damage);
        if (currentHealth - attacker.damage > 0){
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            // 执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else{
            currentHealth = 0;
            // 触发死亡
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }

    /// <summary>
    /// 触发受伤无敌
    /// </summary>
    private void TriggerInvulnerable(){
        if (!invulnerable){
            invulnerable = true;
            invulnerableCounter = invulnerableTime;
        }
    }

    public void OnSlide(int cost){
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }

    private void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Water") && this.gameObject.layer != LayerMask.NameToLayer("BackToMenu")){
            // 死亡，更新血量
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }

    public DataDefination GetDataID(){
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data){
        if (data.characterPosDict.ContainsKey(GetDataID().ID)){
            data.characterPosDict[GetDataID().ID] = new SerializeVector3(transform.position);
            data.floatSaveData[GetDataID().ID + "health"] = currentHealth;
            data.floatSaveData[GetDataID().ID + "power"] = currentPower;
        }
        else{
            data.characterPosDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.floatSaveData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSaveData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    public void LoadData(Data data){
        if (data.characterPosDict.ContainsKey(GetDataID().ID)){
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();
            currentHealth = data.floatSaveData[GetDataID().ID + "health"];
            currentPower = data.floatSaveData[GetDataID().ID + "power"];
            
            // 通知UI更新
            OnHealthChange?.Invoke(this);
        }
    }
}