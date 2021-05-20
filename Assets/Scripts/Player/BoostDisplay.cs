using UnityEngine;
using UnityEngine.UI;

public class BoostDisplay : MonoBehaviour
{

    [SerializeField] private Image displayImage;
    [SerializeField] private BoostManager boostManager;

    private void Update()
    {

        displayImage.fillAmount = boostManager.GetCurrentBoostJuice() / boostManager.GetMaxBoostJuice();

    }

}
