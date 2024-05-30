using Unity.Netcode;
using UnityEngine;

public class WheelEffectsManager : NetworkBehaviour
{
    public WheelCollider frontRight;
    public WheelCollider frontLeft;
    public WheelCollider backRight;
    public WheelCollider backLeft;
    public GameObject smokePrefab;
    public GameObject tireTrailPrefab;
    public float slipAllowance = 0.01f;
    private Vector3 previousVelocity = Vector3.zero;
    [SerializeField] public Transform centerOfMass;
    private float slipThreshold = 5f;

    private WheelParticles wheelParticles;
    private int gameObjectInstanceId;

    void Start()
    {
        gameObjectInstanceId = gameObject.GetInstanceID();

        slipAllowance = 0.00003f;
        InitiateParticles();
    }

    void Update()
    {

        if (GameManager.Instance.IsGamePlaying())
            CheckAndApplyWheelEffects();
        else return;

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
}
