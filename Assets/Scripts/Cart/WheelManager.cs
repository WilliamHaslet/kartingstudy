using UnityEngine;

public class WheelManager : MonoBehaviour
{

    [SerializeField] private float castDistance;
    [SerializeField] private float wheelRadius;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform[] wheelHolders;
    [SerializeField] private Transform[] wheels;
    [SerializeField] private bool debug;

    private Transform upTransform;

    private void Start()
    {

        upTransform = transform.GetChild(0);

    }

    private void Update()
    {

        for (int i = 0; i < wheelHolders.Length; i++)
        {

            bool hit = Physics.Raycast(wheelHolders[i].position + upTransform.up, -upTransform.up, out RaycastHit rayHit, castDistance + 1, groundLayers);

            if (hit)
            {

                wheels[i].position = rayHit.point + (upTransform.up * wheelRadius);

            }
            else
            {

                wheels[i].localPosition = Vector3.zero;

            }

            if (debug)
            {

                if (hit)
                {

                    Debug.DrawRay(wheelHolders[i].position + upTransform.up, -upTransform.up * (castDistance + 1), Color.green);

                    DebugShapes.DrawSphere(rayHit.point, 0.1f, Color.green, 0);

                }
                else
                {

                    Debug.DrawRay(wheelHolders[i].position + upTransform.up, -upTransform.up * (castDistance + 1), Color.red);

                }

            }

        }

    }

}
