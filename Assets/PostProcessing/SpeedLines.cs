using UnityEngine;

public class SpeedLines : MonoBehaviour
{

    [System.Serializable]
    private struct Triangle
    {
        public Vector2 point1;
        public Vector2 point2;
        public Vector2 point3;
    }

    [SerializeField] private Material speedLinesMaterial;
    [SerializeField] private float opacity;
    [Header("Random")]
    [SerializeField] private int triangleCount;
    [SerializeField] private float baseWidth;
    [SerializeField, Range(0, 1)] private float safeRadius;
    [SerializeField] private int safeLoopLimit;
    [SerializeField] private bool play;
    [SerializeField] private float changeTime;
    [Header("Preset")]
    [SerializeField] private Triangle[] triangles;
    [SerializeField] private bool usePreset;

    private Vector4[] points;
    private float changeTimer;

    private void Update()
    {

        if (play)
        {

            changeTimer -= Time.deltaTime;

            if (changeTimer <= 0)
            {

                changeTimer = changeTime;

                SetShaderValues();

            }

        }
        
    }

    private void OnEnable()
    {

        SetShaderValues();

    }

    private void OnDisable()
    {

        speedLinesMaterial.SetInt("triangleCount", 0);

    }

    private void SetShaderValues()
    {

        if (usePreset)
        {

            points = new Vector4[triangles.Length * 3];

            for (int i = 0; i < triangles.Length * 3; i += 3)
            {

                points[i] = triangles[i / 3].point1;
                points[i + 1] = triangles[i / 3].point2;
                points[i + 2] = triangles[i / 3].point3;

            }

            speedLinesMaterial.SetInt("triangleCount", triangles.Length);

        }
        else
        {

            points = new Vector4[triangleCount * 3];

            for (int i = 0; i < triangleCount * 3; i += 3)
            {

                Triangle newTriangle = GenerateTriangle();

                points[i] = newTriangle.point1;
                points[i + 1] = newTriangle.point2;
                points[i + 2] = newTriangle.point3;

            }

            speedLinesMaterial.SetInt("triangleCount", triangleCount);

        }

        if (points.Length != 0)
        {

            speedLinesMaterial.SetFloat("opacity", opacity);

            speedLinesMaterial.SetVectorArray("triangesPoints", points);

            if (!gameObject.activeInHierarchy)
            {

                speedLinesMaterial.SetInt("triangleCount", 0);

            }

        }
        
    }

    private Vector2 GetRandomPoint()
    {

        return new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

    }

    private Triangle GenerateTriangle()
    {

        Vector2 tip;

        int loopCount = 0;

        do
        {

            tip = GetRandomPoint();

            loopCount++;

            if (loopCount >= safeLoopLimit)
            {

                break;

            }

        }
        while (Mathf.Abs((tip.x * 2) - 1) < safeRadius && Mathf.Abs((tip.y * 2) - 1) < safeRadius);

        Vector2 screenCenter = new Vector2(0.5f, 0.5f);

        Vector2 direction =  tip + (tip - screenCenter).normalized;

        Vector2 point2 = Vector2.zero;
        Vector2 point3 = Vector2.zero;

        if (Mathf.Abs((tip.x * 2) - 1) > Mathf.Abs((tip.y * 2) - 1))
        {

            point2.x = direction.x;
            point3.x = direction.x;
            
            point2.y = direction.y + (baseWidth / 2);
            point3.y = direction.y - (baseWidth / 2);

        }
        else
        {

            point2.y = direction.y;
            point3.y = direction.y;

            point2.x = direction.x + (baseWidth / 2);
            point3.x = direction.x - (baseWidth / 2);

        }

        return new Triangle()
        {
            point1 = tip,
            point2 = point2,
            point3 = point3
        };

    }

    public void SetValues(int lineCount, float lineSpeed, float lineOpacity, float lineBaseWidth, float centerRadius)
    {

        opacity = lineOpacity;
        triangleCount = lineCount;
        baseWidth = lineBaseWidth;
        //changeTime = lineSpeed;
        safeRadius = centerRadius;

    }

}
