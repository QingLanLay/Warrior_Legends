public class BoarPatrolState : BaseState{
    public override void OnEnter(Enemy enemy){
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate(){
        // 发现player切换到chase
        if (currentEnemy.FoundPlayer()){
            currentEnemy.lostTimeCounter = currentEnemy.lostTime;
            currentEnemy.SwitchState(NPCState.Chase);
        }

        if (!currentEnemy.physicsCheck.isGround ||
            (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) ||
            (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0)){
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        }
        else{
            currentEnemy.wait = false;
            currentEnemy.anim.SetBool("walk", true);
        }
    }

    public override void PhysicsUpdate(){ }


    public override void OnExit(){
        currentEnemy.wait = true;
        currentEnemy.anim.SetBool("walk", false);
    }
}