using UnityEngine;

public class SpeedLinesManager : MonoBehaviour
{

    [SerializeField] private Transform trackedTransform;
    [SerializeField] private float lineCountScaling;
    [SerializeField] private float lineSpeedScaling;
    [SerializeField] private float lineOpacityScaling;
    [SerializeField] private float lineWidthScaling;
    [SerializeField] private float lineCenterScaling;

    private SpeedLines speedLines;
    private Vector3 lastPosition;

    private void Start()
    {

        speedLines = GetComponent<SpeedLines>();

    }

    private void Update()
    {

        float velocity = ((trackedTransform.transform.position - lastPosition) / Time.deltaTime).magnitude;

        speedLines.SetValues(Mathf.Clamp(Mathf.RoundToInt(velocity * lineCountScaling), 0, 15), 1 / (velocity * lineSpeedScaling), Mathf.Clamp01(velocity * lineOpacityScaling), Mathf.Clamp(velocity * lineWidthScaling, 0, 0.06f), 1 / (velocity * lineCenterScaling));

        lastPosition = trackedTransform.transform.position;

    }

}
