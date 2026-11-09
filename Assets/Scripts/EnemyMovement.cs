using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Partol Settings")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] float patrolPointWaitTime = 2f;
    [Header("Enemy Attack Settings")]
    [SerializeField] float attackAreaRadius = 1.5f;
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] Transform bulletAimPos;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] public Transform AimPointRig;
    [SerializeField] float firingCooldownTime = 1f;
    [SerializeField] float attackDistance = 15f;
    [Header("Audio And Particle")]
    [SerializeField] ParticleSystem gunFireParticle;
    [SerializeField] AudioSource gunFire;

    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Rig rig;
    [HideInInspector] public Transform player;
    [HideInInspector] public AudioSource audioSource;
    

    int _speed;
    int currentTarget;
    Vector3 attackPoint;
    public bool DetectPlayer {private set;get;}

    public float PatrolPointWaitTime => patrolPointWaitTime;
    public float FiringCooldownTime => firingCooldownTime;
    public float AttackDistance => attackDistance;
    public float BulletSpeed => bulletSpeed;
    public GameObject BulletPrefab => bulletPrefab;
    public Transform BulletAimPos => bulletAimPos;
    public ParticleSystem GunFireParticle => gunFireParticle;
    public AudioSource GunFire => gunFire;

    EnemyState currentState;
    [NonSerialized] public PatrolState patrolState;
    [NonSerialized] public AttackState attackState;

    void OnEnable() {GameEvent.PlayerDetected += PlayerDetectedFlag;}
    void OnDisable() {GameEvent.PlayerDetected -= PlayerDetectedFlag;}


    void Awake()
    {
        patrolState = new PatrolState(this);
        attackState = new AttackState(this);

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        AnimatorID();
        ChangeState(patrolState);

        rig = GetComponentInChildren<Rig>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
    }

    void AnimatorID()
    {
        _speed = Animator.StringToHash("Speed");
    }

    void Update()
    {
        AnimationChange();
        currentState?.Update();
    }

    public void ChangeState(EnemyState enemyState)
    {
        currentState?.Exit();
        currentState = enemyState;
        currentState.Enter();
    }

    public Vector3 TargetPoint()
    {
        if(waypoints == null) {return Vector3.zero;}
        if(waypoints.Length <= 1) {return waypoints[0].position;}
        currentTarget = (currentTarget + 1) % waypoints.Length;
        return waypoints[currentTarget].position;
    }

    void AnimationChange()
    {
        float currentSpeed = navMeshAgent.velocity.magnitude;
        float normalizedSpeed = currentSpeed / navMeshAgent.speed;
        animator.SetFloat(_speed, normalizedSpeed);
    }

    public bool HasReached()
    {
        if (navMeshAgent.pathPending) {return false;}

        if (!navMeshAgent.hasPath) {return false;}

        if (navMeshAgent.remainingDistance == Mathf.Infinity) { return false;}

        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 1.5f;
    }

    public Vector3 AttackAreaPoint()
    {
        if(player == null) {return attackPoint;}
        attackPoint = player.position + UnityEngine.Random.onUnitSphere * attackAreaRadius;
        return attackPoint;
    }

    void PlayerDetectedFlag(bool dectected)
    {
        DetectPlayer = dectected;
    }

    void OnFootStep(AnimationEvent animationEvent)
    {
        if(animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if(audioSource != null) {audioSource.Play();}
        }
    }
}
