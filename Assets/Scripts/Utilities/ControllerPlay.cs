using UnityEngine;
using UnityEditor;

public class ControllerPlay : MonoBehaviour
{

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.JoystickButton8))
        {

            EditorApplication.isPlaying = !EditorApplication.isPlaying;

        }

    }

}
