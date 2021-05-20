using UnityEngine;
using UnityEngine.UI;

public class LapDisplay : MonoBehaviour
{

    [SerializeField] private LapCounter lapCounter;

    private Text lapText;
    private int lapCount = 1;

    private void Start()
    {

        lapText = GetComponent<Text>();

        lapCounter.AddLapCallback(IncrementLap);

    }

    private void IncrementLap()
    {

        lapCount++;

        lapText.text = lapCount.ToString() + "/3";

    }

}
