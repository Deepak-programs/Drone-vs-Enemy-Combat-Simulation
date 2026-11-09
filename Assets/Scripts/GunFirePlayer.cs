using UnityEngine;
using System.Collections;

public class GunFirePlayer : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] float firingCooldownTime = 0.3f;
    [SerializeField] float BulletSpeed = 50f;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] Transform BulletAimPos;

    [Header("Effects")]
    [SerializeField] ParticleSystem particleClip;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;

    [Header("Targeting")]
    [SerializeField] float targetRange = 200f;
    [SerializeField] LayerMask targetLayer;

    bool firing;

    Controller inputActions;
    Controller.PlayerActions playerActions;

    void OnEnable() { inputActions?.Enable(); }
    void OnDisable() { inputActions?.Disable(); }

    void Awake()
    {
        inputActions = new Controller();
        playerActions = inputActions.Player;
    }

    void Update()
    {
        if(playerActions.Shoot.IsPressed())
        {
            if(!firing)
            {
                StartCoroutine(FireBullet());
            }
        }
    }

    IEnumerator FireBullet()
    {
        firing = true;

        yield return new WaitForSeconds(firingCooldownTime);

        Camera cam = Camera.main;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 targetPoint = ray.GetPoint(targetRange);

        if (Physics.Raycast(ray, out RaycastHit hit, targetRange, targetLayer))
        {
            targetPoint = hit.point;
        }

        Vector3 direction = (targetPoint - BulletAimPos.position).normalized;

        GameObject ammo = Instantiate(BulletPrefab, BulletAimPos.position,Quaternion.identity);

        ammo.SetActive(true);

        ammo.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);

        Rigidbody rb = ammo.GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.linearVelocity = direction * BulletSpeed;

        Collider bulletCollider = ammo.GetComponent<Collider>();

        if (bulletCollider != null) {bulletCollider.enabled = true;}

        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        if (particleClip != null)
            particleClip.Play();

        Destroy(ammo, 3f);

        firing = false;
    }
}
