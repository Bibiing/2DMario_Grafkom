using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 

    // Jarak kamera dari pemain (Offset)
    public Vector3 offset = new Vector3(0, 2, -12);

    // semakin kecil semakin lambat/smooth
    public float smoothSpeed = 0.125f;

    void LateUpdate() // LateUpdate untuk kamera agar tidak jitter
    {
        if (target == null) return;

        // Tentukan posisi tujuan
        float targetY = Mathf.Max(target.position.y, 0) + offset.y;

        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, targetY, offset.z);

        // Gerakkan kamera perlahan
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}