using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class Sign : MonoBehaviour{
    public GameObject signSprite;
    public Transform player;
    public bool canPress;

    private Animator animator;
    
    private PlayerInputControl playerInput; 
    
    private Iinteractable targetItem;

    private string otherTag;

    private void Awake(){
        animator = signSprite.GetComponent<Animator>();
        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable(){
        InputSystem.onActionChange += OnActionChange;
        playerInput.Gameplay.Cofirm.started += OnConfirm;

    }

    private void OnDisable(){
        canPress = false;
    }


    private void Update(){
        signSprite.GetComponent<SpriteRenderer>().enabled = (canPress && otherTag != "Untagged");
        signSprite.transform.localScale = player.localScale;

  

    }
    
    private void OnConfirm(InputAction.CallbackContext obj){
        if (canPress && otherTag != "Untagged" ){
            targetItem.TriggerAction();
            GetComponent<AudioDefination>().PlayAudioClip();
        }
    }
    
    /// <summary>
    /// 检测设备输入切换动画
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="actionChange"></param>
    private void OnActionChange(object obj, InputActionChange actionChange){
        if (actionChange == InputActionChange.ActionStarted){
            // Debug.Log(((InputAction)obj).activeControl.device);
            var d = ((InputAction)obj).activeControl.device;
            switch (d.device){
                case Keyboard:
                    animator.Play("keyBoard");
                    break;
                case XInputControllerWindows:
                    animator.Play("ps");
                    break;
            }
          
        }
    }
    
    
    private void OnTriggerStay2D(Collider2D other){

        if (other.CompareTag("Interactable")){
            canPress = true;
            targetItem = other.GetComponent<Iinteractable>();
        }

        otherTag = other.tag;

    }

    private void OnTriggerExit2D(Collider2D other){
        canPress = false;
    }
}