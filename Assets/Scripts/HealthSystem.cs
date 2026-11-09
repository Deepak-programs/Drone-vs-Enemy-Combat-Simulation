using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamagable
{
    [SerializeField] float health;

    public void DamageApply(float num)
    {
        health -= num;
        if(health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
