using UnityEngine;

public class CarSounds : MonoBehaviour
{
    public float[] gearSpeeds;
    public float[] gearStartPitch;
    public float maxPitch;
    public float maxSpeed = 800f;

    private AudioSource carAudio;
    private Vector3 previousPosition;
    private float currentSpeed;
    private float previousSpeed;
    private int currentGear = 0;
    private float minPitch = 0.1f;

    private float velocitySmoothFactor = 1f;
    private Vector3 velocityAverage = Vector3.zero;

    void Start()
    {
        carAudio = GetComponent<AudioSource>();
        carAudio.spatialBlend = 1;
        carAudio.maxDistance = 100;
        carAudio.rolloffMode = AudioRolloffMode.Linear;
        carAudio.volume = 0.3f;
        previousPosition = transform.position;
    }

    void Update()
    {
        Vector3 instantVelocity = (transform.position - previousPosition) / Time.deltaTime;
        velocityAverage = Vector3.Lerp(velocityAverage, instantVelocity, Time.deltaTime * velocitySmoothFactor);
        currentSpeed = velocityAverage.magnitude;

        if (Mathf.Abs(previousSpeed - currentSpeed) > 0.1f)
        {
            UpdateEngineSound();
            previousSpeed = currentSpeed;
        }

        previousPosition = transform.position;
    }

    void UpdateEngineSound()
    {
        currentGear = 0;
        while (currentGear < gearSpeeds.Length && currentSpeed > gearSpeeds[currentGear])
        {
            currentGear++;
        }
        currentGear = Mathf.Clamp(currentGear, 0, gearSpeeds.Length - 1);

        float pitch = minPitch;
        if (currentGear == 0)
        {
            pitch = gearStartPitch[currentGear] + (currentSpeed / gearSpeeds[currentGear]) * (maxPitch - gearStartPitch[currentGear]);
        }
        else
        {
            float speedDiff = currentSpeed - gearSpeeds[currentGear - 1];
            float speedRange = gearSpeeds[currentGear] - gearSpeeds[currentGear - 1];
            float pitchRange = maxPitch - gearStartPitch[currentGear];
            pitch = gearStartPitch[currentGear] + (speedDiff / speedRange) * pitchRange;
        }
        carAudio.pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
}
