using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{

    [SerializeField] private VelocityTracker trackedTransform;
    [SerializeField] private string format;

    private Text velocityText;

    private void Start()
    {

        velocityText = GetComponent<Text>();

    }

    private void LateUpdate()
    {

        velocityText.text = trackedTransform.GetSpeed().ToString(format);

    }

}
