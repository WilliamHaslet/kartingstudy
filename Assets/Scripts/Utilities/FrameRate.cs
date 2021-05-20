using UnityEngine;
using UnityEngine.UI;

public class FrameRate : MonoBehaviour
{

    [SerializeField] private int targetFPS;

    private Text text;

    private void Start()
    {

        Application.targetFrameRate = targetFPS;

        QualitySettings.vSyncCount = 0;

        text = GetComponent<Text>();

    }

    private void Update()
    {

        text.text = (1 / Time.deltaTime).ToString("f0");

    }

}
