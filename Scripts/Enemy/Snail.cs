public class Snail : Enemy{
    protected override void Awake(){
        base.Awake();
        
        maxSpeed = 0.6f;
        patrolState = new SnailPatrolState();
        // chaseState = new SnailChaseState();
        skillState = new SnailSkillState();
    }
}