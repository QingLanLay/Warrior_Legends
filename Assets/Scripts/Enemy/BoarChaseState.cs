using UnityEngine;

public class BoarChaseState : BaseState{
    public override void OnEnter(Enemy enemy){
        currentEnemy = enemy;
        // Debug.Log("Chase");

        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }


    public override void LogicUpdate(){
 
        if (currentEnemy.FoundPlayer()){
            currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        }
        
        if (currentEnemy.lostTimeCounter<=0){
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        
        // 如果是悬崖，撞左墙，撞右墙立刻转身
        if (!currentEnemy.physicsCheck.isGround ||
            (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) ||
            (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0)
            ){
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x , 1, 1);
        }
        
        

    }

    public override void PhysicsUpdate(){ }

    public override void OnExit(){
        currentEnemy.lostTimeCounter = 0;
        currentEnemy.anim.SetBool("run", false);
    }
}