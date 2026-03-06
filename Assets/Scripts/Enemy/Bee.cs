using UnityEngine;

public class Bee : Enemy{
    [Header("移动范围")]
    public float patrolRadius;

    protected override void Awake(){
        base.Awake();
        patrolState = new BeePatrolState();
        chaseState = new BeeChaseState();
    }

    public override bool FoundPlayer(){
        var obj = Physics2D.OverlapCircle(transform.position + (Vector3)centerOffset, checkDistance, attackLayer);
        if (obj){
            attacker = obj.transform;
        }

        return obj;
    }

    public override void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset, patrolRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            transform.position + new Vector3(centerOffset.x * faceDir.x, centerOffset.y, 0), checkDistance);
    }

    public override Vector3 GetNewPoint(){
        var targetX = Random.Range(-patrolRadius, patrolRadius);
        var targetY = Random.Range(-patrolRadius, patrolRadius);
        return spwanPoint + new Vector3(targetX, targetY);
    }

    public override void Move(){
        
    }
}