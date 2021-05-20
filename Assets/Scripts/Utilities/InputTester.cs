using UnityEngine;

public class InputTester : MonoBehaviour
{

    private void Update()
    {

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {

            if (Input.GetKeyDown(keyCode))
            {

                Debug.Log(keyCode);

            }

        }

    }

}
