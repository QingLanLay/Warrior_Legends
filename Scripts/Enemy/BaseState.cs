using UnityEngine;


public abstract class BaseState{
    protected Enemy currentEnemy;
    public abstract void OnEnter(Enemy enemy);
    public abstract void LogicUpdate( ); // 逻辑判断
    public abstract void PhysicsUpdate(); // 物理判断
    public abstract void OnExit();
}