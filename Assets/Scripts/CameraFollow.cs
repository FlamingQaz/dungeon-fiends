using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public float followSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0f, 10f, -10f); // Added camera offset

    private void FixedUpdate()
    {
        Vector3 targetPos = playerTransform.position + cameraOffset; // Added camera offset
        Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        transform.position = smoothedPos;
    }
}