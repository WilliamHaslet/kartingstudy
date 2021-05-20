using UnityEngine;

public class BoostManager : MonoBehaviour
{

    [SerializeField] private float maxBoostSpeed;
    [SerializeField] private float boostAddSpeed;
    [SerializeField] private float boostDecaySpeed;
    [SerializeField] private ParticleSystem[] boostEffects;
    [SerializeField] private float maxBoostJuice;
    [SerializeField] private float boostJuiceUseSpeed;
    [SerializeField] private AnimationCurve boostJuiceFillSpeedCurve;
    [SerializeField, HideInInspector] private Rect boostJuiceFillSpeedCurveRange;

    private float currentBoostSpeed;
    private float currentBoostJuice;
    private bool boosting;

    private void Start()
    {

        currentBoostJuice = maxBoostJuice;

    }

    private void Update()
    {

        if (boosting && currentBoostJuice > 0)
        {

            currentBoostSpeed += boostAddSpeed * Time.deltaTime;

            if (currentBoostSpeed > maxBoostSpeed)
            {

                currentBoostSpeed = maxBoostSpeed;

            }

            if (!boostEffects[0].isPlaying)
            {

                foreach (ParticleSystem effect in boostEffects)
                {

                    effect.Play();

                }

            }

            currentBoostJuice -= boostJuiceUseSpeed * Time.deltaTime;

            if (currentBoostJuice < 0)
            {

                currentBoostJuice = 0;

            }

        }
        else
        {

            currentBoostSpeed -= boostDecaySpeed * Time.deltaTime;

            if (currentBoostSpeed < 0)
            {

                currentBoostSpeed = 0;

            }

            if (boostEffects[0].isPlaying)
            {

                foreach (ParticleSystem effect in boostEffects)
                {

                    effect.Stop();

                }

            }

        }
        
        boosting = false;

    }

    public float GetCurrentBoost()
    {

        return currentBoostSpeed;

    }

    public void Boost()
    {

        boosting = true;

    }

    public void AddBoostJuice(float multiplier)
    {

        if (!boosting)
        {

            currentBoostJuice += boostJuiceFillSpeedCurve.Evaluate(multiplier) * Time.deltaTime;

            if (currentBoostJuice > maxBoostJuice)
            {

                currentBoostJuice = maxBoostJuice;

            }

        }

    }

    public float GetCurrentBoostJuice()
    {

        return currentBoostJuice;

    }

    public float GetMaxBoostJuice()
    {

        return maxBoostJuice;

    }

}
