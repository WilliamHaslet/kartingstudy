using UnityEngine;

public class AICartCheckpoint : MonoBehaviour
{

    [SerializeField] private float startDelay;
    [SerializeField] private float driftThreshold;
    [SerializeField] private float inaccuracy;
    [SerializeField] private Transform cartCenter;
    
    private CartMovement cartMovement;
    private LapCounter lapCounter;
    private Transform currentCheckpoint;
    private Vector3 currentTarget;
    
    private void Start()
    {

        cartMovement = GetComponent<CartMovement>();

        lapCounter = GetComponent<LapCounter>();

        GetComponent<FallReturner>().AddFallCallback(FallCallback);

        cartMovement.SetCanMove(false);

    }

    private void Update()
    {

        StartDelay();

        if (currentCheckpoint != lapCounter.GetNextCheckPoint())
        {

            currentCheckpoint = lapCounter.GetNextCheckPoint();

            currentTarget = currentCheckpoint.position + (currentCheckpoint.right * Random.Range(-inaccuracy, inaccuracy));

        }

        DebugShapes.DrawSphere(currentTarget, 0.3f, Color.red, 0);

        float dot = Vector3.Dot((currentTarget - cartCenter.position).normalized, cartCenter.right);

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

        //currentCheckpoint = lapCounter.GetNextCheckPoint();

        currentTarget = currentCheckpoint.position + (currentCheckpoint.right * Random.Range(-inaccuracy, inaccuracy));

    }

}
