using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform positionTarget;
    [SerializeField] private Transform bodyRotationTarget;
    [SerializeField] private Transform driftRotationTarget;
    [SerializeField, Range(0, 1)] private float driftRotationWeight;
    [SerializeField] private float turnStrength;
    [Header("FOV")]
    [SerializeField] private float fovStrength;
    [SerializeField] private float fovScaling;

    private Camera cartCamera;
    private VelocityTracker velocityTracker;
    private Vector3 currentRotation;
    private Vector3 rotationRef;
    private float baseFOV;
    private float fovRef;

    private void Start()
    {

        cartCamera = GetComponentInChildren<Camera>();

        baseFOV = cartCamera.fieldOfView;

        velocityTracker = positionTarget.GetComponent<VelocityTracker>();

    }

    private void LateUpdate()
    {

        transform.position = positionTarget.position;

        Vector3 targetRotation = Quaternion.Lerp(bodyRotationTarget.rotation, driftRotationTarget.rotation, driftRotationWeight).eulerAngles;

        currentRotation.x = Mathf.SmoothDampAngle(currentRotation.x, targetRotation.x, ref rotationRef.x, turnStrength);
        currentRotation.y = Mathf.SmoothDampAngle(currentRotation.y, targetRotation.y, ref rotationRef.y, turnStrength);
        currentRotation.z = Mathf.SmoothDampAngle(currentRotation.z, targetRotation.z, ref rotationRef.z, turnStrength);

        transform.rotation = Quaternion.Euler(currentRotation);

        float speed = velocityTracker.GetSpeed();

        cartCamera.fieldOfView = Mathf.SmoothDamp(cartCamera.fieldOfView, (speed * fovScaling) + baseFOV, ref fovRef, fovStrength);

    }

}
