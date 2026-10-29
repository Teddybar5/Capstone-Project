using UnityEngine;

public class FootGroundSnap : MonoBehaviour
{
    public LayerMask groundMask;
    public float footOffset = 0.02f;   // keep the foot slightly above the surface
    public float rayStartHeight = 0.5f;
    public float rayLength = 2f;

    void LateUpdate()
    {
        Vector3 origin = transform.position + Vector3.up * rayStartHeight;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, groundMask))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + footOffset;
            transform.position = pos;

            // optional: tilt the foot to the surface
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.parent.rotation;
        }
    }
}