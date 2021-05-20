using UnityEngine;

public class BoostZone : MonoBehaviour
{

    [SerializeField] private float boostStrength;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<CartMovement>() != null)
        {

            other.GetComponent<CartMovement>().AddVelocity(transform.forward * boostStrength);

        }

    }

}
