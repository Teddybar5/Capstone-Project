using UnityEngine;

public class GroundSnap : MonoBehaviour
{
    public LayerMask groundMask;
    public float hoverHeight = 0.05f;   // distance of body pivot above surface
    public float rayStartOffset = 1f;   // start the ray above the gecko
    public float rayLength = 3f;
    public float positionSmooth = 15f;
    public float rotationSmooth = 10f;

    void LateUpdate()
    {
        Vector3 origin = transform.position + transform.up * rayStartOffset;

        if (Physics.Raycast(origin, -transform.up, out RaycastHit hit, rayLength, groundMask))
        {
            float tPos = 1f - Mathf.Exp(-positionSmooth * Time.deltaTime);
            float tRot = 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime);

            Vector3 targetPos = hit.point + hit.normal * hoverHeight;
            transform.position = Vector3.Lerp(transform.position, targetPos, tPos);

            Quaternion targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, tRot);
        }
    }
}