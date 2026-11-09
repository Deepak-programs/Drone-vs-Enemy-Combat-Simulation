using System.Collections;
using UnityEngine;

public class PatrolState : EnemyState
{
    Coroutine patrolWait;

    public PatrolState(EnemyMovement enemy) : base(enemy) {}

    public override void Enter()
    {
        enemy.navMeshAgent.isStopped = false;
        enemy.navMeshAgent.SetDestination(enemy.TargetPoint());
    }
    public override void Update()
    {
        if(enemy.DetectPlayer)
        {
            enemy.ChangeState(enemy.attackState);
            return;
        }

        if(enemy.HasReached() && patrolWait == null)
        {
            patrolWait = enemy.StartCoroutine(WaitAtPoint());
        }
    }
    public override void Exit()
    {
        if(patrolWait != null)
        {
            enemy.StopCoroutine(patrolWait);
            patrolWait = null;
        }
    }
    IEnumerator WaitAtPoint()
    {
        yield return new WaitForSeconds(enemy.PatrolPointWaitTime);
        enemy.navMeshAgent.SetDestination(enemy.TargetPoint());
        patrolWait = null;
    }
}
