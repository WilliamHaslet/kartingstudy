using UnityEngine;

public class AICartSpline : MonoBehaviour
{

    [SerializeField] private float startDelay;
    [SerializeField] private float driftThreshold;
    [SerializeField] private float inaccuracy;
    [SerializeField] private Transform cartCenter;
    [SerializeField] private TrackData trackData;
    [SerializeField] private float targetDistance;

    private CartMovement cartMovement;
    private LapCounter lapCounter;
    private Vector3 splinePoint;
    private Vector3 inaccuracyOffset;
    private float currentDistanceAlongSpline;

    private void Start()
    {

        cartMovement = GetComponent<CartMovement>();

        lapCounter = GetComponent<LapCounter>();

        GetComponent<FallReturner>().AddFallCallback(FallCallback);

        cartMovement.SetCanMove(false);

        currentDistanceAlongSpline = targetDistance;

        splinePoint = trackData.trackSpline.GetSplinePositionAtDistance(currentDistanceAlongSpline);

        inaccuracyOffset = Random.insideUnitSphere * inaccuracy;

    }

    private void Update()
    {

        StartDelay();

        float distance = Vector3.Distance(transform.position, splinePoint);

        if (distance < targetDistance)
        {

            currentDistanceAlongSpline += targetDistance - distance;

        }

        splinePoint = trackData.trackSpline.GetSplinePositionAtDistance(currentDistanceAlongSpline);

        DebugShapes.DrawSphere(splinePoint, 0.1f, Color.black, 0);

        DebugShapes.DrawSphere(splinePoint + inaccuracyOffset, 0.2f, Color.red, 0);

        DebugShapes.DrawCircle(splinePoint, targetDistance, Color.black, 0, 20);

        float dot = Vector3.Dot(((splinePoint + inaccuracyOffset) - cartCenter.position).normalized, cartCenter.right);

        cartMovement.Move(new Vector2(dot, 1));

        if (Mathf.Abs(dot) >= driftThreshold)
        {

            cartMovement.Drift(true);
            
        }
        else
        {

            cartMovement.Drift(false);
            
        }

    }

    private void StartDelay()
    {

        if (startDelay > 0)
        {

            startDelay -= Time.deltaTime;

            if (startDelay <= 0)
            {

                cartMovement.SetCanMove(true);

            }

        }

    }

    private void FallCallback()
    {

        //currentDistanceAlongSpline = trackData.trackSpline.GetSplineLength() * lapCounter.GetLapCompletionAtNext();

        inaccuracyOffset = Random.insideUnitSphere * inaccuracy;

        currentDistanceAlongSpline = (trackData.trackSpline.GetSplineLength() * lapCounter.GetLapCompletionAtLast()) + targetDistance;

        splinePoint = trackData.trackSpline.GetSplinePositionAtDistance(currentDistanceAlongSpline);

    }

}
