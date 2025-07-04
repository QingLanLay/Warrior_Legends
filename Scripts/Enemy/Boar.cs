public class Boar : Enemy{
    protected override void Awake(){
        base.Awake();
        maxSpeed = 3f;
        patrolState = new BoarPatrolState();
        chaseState = new BoarChaseState();
    }
}