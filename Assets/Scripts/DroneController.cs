
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] float maxEngineForce = 15f;

    [Header("Flight Controls")]
    [SerializeField] float pitchPower = 8f;
    [SerializeField] float rotationSpeed = 3f;

    Rigidbody rb;
    Controller inputActions;
    Controller.PlayerActions playerActions;
    AudioSource audioSource;

    bool engineOn;
    float pitchInput;
    float rotationInput;
    float verticalInput;

    void OnEnable() {inputActions.Enable();}
    void OnDisable() {inputActions.Disable();}

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        inputActions = new Controller();
        playerActions = inputActions.Player;

        //Engine OFF and ON
        playerActions.Engine.performed += ctx => engineOn = !engineOn;
    }

    void Update()
    {
        ReadInput();
        PlayAudio();
    }

    void FixedUpdate()
    {
        HandleThrottle();
        HandleEngines();
        HandleRotation();
    }

    void ReadInput()
    {
        pitchInput = 0f;
        rotationInput = 0f;
        verticalInput = 0f;

        if (playerActions.W.IsPressed())
            pitchInput = 1f;

        if (playerActions.S.IsPressed())
            pitchInput = -1f;

        if (playerActions.D.IsPressed())
            rotationInput = 1f;

        if (playerActions.A.IsPressed())
            rotationInput = -1f;

        if (playerActions.Ascend.IsPressed())
            verticalInput = 1f;

        if (playerActions.Descend.IsPressed())
            verticalInput = -1f;
    }

    void HandleThrottle()
    {
        if(!engineOn){return;}
        rb.AddForce(Vector3.up * verticalInput * maxEngineForce,ForceMode.Force);
    }

    void HandleEngines()
    {
        if (!engineOn) {return;}

        Vector3 movement = new Vector3(0f, 0f,pitchInput);

        rb.AddForce(transform.TransformDirection(movement) * pitchPower,ForceMode.Force);
    }

    void HandleRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f,rotationInput * rotationSpeed * Time.fixedDeltaTime, 0f ));
    }

    void PlayAudio()
    {
        if(audioSource == null) {return;}
        if (engineOn && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        if (!engineOn && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
