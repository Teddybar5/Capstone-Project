using UnityEngine;

public class StardomeUniversalFade : MonoBehaviour
{
    [SerializeField] private Transform sunTransform;
    [SerializeField] private Renderer stardomeRenderer;

    [Header("Fade Angles (Matches your Sun's X Rotation)")]
    [SerializeField] private float fadeOutStartAngle = 200f;
    [SerializeField] private float fadeOutEndAngle = 180f;
    [SerializeField] private float fadeInStartAngle = 60f;
    [SerializeField] private float fadeInEndAngle = 0f;

    private Material stardomeMaterial;
    private string targetProperty = "";
    private Color originalColor;
    private float originalFloatValue = 1f;
    private bool useFloatProperty = false;

    void Start()
    {
        if (stardomeRenderer != null)
        {
            stardomeMaterial = stardomeRenderer.material;

            // Automatically detect what property this specific stardome shader uses
            if (stardomeMaterial.HasProperty("_Exposure"))
            {
                targetProperty = "_Exposure";
                useFloatProperty = true;
                originalFloatValue = stardomeMaterial.GetFloat(targetProperty);
            }
            else if (stardomeMaterial.HasProperty("_Brightness"))
            {
                targetProperty = "_Brightness";
                useFloatProperty = true;
                originalFloatValue = stardomeMaterial.GetFloat(targetProperty);
            }
            else if (stardomeMaterial.HasProperty("_TintColor"))
            {
                targetProperty = "_TintColor";
                useFloatProperty = false;
                originalColor = stardomeMaterial.GetColor(targetProperty);
            }
            else if (stardomeMaterial.HasProperty("_BaseColor"))
            {
                targetProperty = "_BaseColor";
                useFloatProperty = false;
                originalColor = stardomeMaterial.GetColor(targetProperty);
            }
            else if (stardomeMaterial.HasProperty("_Color"))
            {
                targetProperty = "_Color";
                useFloatProperty = false;
                originalColor = stardomeMaterial.GetColor(targetProperty);
            }
            else
            {
                Debug.LogWarning("Stardome shader doesn't recognize standard fade properties. Defaulting to GameObject toggle.");
            }
        }
    }

    void Update()
    {
        if (sunTransform == null || stardomeMaterial == null) return;

        // Get relative X rotation based on its starting position
        float currentX = sunTransform.eulerAngles.x % 360f;
        if (currentX < 0f) currentX += 360f;

        float starFactor = 0f; // 0 = hidden, 1 = fully visible

        if (currentX >= fadeOutStartAngle && currentX <= fadeOutEndAngle)
        {
            starFactor = Mathf.InverseLerp(fadeOutEndAngle, fadeOutStartAngle, currentX);
        }
        else if (currentX > fadeOutEndAngle && currentX < fadeInStartAngle)
        {
            starFactor = 0f;
        }
        else if (currentX >= fadeInStartAngle && currentX <= fadeInEndAngle)
        {
            starFactor = Mathf.InverseLerp(fadeInStartAngle, fadeInEndAngle, currentX);
        }
        else
        {
            starFactor = 1f;
        }

        // Apply the fade depending on what property the shader supports
        if (!string.IsNullOrEmpty(targetProperty))
        {
            if (useFloatProperty)
            {
                stardomeMaterial.SetFloat(targetProperty, originalFloatValue * starFactor);
            }
            else
            {
                Color fadeColor = originalColor * starFactor;
                stardomeMaterial.SetColor(targetProperty, fadeColor);
            }
        }
        else
        {
            // Fallback: If no shader property works, cleanly toggle the renderer on/off at the threshold
            stardomeRenderer.enabled = (starFactor > 0.05f);
        }
    }
}