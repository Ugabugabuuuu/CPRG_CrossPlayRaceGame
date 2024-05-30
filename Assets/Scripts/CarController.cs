using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : NetworkBehaviour
{
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] public WheelCollider backRight;
    [SerializeField] public WheelCollider backLeft;
    [SerializeField] private Transform fronRightTransform;
    [SerializeField] private Transform fronLeftTransform;
    [SerializeField] private Transform backLeftTransform;
    [SerializeField] private Transform backRightTransform;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] public float acceleration = 1000f;
    [SerializeField] private float breakingForce = 5000f;
    [SerializeField] private float maxTurnAngle = 20f;
    [SerializeField] public float maxSpeed = 240f;
    private float maxBackwardSpeed = 20f;
    [SerializeField] private bool isBreaking;
    private PlayerInput playerInput;
    public InputAction moveAction;
    private InputAction handBreakeAction;
    public Rigidbody rb;
    private float accelerationInput;
    [SerializeField] private GameObject tireTrail;
    [SerializeField] private Material breakLightMaterial;
    float brakingInput;
    float turnInput;
    [SerializeField] private PlayerColors playerColors;
    [SerializeField] private List<Transform> spawnPositionList;
    public static event EventHandler OnAnyPlayerSpawned;
    private Transform respawnTransform;
    public static CarController LocalInstance { get; private set; }

    private bool finished = false;
    public bool isMe = false;
    public GameObject tireTrailPrefab;
    public float slipAllowance = 0.01f;
    private WheelParticles wheelParticles;
    private int currentCheckpoint = -1;
    public float currentAcceleration;
    public float currentBrakeForce;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

    public static bool IsOnAnyPlayerSpawnedNull()
    {
        return OnAnyPlayerSpawned == null;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        handBreakeAction = playerInput.actions.FindAction("HandBreak");

        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerColors.SetPlayerCarColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorId));

        GameObject finishLineGameObject = GameObject.FindWithTag("FinishLine");

        slipAllowance = IsOwner ? 0.3f : 0.0000001f;

        InitiateParticles();

        if (IsOwner)
            GameInput.Instance.onRespawn += Instance_onRespawn;
    }
    private void Instance_onRespawn(object sender, EventArgs e)
    {
        Respawn();
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        Vector3 a = new Vector3(spawnPositionList[MultiplayerManager.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)].position.x,
            spawnPositionList[MultiplayerManager.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)].position.y-45,
            spawnPositionList[MultiplayerManager.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)].position.z);
        transform.position = a;
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }
#if !UNITY_INCLUDE_TESTS
    private void FixedUpdate()
    {
        if (finished || !GameManager.Instance.IsGamePlaying())
        {
            ApplyBrakes(1);
            return;
        }
        accelerationInput = moveAction.ReadValue<Vector2>().y;

        brakingInput = handBreakeAction.ReadValue<float>();

        turnInput = moveAction.ReadValue<Vector2>().x;

        ApplyAcceleration(accelerationInput);
        ApplyBrakes(brakingInput);
        ApplySteering(turnInput);
        UpdateWheels();
        CheckAndApplyWheelEffects();
    }
