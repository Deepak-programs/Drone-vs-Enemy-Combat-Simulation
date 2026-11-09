using UnityEngine;

public abstract class EnemyState
{
    protected EnemyMovement enemy;
    public EnemyState(EnemyMovement enemy)
    {
        this.enemy = enemy;
    }
    public virtual void Enter() {}
    public virtual void Update() {}
    public virtual void Exit() {}
}

public interface IDamagable
{
    void DamageApply(float damage);
}
