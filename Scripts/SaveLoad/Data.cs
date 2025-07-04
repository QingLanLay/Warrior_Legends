using System.Collections.Generic;
using UnityEngine;


public class Data{
    public string sceneToSave;
    
    public Dictionary<string,SerializeVector3> characterPosDict = new Dictionary<string, SerializeVector3>();
    public Dictionary<string,float> floatSaveData = new Dictionary<string, float>();
    public Dictionary<string,bool> boolSaveData = new Dictionary<string, bool>();

    public void SaveGameScene(GameSceneSO saveScene){
        // 因为assets文件没法直接存储
        // 将当前场景序列化保存为字符串sceneToSave
        sceneToSave = JsonUtility.ToJson(saveScene);
    }

    public GameSceneSO GetSaveScene(){
        // 创建一个空的实例
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        // 将sceneTodSave反序列化赋值给newScene
        JsonUtility.FromJsonOverwrite(sceneToSave,newScene);
        
        return newScene;
    }
}

public class SerializeVector3{
    public float x, y, z;

    public  SerializeVector3(Vector3 pos){
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3(){
        return new Vector3(x,y,z);
    }
}