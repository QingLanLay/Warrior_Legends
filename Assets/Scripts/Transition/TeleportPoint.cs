using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, Iinteractable{
    public Vector3 positionToGo;
    public GameSceneSO sceneToGo;
    public SceneLoadEventSO sceneLoadEvent;

    private bool fristEnter;

    public void TriggerAction(){
        Debug.Log("传送!");
        sceneLoadEvent.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
        
    }
}