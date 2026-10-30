using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Attach to the gecko. Shows a world-space speech bubble with typewriter text
/// when the player gets close, and hides it when they walk away.
/// </summary>
public class GeckoDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Canvas bubbleCanvas;          // World Space canvas, child of the gecko
    [SerializeField] TextMeshProUGUI bubbleText;   // TMP text inside the canvas

    [Header("Behavior")]
    [SerializeField] float triggerDistance = 2f;   // meters
    [SerializeField] float charsPerSecond = 40f;
    [SerializeField] float holdTimeAfterLine = 2f;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField]
    string[] lines =
    {
        "Hey there, big friend!",
        "I found a really nice warm rock over there.",
        "Want to come check it out?"
    };

    Transform head;
    bool talking;
    bool hasSpokenThisVisit;
    Coroutine routine;

    void Start()
    {
        head = Camera.main.transform;   // XR Origin's camera is tagged MainCamera
        bubbleCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(head.position, transform.position);

        if (!talking && !hasSpokenThisVisit && dist < triggerDistance)
        {
            routine = StartCoroutine(Talk());
        }
        else if (dist > triggerDistance * 1.5f)
        {
            // Player walked away: hide bubble and allow talking again next visit
            if (talking) StopTalking();
            hasSpokenThisVisit = false;
        }
    }

    // Keep the bubble facing the player's head
    void LateUpdate()
    {
        if (!bubbleCanvas.gameObject.activeSelf) return;
        Vector3 away = bubbleCanvas.transform.position - head.position;
        bubbleCanvas.transform.rotation = Quaternion.LookRotation(away, Vector3.up);
    }

    IEnumerator Talk()
    {
        talking = true;
        bubbleCanvas.gameObject.SetActive(true);

        foreach (string line in lines)
        {
            bubbleText.text = line;
            bubbleText.maxVisibleCharacters = 0;
            bubbleText.ForceMeshUpdate();

            // Typewriter effect
            int total = line.Length;
            float shown = 0f;
            while (shown < total)
            {
                shown += charsPerSecond * Time.deltaTime;
                bubbleText.maxVisibleCharacters = Mathf.Min(Mathf.FloorToInt(shown), total);
                yield return null;
            }

            yield return new WaitForSeconds(holdTimeAfterLine);
        }

        StopTalking();
        hasSpokenThisVisit = true;   // don't repeat until player leaves and returns
    }

    void StopTalking()
    {
        if (routine != null) StopCoroutine(routine);
        talking = false;
        bubbleCanvas.gameObject.SetActive(false);
    }

    /// <summary>Call this from other scripts/events to make him say something specific.</summary>
    public void Say(string message)
    {
        if (routine != null) StopCoroutine(routine);
        lines = new[] { message };
        hasSpokenThisVisit = false;
        routine = StartCoroutine(Talk());
    }
}