using System.Collections.Generic;
using UnityEngine;

public class InputRecorder : MonoBehaviour
{

    public enum InputAction
    {
        turn,
        accelerate,
        drift
    }

    private List<InputAction> inputActions;
    private List<float> deltaTimes;

    public void RecordInput(InputAction inputAction)
    {



    }

}
