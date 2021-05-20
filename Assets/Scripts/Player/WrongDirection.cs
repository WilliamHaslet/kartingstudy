using UnityEngine;

public class WrongDirection : MonoBehaviour
{

    [SerializeField] private GameObject wrongWayMessage;
    [SerializeField] private Transform cartCenter;
    [SerializeField] private float backwardDot;
    
    private LapCounter lapCounter;

    private void Start()
    {

        lapCounter = GetComponent<LapCounter>();
        
    }

    private void Update()
    {

        wrongWayMessage.SetActive(Vector3.Dot(lapCounter.GetCurrentDirection(), cartCenter.forward) <= backwardDot);

    }

}
