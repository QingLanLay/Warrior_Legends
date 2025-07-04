using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour{
    public PlayerStatBar playerStatBar;

    [Header("事件监听")]
    public CharacterEventSO healthEvent;

    public VoidEventSO pasueEvent;
    public FloatEventSO syncVolumeEvent;

    [FormerlySerializedAs("loadEvent")]
    public SceneLoadEventSO unloadedSceneEvent;

    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;

    [Header("组件")]
    public GameObject gameOverPanel;

    public GameObject restartBtn;
    public GameObject mobileTouch;
    public Button settingsBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;

    private void Awake(){
# if UNITY_STANDALONE
        mobileTouch.SetActive(false);
# endif

        settingsBtn.onClick.AddListener(TogglePausePanel);
    }

    private void OnEnable(){
        healthEvent.OnEventRaised += OnHealthEvent;
        unloadedSceneEvent.LoadRequestEvent += OnUnLoadSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
    }

    private void OnDisable(){
        healthEvent.OnEventRaised -= OnHealthEvent;
        unloadedSceneEvent.LoadRequestEvent -= OnUnLoadSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
    }

    private void TogglePausePanel(){
        // 根据GameObject自身激活情况取反
        pausePanel.SetActive(!pausePanel.activeSelf);
        // 如果激活暂停，没激活游戏运行
        if (pausePanel.activeSelf){
            pasueEvent.Raise();
        }

        Time.timeScale = pausePanel.activeSelf ? 0 : 1;
    }

    private void OnSyncVolumeEvent(float amount){
        volumeSlider.value = (amount + 80f) / 100f;
    }

    private void OnGameOverEvent(){
        gameOverPanel.SetActive(true);
    }

    private void OnLoadDataEvent(){
        gameOverPanel.SetActive(false);
    }


    private void OnUnLoadSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2){
        var isMenu = sceneToLoad.sceneType == SceneType.Menu;
        playerStatBar.gameObject.SetActive(!isMenu);
    }

    private void OnHealthEvent(Character character){
        var persentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChanged(persentage);

        playerStatBar.OnPowerChanged(character);
    }
}