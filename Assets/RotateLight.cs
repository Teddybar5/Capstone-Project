using UnityEngine;

public class DayNightCycleStable : MonoBehaviour
{
    [Tooltip("Total duration of one full day/night cycle in seconds (120 = 2 minutes).")]
    [SerializeField] private float cycleDuration = 120f;

    private float timer = 0f;
    private Quaternion initialRotation;

    void Start()
    {
        // Cache the exact starting rotation (where you manually angled your dawn/sunrise)
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Smoothly accumulate time, looping back to 0 at the end of the 2 minutes
        timer += Time.deltaTime;
        if (timer >= cycleDuration)
        {
            timer %= cycleDuration;
        }

        // Calculate progress from 0.0 to 1.0 over the 120 seconds
        float progress = timer / cycleDuration;
        float currentAngleX = progress * 360f;

        // Apply rotation purely via Quaternion multiplication. 
        // This completely avoids euler-angle flipping and frame-rate jitter.
        transform.rotation = initialRotation * Quaternion.Euler(-currentAngleX, 0f, 0f);
    }
}