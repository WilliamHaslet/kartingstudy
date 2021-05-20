using System;
using UnityEngine;

public class FallReturner : MonoBehaviour
{

    [SerializeField] private Layer fallBoxLayer;
    
    private CartMovement cartMovement;
    private LapCounter lapCounter;
    private Action fallCallback;

    private void Start()
    {

        cartMovement = GetComponent<CartMovement>();

        lapCounter = GetComponent<LapCounter>();

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == fallBoxLayer)
        {

            transform.position = lapCounter.GetLastCheckPoint().position;

            cartMovement.ClearVelocity();

            cartMovement.SetCartRotation(lapCounter.GetLastCheckPoint().rotation);

            if (fallCallback != null)
            {

                fallCallback.Invoke();

            }

        }

    }

    public void AddFallCallback(Action callback)
    {

        fallCallback += callback;

    }

}
