using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.InputSystem;
using Formatting = System.Xml.Formatting;

// 通过注解让脚本最优先执行
[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour{
    public static DataManager instance;

    [Header("事件监听")]
    public VoidEventSO saveDataEvent;

    public VoidEventSO loadDataEvent;
    public VoidEventSO newGameEvent;

    private List<ISaveable> saveableList = new List<ISaveable>();
    private Data saveData;
    private bool newGame=true;

    private string jsonFolder;
    
    private void Awake(){
        if (instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }

        saveData = new Data();
        
        // 获取默认存储路径
        jsonFolder = Application.persistentDataPath+"/SAVE_DATA/";
       
        
        ReadSaveData();
        Debug.Log(jsonFolder);
    }


    private void OnEnable(){
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;
    }

    private void OnDisable(){
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }

    public void RegisterSaveData(ISaveable saveable){
        if (!saveableList.Contains(saveable)){
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterSaveData(ISaveable saveable){
        saveableList.Remove(saveable);
    }

    public void Save(){
        newGame = false;
        foreach (var saveable in saveableList){
            saveable.GetSaveData(saveData);
        }

        
        // 创建文件具体路径
        var resultPath = jsonFolder + "data.sav";
        // 序列化为字符串
        var jsonData = JsonConvert.SerializeObject(saveData);
        // 判断是否存在该文件
        if (!File.Exists(resultPath)){
            // 如果不存在该文件，先创建路径文件夹
            Directory.CreateDirectory(jsonFolder);
        }
        // 将文件写入对应路径的文件中
        File.WriteAllText(resultPath, jsonData);
    }

    public void Load(){
        var resultPath = jsonFolder + "data.sav";
        if (!newGame || File.Exists(resultPath) ){
            foreach (var saveable in saveableList){
                saveable.LoadData(saveData);
            }
        }else{
           return;
        }
    }

    public void LoadAfterScreen(DataDefination dataID){
        foreach (var saveable in saveableList){
            if (saveable.GetDataID().ID != dataID.ID){
                saveable.LoadData(saveData);
            }
        }
    }
    
    // 读取数据
    private void ReadSaveData(){
        var resultPath = jsonFolder + "data.sav";
        if (File.Exists(resultPath)){
            // 将文件读取到stringData对象中
            var stringData = File.ReadAllText(resultPath);
            // 文件反序列化
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);
            saveData = jsonData;
        }
    }
}