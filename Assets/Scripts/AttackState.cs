using System.Collections;
using UnityEngine;

public class AttackState : EnemyState
{
    bool firing;

    public AttackState(EnemyMovement enemy) : base(enemy) {}

    public override void Enter()
    {
        if(enemy.player == null) {enemy.ChangeState(enemy.patrolState);}
        enemy.navMeshAgent.stoppingDistance = enemy.AttackDistance;
        enemy.navMeshAgent.SetDestination(enemy.player.position);
        enemy.rig.weight = 1f;
        enemy.AimPointRig.position = enemy.AttackAreaPoint();
    }
    public override void Update()
    {
        if(!enemy.DetectPlayer) 
        {
            enemy.ChangeState(enemy.patrolState);
            return;
        }
        RotateToPlayer();
        if(!firing && enemy.HasReached())
        {
            enemy.animator.SetTrigger("Aim");
            enemy.StartCoroutine(FireBullet());
        }

    }
    public override void Exit()
    {
        enemy.rig.weight = 0f;
        enemy.navMeshAgent.stoppingDistance = 0f;
    }
    IEnumerator FireBullet()
    {
        firing = true;

        yield return new WaitForSeconds(enemy.FiringCooldownTime);
        if(enemy.player == null) {yield break;}
        
        Vector3 rotateTowardsPlayer = enemy.player.position - enemy.transform.position;
        rotateTowardsPlayer.y = 0f;

        if (rotateTowardsPlayer.sqrMagnitude > 0.01f) 
        {
            enemy.transform.rotation = Quaternion.LookRotation(rotateTowardsPlayer);
        }

        enemy.animator.SetTrigger("Fire");

        GameObject ammo = Object.Instantiate(enemy.BulletPrefab, enemy.BulletAimPos.position, Quaternion.identity);
        ammo.transform.localRotation = enemy.BulletAimPos.localRotation * Quaternion.Euler(90, 0, 0);;

        ammo.SetActive(true);
        ammo.GetComponent<Collider>().enabled = true;

        enemy.AimPointRig.position = enemy.AttackAreaPoint();
        Vector3 direction = (enemy.AimPointRig.position - ammo.transform.position).normalized;
        ammo.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);;

        enemy.GunFire.Play();

        Rigidbody rb = ammo.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearVelocity = direction * enemy.BulletSpeed;

        enemy.GunFireParticle.Play();

        Object.Destroy(ammo,1f);
        firing = false;
    }
    void RotateToPlayer()
    {
        if(enemy.player == null) {return;}
        Vector3 direction = enemy.player.position - enemy.transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }
    }
}
