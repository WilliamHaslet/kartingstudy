using UnityEngine;

public class VelocityTracker : MonoBehaviour
{

    private float speed;
    private Vector3 lastPosition;

    private void LateUpdate()
    {

        speed = ((transform.position - lastPosition) / Time.deltaTime).magnitude;

        lastPosition = transform.position;

    }

    public float GetSpeed()
    {

        return speed;

    }

}