#endif

    public void ApplyAcceleration(float input)
    {
#if !UNITY_INCLUDE_TESTS
        accelerationInput = moveAction.ReadValue<Vector2>().y;
#endif
        currentAcceleration = acceleration * input;
#if !UNITY_INCLUDE_TESTS
        if (input >=0)
        {
            if (rb.velocity.magnitude > maxSpeed/3.6f)
            {
                currentAcceleration = 0;
            }
        }
        else if (input <= 0)
        {
            if (rb.velocity.magnitude > maxBackwardSpeed/3.6f)
            {
                currentAcceleration = 0;
            }
        }
            backRight.motorTorque = currentAcceleration;
            backLeft.motorTorque = currentAcceleration;
#endif
    }
    public void ApplyBrakes(float input)
    {
        currentBrakeForce = breakingForce * input;
#if !UNITY_INCLUDE_TESTS
        backLeft.brakeTorque = currentBrakeForce;
        backRight.brakeTorque = currentBrakeForce;
        frontRight.brakeTorque = currentBrakeForce * 0.5f;
        frontLeft.brakeTorque = currentBrakeForce * 0.5f;
#endif
    }
    public float currentTurnAngle; //public for testing
    public void ApplySteering(float input)
    {

        currentTurnAngle = maxTurnAngle * input;
#if !UNITY_INCLUDE_TESTS
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
#endif
    }
    private void UpdateWheels()
    {
        UpdateWheel(frontLeft, fronLeftTransform);
        UpdateWheel(frontRight, fronRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);
    }

    private void UpdateWheel(WheelCollider collider, Transform transform)
    {
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);

        if (transform == fronRightTransform || transform == backRightTransform)
        {
            rotation *= Quaternion.Euler(0f, 180f, 0f);
        }
        transform.position = position;
        transform.rotation = rotation;
    }

    public InputAction GetMoveAction()
    {
        return moveAction;
    }
    public InputAction GetHandBreakeAction() { return handBreakeAction; }
    private void BreakeAllWheels(float input)
    {
        float currentBrakeForce = breakingForce * Mathf.Abs(input);
        backLeft.brakeTorque = currentBrakeForce;
        backRight.brakeTorque = currentBrakeForce;
        frontLeft.brakeTorque = currentBrakeForce;
        frontRight.brakeTorque = currentBrakeForce;
    }
    private void StopBreaking()
    {
        float currentBrakeForce = 0;
        backLeft.brakeTorque = currentBrakeForce;
        backRight.brakeTorque = currentBrakeForce;
        frontLeft.brakeTorque = currentBrakeForce;
        frontRight.brakeTorque = currentBrakeForce;
    }

    // Exposed for testing purposes
    public bool IsBreaking(float currentSpeed, float brakingInput)
    {
        return ((currentSpeed > 0 && brakingInput < 0) || (currentSpeed < 0 && brakingInput > 0));
    }

    private void enableBreakLights(bool breakLightsEnabled)
    {
        if (breakLightsEnabled)
        {

           // Debug.LogError("o tai sviesu nebus as nesuprantu " + breakingColor);
          //  breakingColor = Color.red;
          //  breakingColorIntensity = 10;
          //  breakLightMaterial.EnableKeyword("_EMISSION");
           // breakLightMaterial.SetColor("_EmissionColor", breakingColor * Mathf.Pow(2, breakingColorIntensity));
        }
        else disableBreakLights();
    }
    private void disableBreakLights()
    {
        breakLightMaterial.DisableKeyword("_EMISSION");
        breakLightMaterial.SetColor("_EmissionColor", Color.black);
    }

    // Exposed for testing purposes
    public float CalculateSpeed(Vector3 velocity)
    {
        float magnitude = velocity.magnitude;

        float forwardVelocity = Vector3.Dot(velocity, transform.forward);
        float speed;

        if (forwardVelocity >= 0)
        {
            speed = magnitude;
        }
        else
        {
            speed = -magnitude;
        }
        return speed;
    }
    public void Respawn()
    {

        if (IsOwner && respawnTransform != null)
        {

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, respawnTransform.rotation.eulerAngles.y, 0);
            Vector3 tmp = new Vector3(respawnTransform.position.x,
                respawnTransform.position.y - 10,
                respawnTransform.position.z);
            transform.position = tmp;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
            if (other.CompareTag("FinishLine") && IsOwner && currentCheckpoint == 8)
            {
            InGameMusicManager.Instance.InvokeOnGameEnded();
            finished = true;
           // PlatformUIController.Instance.InvokeOnFinished(); // disabled for tests, for manual tests use project in main branch
                MyEventArgs tmpArgs = new MyEventArgs(OwnerClientId);

            FinishLineTriggerCheck.Instance.InvokeEvent(tmpArgs);

            }
            if (IsOwner)
            {
                if (other.CompareTag("RespawnPoint"))
                {
                    if (int.TryParse(other.gameObject.name.Replace("RespawnPoint", ""), out int checkpointNumber))
                    {
                        if (checkpointNumber == currentCheckpoint + 1)
                        {
                            currentCheckpoint = checkpointNumber;
                            other.gameObject.SetActive(false);
                        respawnTransform = other.transform;
                        }
                    }
                }
            }
    }
    public int GetMyNetworkIdD()
    {
        return NetworkManager.Singleton.GetInstanceID();
    }
    private void InitiateParticles()
    {
        if (smokePrefab && tireTrailPrefab)
        {
            wheelParticles = new WheelParticles
            {
                FRWheel = Instantiate(smokePrefab, frontRight.transform.position - Vector3.up * frontRight.radius, Quaternion.identity, frontRight.transform).GetComponent<ParticleSystem>(),
                FLWheel = Instantiate(smokePrefab, frontLeft.transform.position - Vector3.up * frontLeft.radius, Quaternion.identity, frontLeft.transform).GetComponent<ParticleSystem>(),
                RRWheel = Instantiate(smokePrefab, backRight.transform.position - Vector3.up * backRight.radius, Quaternion.identity, backRight.transform).GetComponent<ParticleSystem>(),
                RLWheel = Instantiate(smokePrefab, backLeft.transform.position - Vector3.up * backLeft.radius, Quaternion.identity, backLeft.transform).GetComponent<ParticleSystem>(),
                FRWheelTrail = Instantiate(tireTrailPrefab, frontRight.transform.position - Vector3.up * frontRight.radius, Quaternion.identity, frontRight.transform).GetComponent<TrailRenderer>(),
                FLWheelTrail = Instantiate(tireTrailPrefab, frontLeft.transform.position - Vector3.up * frontLeft.radius, Quaternion.identity, frontLeft.transform).GetComponent<TrailRenderer>(),
                RRWheelTrail = Instantiate(tireTrailPrefab, backRight.transform.position - Vector3.up * backRight.radius, Quaternion.identity, backRight.transform).GetComponent<TrailRenderer>(),
                RLWheelTrail = Instantiate(tireTrailPrefab, backLeft.transform.position - Vector3.up * backLeft.radius, Quaternion.identity, backLeft.transform).GetComponent<TrailRenderer>()
            };
        }
    }

    private void CheckAndApplyWheelEffects()
    {
        UpdateWheelEffect(frontRight, wheelParticles.FRWheel, wheelParticles.FRWheelTrail);
        UpdateWheelEffect(frontLeft, wheelParticles.FLWheel, wheelParticles.FLWheelTrail);
        UpdateWheelEffect(backRight, wheelParticles.RRWheel, wheelParticles.RRWheelTrail);
        UpdateWheelEffect(backLeft, wheelParticles.RLWheel, wheelParticles.RLWheelTrail);
    }

    private void UpdateWheelEffect(WheelCollider wheelCollider, ParticleSystem wheelParticles, TrailRenderer wheelTrail)
    {
        WheelHit wheelHit;

        if (wheelCollider.GetGroundHit(out wheelHit))
        {
            bool isSlipping = Mathf.Abs(wheelHit.sidewaysSlip) > slipAllowance || Mathf.Abs(wheelHit.forwardSlip) > slipAllowance;
            if (isSlipping)
            {
                if (!wheelParticles.isPlaying)
                    wheelParticles.Play();
                wheelTrail.emitting = true;
            }
            else
            {
                if (wheelParticles.isPlaying)
                    wheelParticles.Stop();
                wheelTrail.emitting = false;
            }
        }
    }
    public Rigidbody GetRigidBody()
    {
        return rb;
    }
    public bool GetFinishState()
    {
        return finished;
    }

#if !UNITY_INCLUDE_TESTS
    private void OnDestroy()
    {
        GameInput.Instance.onRespawn -= Instance_onRespawn;
    }
#endif

}
public class WheelParticles
{
    public ParticleSystem FRWheel;
    public ParticleSystem FLWheel;
    public ParticleSystem RRWheel;
    public ParticleSystem RLWheel;

    public TrailRenderer FRWheelTrail;
    public TrailRenderer FLWheelTrail;
    public TrailRenderer RRWheelTrail;
    public TrailRenderer RLWheelTrail;
}
public class MyEventArgs : EventArgs
{
    public ulong ID { get; private set; }

    public MyEventArgs(ulong id)
    {
        ID = id;
    }
}