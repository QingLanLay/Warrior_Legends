
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject{
    public UnityAction<GameSceneSO,Vector3,bool> LoadRequestEvent;

    /// <summary>
    /// 场景加载请求
    /// </summary>
    /// <param name="scene">要加载场景</param>
    /// <param name="position">位置</param>
    /// <param name="loading">是否有过场</param>
    public void RaiseLoadRequestEvent(GameSceneSO scene, Vector3 position, bool loading){
        LoadRequestEvent?.Invoke(scene,position,loading);
    }
}

