using UnityEngine;

public class GunFire : MonoBehaviour
{
    [Header("Bullet Interact Layer")]
    [SerializeField] LayerMask layerMask;
    [Header("Attack Power")]
    [SerializeField] float attack;
    [Header("Interact Effect")]
    [SerializeField] ParticleSystem explosion;
    
    Vector3 lastPosition;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        if(rb != null)
        {
            Vector3 movement = rb.position - lastPosition;

            if (Physics.Raycast(lastPosition, movement.normalized,out RaycastHit hit, movement.magnitude, layerMask))
            {
                LayerMask solid = hit.transform.gameObject.layer;
                if((layerMask.value & (1 << solid)) != 0)
                {
                    GetComponent<Rigidbody>().useGravity = false;
                    explosion.Play();
                    Destroy(gameObject,explosion.main.duration);
                    IDamagable damageValue = hit.collider.GetComponentInParent<IDamagable>();
                    if(damageValue != null)
                    {
                        damageValue.DamageApply(attack);                       
                    }
                }
            }

            lastPosition = rb.position;
        }
    }
}
