using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, ISaveable{
    public Vector3 firstPos;
    public Vector3 menuPos;

    [Header("事件监听")]
    public SceneLoadEventSO sceneLoadEvent;

    public FadeEventSO fadeEvent;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("场景")]
    public GameSceneSO firstScene;

    public GameSceneSO menuScene;

    private GameSceneSO sceneToLoad;
    private GameSceneSO currentLoadScene;


    private bool toMenu;
    public bool loadAfterState;

    public bool ToMenu{
        get => toMenu;
    }

    private Vector3 positionToGO;
    private bool fadeScreen;
    public float fadeDuration;
    private bool isLoading;

    [Header("玩家")]
    public Transform playerTransform;

    [Header("广播")]
    public VoidEventSO afterLoadEvent;

    public SceneLoadEventSO unloadEvent;






    // TODO：做完MainMenu后修改
    private void Start(){
        // NewGame();
        sceneLoadEvent.RaiseLoadRequestEvent(menuScene, menuPos, true);
    }


    private void OnEnable(){
        sceneLoadEvent.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable(){
        sceneLoadEvent.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;


        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void OnBackToMenuEvent(){
        sceneToLoad = menuScene;
        sceneLoadEvent.RaiseLoadRequestEvent(menuScene, menuPos, true);
        toMenu = true;
    }

    private void NewGame(){
        sceneToLoad = firstScene;
        sceneLoadEvent.RaiseLoadRequestEvent(sceneToLoad, firstPos, true);
        toMenu = false;
    }


    /// <summary>
    /// 场景加载
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="pos"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 pos, bool fadeScreen){
        if (isLoading){
            return;
        }


        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGO = pos;
        this.fadeScreen = fadeScreen;

        if (currentLoadScene != null){
            StartCoroutine(UnLoadPreviousScene());
        }
        else{
            LoadNewScene();
        }
    }

    private IEnumerator UnLoadPreviousScene(){
        if (fadeScreen){
            // 渐入渐出
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);
        //广播事件调整血条显示
        unloadEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGO, true);

        if (currentLoadScene != null){
            yield return currentLoadScene.sceneReference.UnLoadScene();
        }

        // 关闭人物
        playerTransform.gameObject.SetActive(false);

        // 加载新场景
        LoadNewScene();
    }

    private void LoadNewScene(){
        // 通过Addressable将场景打包的类型，可以直接调用异步加载和卸载
        // 参数：（加载类型，加载后是否激活，异步优先级）
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);

        // AsyncOperation.completed
        // 操作完成时调用的事件。即使操作能够同步完成，
        // 也将在下一帧调用在创建它的调用所在的帧中注册的事件处理程序。
        // 如果处理程序是在操作完成后注册的，并且已调用 complete 事件，则将同步调用该处理程序。
        loadingOption.Completed += OnLoadCompleted;
    }

    // 加载场景后执行的方法
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj){
        currentLoadScene = sceneToLoad;

        playerTransform.position = positionToGO;

        playerTransform.gameObject.SetActive(true);


        if (fadeScreen){
            // 渐入渐出
            fadeEvent.FadeOut(fadeDuration);
        }


        isLoading = false;

        if (currentLoadScene.sceneType == SceneType.Location){
            toMenu = false;
            // 加载场景完成后事件 
            afterLoadEvent.Raise();
            GameObject.FindWithTag("Player").layer = LayerMask.NameToLayer("Player");
        }
    }

    public DataDefination GetDataID(){
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data){
        data.SaveGameScene(currentLoadScene);
    }

    public void LoadData(Data data){
        var playerID = playerTransform.GetComponent<DataDefination>().ID;

        if (data.characterPosDict.ContainsKey(playerID)){
            positionToGO = data.characterPosDict[playerID].ToVector3();
            sceneToLoad = data.GetSaveScene();
            OnLoadRequestEvent(sceneToLoad, positionToGO, true);
            loadAfterState = true;
        }
    }
}